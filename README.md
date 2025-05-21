# BookKeep API

A .NET-based RESTful API for managing a book collection. This API provides functionality to store, retrieve, and manage book information, including integration with the OpenLibrary API for book cover images. This API is designed to work with [BookKeepWeb](https://github.com/imbavirus/BookKeepWeb), a modern web interface for managing your book collection.

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

## 🌐 Demo

Try out BookKeep API without installing! Visit our [live demo](https://bookkeep-api.home.infernos.co.za/swagger) to explore all features and see the application in action.

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

## 🚀 Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/BookKeepAPI.git
cd BookKeepAPI
```

2. Set up the database by running Entity Framework migrations:
```bash
dotnet ef database update --project ./Application --startup-project ./API
```

For convenience, you can also use the provided PowerShell 7 script:
```powershell
./setup-database.ps1
```

## 🏃‍♂️ Running the Application

Run the API using the .NET CLI:
```bash
dotnet run --project ./API
```

For convenience, you can also use the provided PowerShell 7 script:
```powershell
./run.ps1
```

The API will be available at:
- HTTP: http://localhost:5001
- Swagger UI: http://localhost:5001/swagger

## 🏭 Building for Production

1. Build the application:
```bash
dotnet publish ./API -c Release -o ./publish
```

2. Configure production settings in `appsettings.json`:
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://your-production-frontend-domain.com"
    ]
  },
  "Endpoints": [
    "http://0.0.0.0:5001"
  ]
}
```

3. Set up your database:
```bash
cd ./publish
dotnet BookKeepAPI.API.dll --apply-migrations
```

4. Run in production:
```bash
dotnet BookKeepAPI.API.dll
```

For containerized deployment, you can use Docker:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY ./publish .
EXPOSE 5001
ENTRYPOINT ["dotnet", "BookKeepAPI.API.dll"]
```

Build and run the container:
```bash
docker build -t bookkeep-api .
docker run -p 5001:5001 bookkeep-api
```

## 🧪 Running Tests

Run tests using the .NET CLI:
```bash
dotnet test
```

For convenience, you can also use the provided PowerShell 7 script:
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

For convenience, you can also use the provided PowerShell 7 script which combines these steps:
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

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the GNU General Public License v3.0 (GPL-3.0) - see the [LICENSE](LICENSE) file for details.

This means you are free to:
- Use this software for any purpose
- Change the software to suit your needs
- Share the software with your friends and neighbors
- Share the changes you make

Under the following conditions:
- If you distribute this software, you must include the source code
- Any modifications must also be under the GPL-3.0 license
- You must state significant changes made to the software
- Include the original license and copyright notices

For more information about the GPL-3.0 license, visit [GNU GPL v3](https://www.gnu.org/licenses/gpl-3.0.en.html).

## 🙏 Acknowledgements

- [OpenLibrary API](https://openlibrary.org/developers/api) - For providing book cover images and metadata
- [.NET](https://dotnet.microsoft.com/) - The foundation of our application
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - Our robust ORM
- [SQLite](https://www.sqlite.org/) - Our database engine
- [Swagger/OpenAPI](https://swagger.io/) - For API documentation
- [FluentValidation](https://fluentvalidation.net/) - For robust model validation
- [xUnit](https://xunit.net/) - For our testing framework
- [Moq](https://github.com/moq/moq4) - For mocking in tests
- [BookKeepWeb](https://github.com/imbavirus/BookKeepWeb) - The frontend application for this API
