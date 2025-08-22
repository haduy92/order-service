using System.Text.Json.Serialization;
using Api.Extensions;
using Api.Middlewares;
using Application.Extensions;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add FastEndpoints
builder.Services.AddFastEndpoints();

builder.Services.AddApiVersion();
builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddDIs();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen(); // Use FastEndpoints Swagger
}

MigrateDb();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CurrentUserMiddleware>();

// Use FastEndpoints instead of MapControllers
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

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

