using Asp.Versioning;
using FlashCard.Api.Extensions;
using FlashCard.Api.Services;
using FlashCard.Application.Extensions;
using FlashCard.Application.Interfaces.Application;
using FlashCard.Infrastructure.Data;
using FlashCard.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
   {
       options.DefaultApiVersion = new ApiVersion(1, 0);
       options.AssumeDefaultVersionWhenUnspecified = true;
       options.ReportApiVersions = true;
       options.ApiVersionReader = ApiVersionReader.Combine(
           new UrlSegmentApiVersionReader(),
           new HeaderApiVersionReader("X-Api-Version")
       );
   })
   .AddApiExplorer(options =>
   {
       options.GroupNameFormat = "'v'VVV";
       options.SubstituteApiVersionInUrl = true;
   });

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<CurrentUser>();
builder.Services.AddScoped<ICurrentUserInitializer, CurrentUser>(x => x.GetRequiredService<CurrentUser>());
builder.Services.AddScoped<ICurrentUser, CurrentUser>(x => x.GetRequiredService<CurrentUser>());
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("auth", new OpenApiInfo
    {
        Title = "Auth",
        Version = "v1",
        Description = "An collection of API to manage user's authentication",
    });
    options.SwaggerDoc("card", new OpenApiInfo
    {
        Title = "Card",
        Version = "v1",
        Description = "An collection of API to manage flashcards",
    });
});
builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("api", p =>
    {
        p.RequireAuthenticatedUser();
        p.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
    });

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

using (var scope = app.Services.CreateAsyncScope())
{
    var identityDbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
    if (identityDbContext.Database.EnsureCreated()) // If new database is created
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCurrentUser();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
