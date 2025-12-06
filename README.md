# 📝 Blog API

A production-ready ASP.NET Core 8 Web API for a blog platform, featuring posts, comments, likes, user authentication, and comprehensive security measures.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?logo=dotnet)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?logo=microsoftsqlserver)
![License](https://img.shields.io/badge/License-MIT-green)
[![Live Demo](https://img.shields.io/badge/🚀_Live_Demo-blog--restapi.runasp.net-brightgreen?style=for-the-badge)](http://blog-restapi.runasp.net)

---

## 📋 Table of Contents

- [✨ Features](#-features)
- [🏗️ Architecture](#️-architecture)
- [🧰 Technologies Used](#-technologies-used)
- [🚀 Getting Started](#-getting-started)
- [⚙️ Configuration](#️-configuration)
- [🌐 API Endpoints](#-api-endpoints)
- [🔐 Authentication](#-authentication)
- [📊 Health Checks](#-health-checks)
- [🧱 Data Models](#-data-models)
- [❗ Error Handling](#-error-handling)
- [📦 NuGet Packages](#-nuget-packages)
- [🪪 License](#-license)

---

## ✨ Features

### Core Features
- **User Authentication**: Secure JWT-based registration and login with role-based authorization (Admin/User)
- **Blog Post Management**: Full CRUD operations with filtering, sorting, and pagination
- **Category System**: Organize posts by categories
- **Tag System**: Many-to-many tagging for blog posts
- **Comment System**: Comment on posts with support for nested replies
- **Like Functionality**: Like both blog posts and comments (toggle like/unlike)
- **Image Support**: Attach multiple image URLs to blog posts with validation

### Production-Ready Features
- **Rate Limiting**: Configurable rate limits (General: 100 req/min, Auth: 10 req/min)
- **CORS Policy**: Configurable allowed origins with restrictive defaults
- **Health Checks**: Endpoints for liveness, readiness, and database connectivity
- **Response Compression**: Brotli & Gzip compression for optimized responses
- **Response Caching**: Built-in response caching middleware
- **Structured Logging**: Serilog with console and file sinks
- **RFC 7807 ProblemDetails**: Standardized error responses
- **Correlation IDs**: Request tracking via `X-Correlation-ID` header
- **XML Documentation**: Full API documentation in Swagger UI

### Developer Features
- **Swagger/OpenAPI**: Interactive API documentation with JWT auth support
- **Environment Configs**: Separate settings for Development, Staging, and Production
- **AutoMapper**: Automatic entity-to-DTO mapping
- **JSON Patch**: Partial updates for comments via PATCH endpoints

---

## 🏗️ Architecture

```
Blog API/
├── Controllers/           # API endpoint controllers
├── Configurations/        # Service registration & configuration
├── Data/                  # DbContext and database setup
├── Mappings/              # AutoMapper profiles
├── Middlewares/           # Custom middleware (exception handling)
├── Models/
│   ├── DTOs/              # Data Transfer Objects
│   └── Entities/          # Entity Framework entities
├── Repositories/          # Data access layer (Repository pattern)
├── Services/              # Business logic layer
└── Utilities/             # Helper classes (JWT generator, validators)
```

**Design Patterns:**
- Repository Pattern for data access abstraction
- Dependency Injection throughout
- Clean separation: Controllers → Services → Repositories

---

## 🧰 Technologies Used

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 8.0 | Runtime framework |
| ASP.NET Core | 8.0 | Web API framework |
| Entity Framework Core | 8.0 | ORM for database access |
| SQL Server | 2019+ | Database |
| ASP.NET Core Identity | 8.0 | Authentication & authorization |
| JWT Bearer | 8.0 | Token-based authentication |
| Serilog | 8.0 | Structured logging |
| Swashbuckle | 6.5+ | Swagger/OpenAPI documentation |
| AutoMapper | 13.0 | Object-to-object mapping |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server Express
- (Optional) [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/HoBaaMa/Blog-API.git
   cd Blog-API
   ```

2. **Set environment variables:**
   ```bash
   # Windows PowerShell
   $env:JWT_SECRET_KEY = "YourSecureSecretKeyAtLeast32CharactersLong!"
   
   # Linux/macOS
   export JWT_SECRET_KEY="YourSecureSecretKeyAtLeast32CharactersLong!"
   ```

3. **Configure the database connection:**
   Update `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Data Source=.;Initial Catalog=BlogDb;Integrated Security=True;TrustServerCertificate=True"
   }
   ```

4. **Apply database migrations:**
   ```bash
   cd "Blog API"
   dotnet ef database update
   ```

5. **Run the application:**
   ```bash
   dotnet run
   ```

6. **Access Swagger UI:**
   Navigate to `https://localhost:7xxx/swagger` in your browser.

---

## ⚙️ Configuration

### Environment Variables

| Variable | Required | Description |
|----------|----------|-------------|
| `JWT_SECRET_KEY` | ✅ Yes | Secret key for JWT signing (min 32 chars) |
| `ASPNETCORE_ENVIRONMENT` | No | `Development`, `Staging`, or `Production` |

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-connection-string"
  },
  "Jwt": {
    "Key": "",  // Use JWT_SECRET_KEY env var instead
    "Issuer": "https://localhost:44354",
    "Audience": "https://localhost:44354"
  },
  "Cors": {
    "AllowedOrigins": ["https://localhost:3000", "https://localhost:5173"]
  },
  "RateLimiting": {
    "GeneralPermitLimit": 100,
    "GeneralWindowMinutes": 1,
    "AuthPermitLimit": 10,
    "AuthWindowMinutes": 1
  }
}
```

### Environment-Specific Settings

| Environment | Rate Limits | Log Level |
|-------------|-------------|-----------|
| Development | 100/min general, 10/min auth | Debug |
| Staging | 80/min general, 8/min auth | Information |
| Production | 60/min general, 5/min auth | Warning |

---

## 🌐 API Endpoints

### Authentication (`/api/auth`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/register` | Register a new user | ❌ |
| POST | `/api/auth/login` | Login and get JWT token | ❌ |
| POST | `/api/auth/logout` | Logout current user | ✅ |

### Blog Posts (`/api/blogposts`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/blogposts` | Get all posts (paginated) | ❌ |
| GET | `/api/blogposts/{id}` | Get post by ID | ❌ |
| GET | `/api/blogposts/blogCategory` | Get posts by category | ❌ |
| GET | `/api/blogposts/{id}/images` | Get post images | ❌ |
| POST | `/api/blogposts` | Create new post | ✅ Admin |
| PUT | `/api/blogposts/{id}` | Update post | ✅ Admin |
| DELETE | `/api/blogposts/{id}` | Delete post | ✅ Admin |

**Query Parameters for GET `/api/blogposts`:**
- `pageNumber` (default: 1)
- `pageSize` (default: 10)
- `filterOn` (e.g., "Title")
- `filterQuery` (search term)
- `sortBy` (e.g., "CreatedAt", "LikeCount")
- `isAscending` (true/false)

### Comments (`/api/comments`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/comments/blogpost/{id}` | Get comments for post | ❌ |
| GET | `/api/comments/{id}` | Get comment by ID | ❌ |
| POST | `/api/comments` | Create comment | ✅ |
| PATCH | `/api/comments/{id}` | Partial update (JSON Patch) | ✅ |
| DELETE | `/api/comments/{id}` | Delete comment | ✅ |

### Likes (`/api/likes`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/likes` | Toggle like on post/comment | ✅ |
| GET | `/api/likes/blogpost?blogPostId={id}` | Get likes for post | ❌ |
| GET | `/api/likes/comment?commentId={id}` | Get likes for comment | ❌ |

### Blog Categories (`/api/blogcategory`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/blogcategory` | Get all categories | ❌ |
| POST | `/api/blogcategory` | Create category | ✅ Admin |
| DELETE | `/api/blogcategory/{id}` | Delete category | ✅ Admin |

---

## 🔐 Authentication

This API uses **JWT Bearer Authentication**.

### Getting a Token

1. Register: `POST /api/auth/register`
   ```json
   {
     "userName": "john_doe",
     "email": "john@example.com",
     "password": "SecurePass123!"
   }
   ```

2. Login: `POST /api/auth/login`
   ```json
   {
     "userName": "john_doe",
     "password": "SecurePass123!"
   }
   ```
   
   Response:
   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
     "message": "Login successful",
     "userName": "john_doe"
   }
   ```

### Using the Token

Include the token in the `Authorization` header:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Roles
- **User**: Can create comments, toggle likes
- **Admin**: Full access to all endpoints including blog post management

---

## 📊 Health Checks

| Endpoint | Description |
|----------|-------------|
| `/health` | Overall application health |
| `/health/ready` | Database readiness check |
| `/health/live` | API liveness probe |

Response format:
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0234567",
  "entries": {
    "database": { "status": "Healthy" },
    "self": { "status": "Healthy" }
  }
}
```

---

## 🧱 Data Models

### BlogPost
```csharp
public class BlogPost
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public BlogCategory BlogCategory { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Like> Likes { get; set; }
    public ICollection<Tag> Tags { get; set; }
    public ICollection<string> ImageUrls { get; set; }
}
```

### Comment
```csharp
public class Comment
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid BlogPostId { get; set; }
    public string UserId { get; set; }
    public Guid? ParentCommentId { get; set; }  // For nested replies
    public ICollection<Comment> Replies { get; set; }
    public ICollection<Like> Likes { get; set; }
}
```

---

## ❗ Error Handling

All errors return **RFC 7807 ProblemDetails** format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Resource Not Found",
  "status": 404,
  "detail": "Blog post with ID '...' was not found.",
  "instance": "/api/blogposts/...",
  "correlationId": "abc123-def456",
  "traceId": "00-1234567890abcdef-..."
}
```

### HTTP Status Codes

| Code | Meaning | Common Causes |
|------|---------|---------------|
| 200 | OK | Successful request |
| 201 | Created | Resource created successfully |
| 204 | No Content | Successful deletion |
| 400 | Bad Request | Invalid input or validation failure |
| 401 | Unauthorized | Missing or invalid JWT token |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Resource doesn't exist |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | Unexpected server error |

---

## 📦 NuGet Packages

| Package | Purpose |
|---------|---------|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT authentication |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | User identity management |
| `Microsoft.AspNetCore.JsonPatch` | JSON Patch support |
| `Microsoft.AspNetCore.Mvc.NewtonsoftJson` | JSON serialization |
| `Microsoft.EntityFrameworkCore` | Entity Framework Core |
| `Microsoft.EntityFrameworkCore.SqlServer` | SQL Server provider |
| `Microsoft.EntityFrameworkCore.Tools` | EF Core CLI tools |
| `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` | EF health checks |
| `AspNetCore.HealthChecks.UI.Client` | Health check UI responses |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | AutoMapper DI |
| `Serilog.AspNetCore` | Structured logging |
| `Serilog.Settings.Configuration` | Serilog configuration |
| `Serilog.Sinks.File` | File logging |
| `Swashbuckle.AspNetCore` | Swagger/OpenAPI |
| `System.IdentityModel.Tokens.Jwt` | JWT token handling |

---

## 🪪 License

This project is licensed under the [MIT License](LICENSE).

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📧 Contact

For questions or support, please open an issue on GitHub.
