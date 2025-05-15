# MyLibraryApp

**MyLibraryApp** is a RESTful API built with ASP.NET Core 8 for managing books and categories, featuring JWT-based authentication, role-based authorization (`User` and `Admin`), and personal user libraries.

## Features

### Authentication & Authorization
- User registration and login (JWT token issued)
- Role-based access: `User` and `Admin`
- Admins can manage all entities
- Users can manage their personal library

### Categories (`CategoryController`)
- `GET /api/category` — Get all categories (AllowAnonymous)
- `GET /api/category/{id}` — Get a category by ID (Admin only)
- `POST /api/category` — Create a category (Admin only)
- `PUT /api/category/{id}` — Update a category (Admin only)
- `DELETE /api/category/{id}` — Delete a category (Admin only)

### Books (`BookController`)
- `GET /api/book` — Get all books (AllowAnonymous)
- `GET /api/book/{id}` — Get a book by ID (AllowAnonymous)
- `POST /api/book` — Add a new book (Admin only)
- `PUT /api/book/{id}` — Update a book (Admin only)
- `DELETE /api/book/{id}` — Delete a book (Admin only)

### User Library (`UserController`)
- `POST /api/user/books/{bookId}` — Add a book to the user’s library (User only)
- `DELETE /api/user/books/{bookId}` — Remove a book from the library (User only)
- `GET /api/user/books` — Get all books in the user’s library (User only)

### Authentication (`AuthController`)
- `POST /api/auth/register` — Register a new user
- `POST /api/auth/register-admin` — Register a new admin (Admin only)
- `POST /api/auth/login` — Login and receive a JWT token

## Tech Stack

- ASP.NET Core 8
- Entity Framework Core
- JWT Authentication
- ASP.NET Identity
- xUnit & FluentAssertions for testing
- InMemory database for integration tests

## Getting Started

1. Clone the repository:
   git clone https://github.com/kazanzhi/MyLibraryApp.git
   cd MyLibraryApp
   
3. Update the connection string in appsettings.json:
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyLibraryAppDb;Trusted_Connection=True;"
}

4. Apply EF Core migrations:
dotnet ef database update

5. Run the application:
dotnet run

## Running Tests
Unit Tests
cd MyLibraryApp.Tests
dotnet test

## Project Structure

MyLibraryApp/                        # ASP.NET Core Web API
   Properties/
   Controllers/
   Data/
   Dtos/
   Interfaces/
   Models/
   Repositories/
   Services/
   Program.cs
MyLibraryApp.Tests/               # Unit tests
  
## Notes
For testing purposes, the X-Test-Role header can be added to simulate roles (User, Admin).
