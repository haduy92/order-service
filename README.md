# About the project

For this self-study project, the goal is to develop a .NET Web API using Clean Architecture principles, with a focus on integrating Entity Framework Core (EF Core) for data persistence and Identity for user authentication, enhanced with JWT (JSON Web Token) authentication. This will ensure secure and stateless authentication across the API. Additionally, the project will integrate an external AI-powered API to enhance the application's functionality. By exploring JWT-based Identity authentication, EF Core for data handling, and Clean Architecture for maintainability, this experiment will showcase modern techniques for building scalable, secure, and extensible APIs. The integration of AI services will further highlight the potential for intelligent features in web applications.

## Built with

* .NET 8
* AspNetCore.Identity vs Authentication.JwtBearer
* EntityFramework Core
* PostgreSQL
* Open AI (Chat Assistant)
* xUnit

## Getting Started

This is instruction to setup and run project in local machine.

### Prequisites

1. [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
2. [VisualStudioCode](https://code.visualstudio.com)
3. [Docker](https://www.docker.com)

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/github_username/repo_name.git
   ```
1. Install PostgreSQL using docker
   ```sh
   docker run --name postgresql -p 5455:5432 -e 'POSTGRES_USER=postgres' -e 'POSTGRES_PASSWORD=postgres' -v ./data:/var/lib/postgresql/data
   ```
1. Open file `appsettings.development.json` and input connection strings, api key...
1. Install EF tool (for db migration)
   ```sh
   dotnet tool install --global dotnet-ef
   ```
1. Migrate database (optional)
   ```sh
   dotnet ef database update --context AppDbContext --project FlashCard.Infrastructure
   dotnet ef database update --context AppIdentityDbContext --project FlashCard.Infrastructure
   ```
1. Open terminal at apis/src/FlashCard.Api and run
   ```sh
   dotnet run
   ```
1. Access swagger page at [http://localhost:5000/swagger](http://localhost:5000/swagger)

## Completed features

* FE: not implemented
* BE:
  - Auth API: user signup, login, and logout
  - Card API: search with paging (Full-Text search in Postgres), create, read, update, and delete cards
  - AI enhancement: generate a few definition sentences for a vocabulary world.
  - Global exception handler: allow only intended error details are sent to client 
  - Authentication with JwtBearer
* CI/CD: not implemented
* Deployment: not implemented

## Limitations

1. Using only jwt bearer token for api authentication causes a security hole when an user signout from webapp but another party is able to steal the token. Then that party can still call api using that token.
   - Current solution: set token expiration time to 5 min
   - Better solution: store token along with user id in redis db when singin/singup and delete it when singout
1. Can only generate text for a prompt

## Future enhancements

1. Better logging (trace id, correlation id)
1. Apply EventSourcing/CQRS: allow users to send a list of vacabulary as a command, then send an event to a consummer to tell it to generate texts, images in the background

## License

[MIT](https://choosealicense.com/licenses/mit/)