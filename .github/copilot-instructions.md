# Copilot Instructions

This document outlines the architectural principles, coding standards, and conventions to be followed in this solution. As an AI assistant, you must adhere to these guidelines when generating or modifying code to ensure consistency, maintainability, and quality.

## Architectural & Design Principles

The solution is built upon the **Onion Architecture**. All code generated must conform to this structure and its principles.

### 1. Onion Architecture

The core principle is that **dependencies flow inwards**. Outer layers can depend on inner layers, but inner layers **must not** depend on outer layers.

* **Domain Layer (Core):**
    * Contains enterprise logic, entities, and value objects.
    * This is the center of the architecture and has **zero dependencies** on any other layer.
    * It defines interfaces for repositories that the Infrastructure layer will implement.
* **Application Layer:**
    * Contains application-specific logic (use cases).
    * Orchestrates the domain objects to perform tasks.
    * Depends on the Domain layer but not on the Infrastructure or Presentation layers.
    * Defines interfaces for services that are implemented in the outer layers (e.g., `IEmailService`, `IDateTimeProvider`).
* **Infrastructure Layer:**
    * Implements interfaces defined in the inner layers.
    * Handles external concerns like database access (e.g., Entity Framework Core), file system access, and interactions with third-party APIs.
    * Depends on the Application and Domain layers.
* **Presentation Layer (API/UI):**
    * The outermost layer.
    * Handles user interaction, API endpoints, and the user interface.
    * Depends on the Application layer to execute commands and queries.

### 2. SOLID Principles

Adhere strictly to the SOLID principles in all code.

* **(S) Single Responsibility Principle:** A class should have only one reason to change. Each class should have a single, well-defined purpose.
* **(O) Open/Closed Principle:** Software entities should be open for extension but closed for modification. Use interfaces and abstraction to allow new functionality without changing existing code.
* **(L) Liskov Substitution Principle:** Subtypes must be substitutable for their base types without altering the correctness of the program.
* **(I) Interface Segregation Principle:** Clients should not be forced to depend on interfaces they do not use. Prefer smaller, more specific interfaces over large, monolithic ones.
* **(D) Dependency Inversion Principle:** High-level modules should not depend on low-level modules; both should depend on abstractions. This is the foundation of the Onion Architecture.

### 3. DRY (Don't Repeat Yourself)

* Every piece of knowledge must have a single, unambiguous, authoritative representation within a system.
* Avoid duplicating code. Encapsulate common logic in reusable methods, services, or components.

### 4. KISS (Keep It Simple, Stupid)

* Favor simplicity over complexity.
* Write the most straightforward code that solves the problem. Avoid unnecessary abstractions or over-engineering.

## Coding Standards

### General Principles

* **Target Framework:** All projects target **.NET 8.0**. Ensure compatibility with this version.
* **Language:** Use **C#** for all code unless otherwise specified.
* **Readability:** Write clear, self-explanatory code. Use meaningful variable, method, and class names.
* **Consistency:** Follow consistent naming conventions and code formatting throughout the solution.
* **Comments:** Add inline comments only where necessary to clarify complex or non-obvious logic.
* **Error Handling:** Use structured exception handling (`try-catch`). Avoid swallowing exceptions; log or rethrow as appropriate.
* **Dependency Injection:** Prefer **constructor injection** for all dependencies.
* **Async/Await:** Use asynchronous programming patterns (`async/await`) where appropriate, especially for I/O-bound operations.
* **Magic Numbers:** Avoid magic numbers; use named constants or enums.
* **File Organization:** One class per file. Organize files into appropriate folders by feature or layer.

### Naming Conventions

* **Classes & Interfaces:** `PascalCase` (e.g., `UserService`, `IUserRepository`)
* **Methods & Properties:** `PascalCase` (e.g., `GetUserById`, `IsActive`)
* **Variables & Parameters:** `camelCase` (e.g., `userId`, `cancellationToken`)
* **Constants:** `PascalCase` (e.g., `DefaultTimeoutInSeconds`)
* **Unit Test Methods:** Use descriptive names indicating the scenario and expected outcome (e.g., `GetUserById_ShouldReturnUser_WhenUserExists`)

### Code Style

* **Braces:** Use **Allman style** (braces on new lines).
* **Indentation:** Use **4 spaces** per indentation level.
* **Line Length:** Limit lines to **120 characters**.
* **Usings:** Place `using` statements outside the namespace and remove unused usings.
* **File-Scoped Namespaces:** Use file-scoped namespaces to reduce nesting (e.g., `namespace MySolution.Services;`).

## Copilot Usage

* **Adhere to all standards** outlined in this document when generating or modifying code.
* **Prioritize the architectural principles** (Onion, SOLID, DRY, KISS) as the foundation for all code structure.
* **Prefer existing patterns** and conventions found in the current solution.
* **Generate code that is ready to use** and fits seamlessly into the existing structure.
* **Document any necessary deviations** from these standards in pull requests or code reviews.