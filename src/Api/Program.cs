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

// Initialize database with seeding
await app.SeedDatabaseAsync();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Add exception handler middleware before response wrapper
app.UseMiddleware<ExceptionHandlerMiddleware>();

// Add current user middleware to set the current user context
app.UseMiddleware<CurrentUserMiddleware>();

// Add response wrapper middleware before FastEndpoints
app.UseMiddleware<ResponseWrapperMiddleware>();

// Use FastEndpoints instead of MapControllers
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

app.Run();

