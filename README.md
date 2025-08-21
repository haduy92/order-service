# Flash Card Management System

A robust flash card management system built with .NET 8 and Clean Architecture principles.

## Quick Start

To get the application running:

1. Clone the repository
1. Navigate to the project directory
1. Run the database migrations:
   ```sh
   dotnet ef database update --context AppIdentityDbContext --project Infrastructure
   ```
1. Open terminal at src/Api and run:
   ```sh
   dotnet run
   ```
1. Access swagger page at [http://localhost:5000/swagger](http://localhost:5000/swagger)

## Completed features

* FE: not implemented
* BE:
  - Auth API: user sign-up, sign-in, sign-out and refresh token
  - Card API: search with paging (Full-Text search in Postgres), create, read, update, and delete cards
  - AI enhancement: generate a few definition sentences for a vocabulary world.
  - Global exception handler: allow only intended error details are sent to client
  - Authentication with JwtBearer
* CI/CD: not implemented
* Deployment: not implemented

## License

[MIT](https://choosealicense.com/licenses/mit/)
