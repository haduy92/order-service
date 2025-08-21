using System.Text.Json.Serialization;
using Api.Extensions;
using Api.Middlewares;
using Application.Extensions;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
   options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});;
builder.Services.AddApiVersion();
builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddDIs();
var app = builder.Build();

app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/auth/swagger.json", "auth");
        options.SwaggerEndpoint("/swagger/card/swagger.json", "card");
    });
}

MigrateDb();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CurrentUserMiddleware>();

app.MapControllers();
app.Run();

// Private methods
void MigrateDb()
{
    using (var scope = app.Services.CreateAsyncScope())
    {
        var identityDbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
        if (identityDbContext.Database.EnsureCreated()) // If new database is created
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }
    }
}

