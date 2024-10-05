dotnet ef migrations add InitialModel --context AppDbContext --project FlashCard.Infrastructure --output-dir Migrations\Persistence
dotnet ef migrations add InitialModel --context AppIdentityDbContext --project FlashCard.Infrastructure --output-dir Migrations\Identity
dotnet ef database update --context AppDbContext --project FlashCard.Infrastructure
dotnet ef database update --context AppIdentityDbContext --project FlashCard.Infrastructure