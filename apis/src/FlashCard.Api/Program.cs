using FlashCard.Api.Extensions;
using FlashCard.Application.Extensions;
using FlashCard.Infrastructure.Data;
using FlashCard.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApiVersion();
builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddDependencyInjections();

var app = builder.Build();

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
app.UseCurrentUser();

app.MapControllers();
app.Run();

// private methods
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
