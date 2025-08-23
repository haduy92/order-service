# 🛍️ Order Service

A modern, scalable order management service built with .NET 8 and clean architecture principles.

## 📝 Overview

This application provides a comprehensive platform for managing e-commerce orders throughout their lifecycle. The system is designed to support distributed order processing workflows with real-time event handling and asynchronous processing capabilities.

## 🏗️ Architecture

The solution follows **Onion Architecture** (Clean Architecture) principles, ensuring loose coupling, high testability, and maintainability.

### Architecture Layers

```
┌─────────────────────────────────────┐
│            Presentation             │  → API Layer (FastEndpoints)
├─────────────────────────────────────┤
│            Application              │  → Use Cases, Commands, Queries
├─────────────────────────────────────┤
│            Infrastructure           │  → Data Access, External Services
├─────────────────────────────────────┤
│              Domain                 │  → Business Logic, Entities
└─────────────────────────────────────┘
```

- **Domain Layer**: Core business entities (Orders, OrderItems) and rules (zero dependencies)
- **Application Layer**: Use cases, CQRS commands/queries, DTOs, order processing logic
- **Infrastructure Layer**: Database access, external services, messaging
- **Presentation Layer**: REST API endpoints for order management

## 🛠️ Technologies & Frameworks

### Core Framework
- **.NET 8.0** - Latest LTS version
- **C# 12** - Latest language features

### Web API
- **FastEndpoints** - High-performance minimal API framework
- **ASP.NET Core** - Web framework
- **Swagger/OpenAPI** - API documentation
- **JWT Bearer Authentication** - Secure authentication

### Data & Persistence
- **Entity Framework Core** - ORM
- **PostgreSQL** - Primary database
- **Npgsql** - PostgreSQL provider

### Messaging & Communication
- **RabbitMQ** - Message broker for async order processing
- **MediatR** - In-process messaging (CQRS pattern)

### Authentication & Security
- **ASP.NET Core Identity** - User management
- **JWT Tokens** - Stateless authentication

### AI Integration
- **OpenAI SDK** - AI-powered features for order optimization

### Development & Testing
- **xUnit** - Testing framework
- **Moq** - Mocking framework
- **AutoFixture** - Test data generation
- **FluentAssertions** - Assertion library

### Mapping & Validation
- **Riok.Mapperly** - Source generator-based mapping
- **Custom validation** - Business rule validation

## 📂 Project Structure

```
src/
├── Api/                    # Web API layer (FastEndpoints)
├── Application/            # Application services, CQRS handlers
├── Consumer/               # Background service for order processing
├── Domain/                 # Core business logic and entities
├── Infrastructure/         # Data access and external services
└── Shared/                 # Common utilities and cross-cutting concerns

tests/
├── Application.Tests/      # Unit tests for application layer
└── Infrastructure.Tests/   # Unit tests for infrastructure layer
```

## 🔄 Component Workflow

### Order Processing Flow
1. **API Layer** receives order requests via FastEndpoints
2. **Authentication** validates JWT tokens
3. **Application Layer** processes order commands/queries via MediatR
4. **Domain Layer** applies business rules and validation
5. **Infrastructure Layer** persists orders to PostgreSQL
6. **Messaging** publishes order events to RabbitMQ for async processing

### Event-Driven Architecture
- **Order Created Events** trigger inventory checks and payment processing
- **Order Status Changed Events** handle workflow transitions
- **Message broker** enables loose coupling between order creation and processing
- **Background consumers** handle long-running order operations
- **State machines** manage complex order workflows (using Stateless library)

## ✨ Key Features

### Order Management
- **Order Creation** with items and shipping details
- **Order Status Tracking** (Created, Processing, Completed, Cancelled, Error)
- **Order Retrieval** with detailed information
- **Order Status Updates** with event publishing

### Architecture Patterns
- **CQRS Pattern**: Separate read and write operations
- **Domain Events**: Event-driven order processing
- **Message Queuing**: Asynchronous order workflow handling
- **Clean Architecture**: Dependency inversion and separation of concerns
- **Comprehensive Testing**: Unit tests with high coverage
- **Secure Authentication**: JWT-based security
- **API Documentation**: Swagger/OpenAPI integration

## 🎨 Design Patterns

- **CQRS (Command Query Responsibility Segregation)**
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management
- **Domain Events** - Business event handling
- **Dependency Injection** - Loose coupling
- **State Machine** - Order workflow management

## 📦 Domain Models

### Core Entities
- **Order**: Main aggregate containing order details, status, and shipping information
- **OrderItem**: Individual items within an order with product details and pricing
- **Address**: Value object for shipping addresses

### Order Statuses
- `Created` - Initial order state
- `Processing` - Order being processed
- `Completed` - Order successfully fulfilled
- `Cancelled` - Order cancelled by user or system
- `Error` - Order encountered processing errors

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL database
- RabbitMQ message broker

### Configuration
1. Set up connection strings in `appsettings.json`
2. Configure JWT settings
3. Set up RabbitMQ connection
4. Configure OpenAI API key (if using AI features)

### Running the Application
```bash
# Start the API
dotnet run --project src/Api

# Start the background consumer
dotnet run --project src/Consumer
```

### Running Tests
```bash
dotnet test
```

## 📜 Development Guidelines

This project follows strict coding standards and architectural principles:

- **SOLID Principles** - Maintainable and extensible code
- **DRY (Don't Repeat Yourself)** - Eliminate code duplication
- **KISS (Keep It Simple, Stupid)** - Favor simplicity
- **Test-Driven Development** - Comprehensive unit testing
- **Consistent Naming** - PascalCase for public members, camelCase for parameters

For detailed development guidelines, see `.github/copilot-instructions.md`.

## 📚 API Documentation

API documentation is available via Swagger UI when running the application in development mode.

### Key Endpoints
- `POST /api/v1/orders` - Create new order
- `GET /api/v1/orders/{id}` - Get order details
- `PUT /api/v1/orders/{id}/status` - Update order status
- `GET /api/v1/orders` - List orders with filtering

---

*This application demonstrates modern .NET development practices for e-commerce order management with clean architecture, comprehensive testing, and enterprise-grade patterns.*