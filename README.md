# BookKeepAPI

A .NET-based RESTful API for managing a book collection. This API provides functionality to store, retrieve, and manage book information, including integration with the OpenLibrary API for book cover images. This API is designed to work with [BookKeepWeb](https://github.com/imbavirus/BookKeepWeb), a modern web interface for managing your book collection.

## 🌐 Demo

A live demo of the API with Swagger documentation is available at:
🔗 **[Live Demo](https://bookkeep-api.home.infernos.co.za/swagger)**

## 🚀 Features

- CRUD operations for books
- Book cover image integration with OpenLibrary API
- SQLite database with Entity Framework Core
- Swagger/OpenAPI documentation
- Global exception handling
- CORS support
- Automated database migrations
- Comprehensive testing suite

## 🛠️ Tech Stack

- .NET 9.0
- Entity Framework Core 9.0.5
- SQLite
- FluentValidation
- xUnit for testing
- Swagger/OpenAPI
- Moq for mocking in tests

## 📋 Prerequisites

- .NET 9.0 SDK or later
- PowerShell 7.0 or later
- Git

## 🔧 Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/BookKeepAPI.git
cd BookKeepAPI
```

2. Set up the database by running Entity Framework migrations:
```bash
dotnet ef database update --project ./Application --startup-project ./API
```

For convenience, you can also use the provided PowerShell script:
```powershell
./setup-database.ps1
```

## 🏃‍♂️ Running the Application

Run the API using the .NET CLI:
```bash
dotnet run --project ./API
```

Alternatively, use the convenience script:
```powershell
./run.ps1
```

The API will be available at:
- HTTP: http://localhost:5001
- Swagger UI: http://localhost:5001/swagger

## 🧪 Running Tests

Run tests using the .NET CLI:
```bash
dotnet test
```

Or use the convenience script:
```powershell
./run-tests.ps1
```

## 📁 Project Structure

- `Api/` - API controllers and middleware
- `Application/` - Core business logic, models, and data access
- `Tests/` - Unit and integration tests
- `*.ps1` - PowerShell scripts for common operations

## 🔄 Database Migrations

Create a new migration using Entity Framework CLI:
```bash
dotnet ef migrations add "YourMigrationName" --project ./Application --startup-project ./API
dotnet ef database update --project ./Application --startup-project ./API
```

For convenience, you can use the provided PowerShell script which combines these steps:
```powershell
./run-migrations.ps1 -MigrationName "YourMigrationName"
```

## 📝 API Documentation

Once the application is running, visit http://localhost:5001/swagger to view the complete API documentation.

## 🔒 CORS Configuration

CORS is configured to allow requests from:
- http://localhost:5173
- http://127.0.0.1:5173
- https://bookkeep-web.home.infernos.co.za

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License

This project is licensed under the terms included in the [LICENSE](LICENSE) file.

## 🏗️ Project Architecture

The project follows a clean architecture pattern with clear separation of concerns:

- **API Layer** (`Api/`)
  - Controllers
  - Middleware
  - Services
  - API-specific configuration

- **Application Layer** (`Application/`)
  - Business Logic
  - Data Access
  - Models
  - Validation
  - External Service Integration

- **Test Layer** (`Tests/`)
  - Unit Tests
  - Integration Tests
  - Test Utilities

## 🛡️ Error Handling

The API implements global exception handling through middleware that provides consistent error responses across all endpoints.

## 📚 Models

All models inherit from `BaseModel` which includes:
- Unique identifier (Id)
- GUID
- Creation timestamp
- Last update timestamp
- Active status flag

### Book Model

The main entity in the system is the Book model, which includes:

Required fields:
- Title (max 200 characters)
- Author (max 100 characters)
- ISBN (10-13 characters)

Optional fields:
- Description (max 2000 characters)
- Publication Year (between 1000 and current year + 5)
- Genre (max 50 characters)
- Cover Image URL (max 500 characters)

Features:
- Automatic cover image fetching from OpenLibrary API using ISBN
- Validation for all fields including ISBN format
- Soft delete functionality
- Duplicate ISBN prevention
- Audit trail (creation and update timestamps) 