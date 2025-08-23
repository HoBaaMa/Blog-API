# 📝 Blog API

A comprehensive ASP.NET Core Web API for a blog platform, featuring posts, comments, likes, and user authentication.

-----

## 📋 Table of Contents

- [✨ Features](#-features)
- [🧰 Technologies Used](#-technologies-used)
- [🚀 Getting Started](#-getting-started)
  - [🔧 Prerequisites](#-prerequisites)
  - [⚙️ Setup](#setup)
- [🌐 API Endpoints](#-api-endpoints)
- [🧱 Models](#-models)
- [❗ Error Handling](#-error-handling)
- [📦 Required NuGet Packages](#-required-nuget-packages)
- [🪪 License](#-license)
-----

## ✨ Features

  - **User Authentication**: Secure registration and login functionality for users.
  - **Blog Post Management**: Full CRUD (Create, Read, Update, Delete) operations for blog posts.
  - **Comment System**: Users can comment on blog posts, with support for nested replies.
  - **Like Functionality**: Users can like both blog posts and comments.
  - **Partial Updates**: Supports JSON Patch for efficiently updating comments.
  - **Structured Logging**: Utilizes Serilog for detailed and structured application logging.

-----

## 🧰 Technologies Used

- **ASP.NET Core 8.0**: Modern web framework for building APIs
- **Entity Framework Core**: Object-relational mapping (ORM) for data access
- **SQL Server**: Database management system
- **ASP.NET Core Identity**: Authentication and authorization framework
- **AutoMapper**: Object-to-object mapping library
- **Serilog**: Structured logging library
- **Swagger/OpenAPI**: API documentation and testing interface
- **Newtonsoft.Json**: JSON serialization framework
- **JSON Patch**: Support for partial resource updates

-----

## 🚀 Getting Started

### 🔧 Prerequisites

  - [.NET 8 SDK](https://dotnet.microsoft.com/download)
  - [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### ⚙️ Setup

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/HoBaaMa/Blog-API.git
    ```
2.  **Configure the database connection:**
    Update the `DefaultConnection` string in `appsettings.json` with your SQL Server details.
3.  **Apply migrations:**
    ```bash
    dotnet ef database update
    ```
4.  **Run the application:**
    ```bash
    dotnet run
    ```
5.  **Access Swagger UI:**
    Navigate to `https://localhost:{port}/swagger` in your browser.

-----

## 🌐 API Endpoints

### Accounts

| 🔠 Method | 🌐 Endpoint          | 📝 Description         |
| :--- | :--- | :--- |
| ➕ POST  | `/api/accounts/register` | Register a new user    |
| ➡️ POST  | `/api/accounts/login`    | Log in a user          |
| ⬅️ POST  | `/api/accounts/logout`   | Log out a user         |

### Blog Posts

| 🔠 Method | 🌐 Endpoint            | 📝 Description               |
| :--- | :--- | :--- |
| 🟢 GET   | `/api/blogposts`       | Get all blog posts         |
| 🔍 GET   | `/api/blogposts/{id}`  | Get a blog post by ID      |
| ➕ POST  | `/api/blogposts`       | Create a new blog post     |
| ♻️ PUT   | `/api/blogposts/{id}`  | Update an existing blog post |
| ❌ DELETE| `/api/blogposts/{id}`  | Delete a blog post         |

### Comments

| 🔠 Method | 🌐 Endpoint                  | 📝 Description                        |
| :--- | :--- | :--- |
| 🟢 GET   | `/api/comments/blogpost/{id}`| Get all comments for a blog post    |
| 🔍 GET   | `/api/comments/{id}`         | Get a comment by ID                 |
| ➕ POST  | `/api/comments`              | Create a new comment                |
| 🩹 PATCH | `/api/comments/{id}`         | Partially update a comment          |
| ❌ DELETE| `/api/comments/{id}`         | Delete a comment                    |

### Likes

| 🔠 Method | 🌐 Endpoint                    | 📝 Description                        |
| :--- | :--- | :--- |
| ➕ POST  | `/api/likes`                   | Toggle like/unlike for post or comment |
| 🟢 GET   | `/api/likes/blogpost?blogPostId={id}` | Get all likes for a blog post |
| 🟢 GET   | `/api/likes/comment?commentId={id}`   | Get all likes for a comment   |

-----

## 🧱 Models

### BlogPost

```csharp
public class BlogPost
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Like> Likes { get; set; }
    public BlogCategory BlogCategory { get; set; }
    public ICollection<Tag> Tags { get; set; }
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
    public BlogPost? BlogPost { get; set; }
    public string UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public ICollection<Like> Likes { get; set; }
    public Guid? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; }
}
```

### Like

```csharp
public class Like
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public Guid? BlogPostId { get; set; }
    public BlogPost? BlogPost { get; set; }
    public Guid? CommentId { get; set; }
    public Comment? Comment { get; set; }
}
```

### ApplicationUser

```csharp
public class ApplicationUser : IdentityUser
{
    public ICollection<BlogPost> BlogPosts { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Like> Likes { get; set; }
}
```

### Tag

```csharp
public class Tag
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ICollection<BlogPost>? BlogPosts { get; set; }
}
```

### BlogCategory

```csharp
public enum BlogCategory
{
    Technology,
    Education,
    Business,
    Lifestyle,
    Health,
    Travel,
    Food,
    Entertainment,
    Science,
    Sports,
    Finance,
    News,
    Opinion
}
```

-----

## ❗ Error Handling

  - Returns `400 Bad Request` for invalid input.
  - Returns `401 Unauthorized` for failed login attempts.
  - Returns `403 Forbidden` for unauthorized actions.
  - Returns `404 Not Found` if a resource does not exist.
  - Returns `500 Internal Server Error` for unexpected server-side errors.

-----

## 📦 Required NuGet Packages

| 📦 Package Name                                  | 📝 Description                                |
| :--- | :--- |
| 🗺️ AutoMapper.Extensions.Microsoft.DependencyInjection | Object-to-object mapping with DI integration    |
| 📂 Microsoft.AspNetCore.Identity.EntityFrameworkCore | ASP.NET Core Identity provider for EF Core      |
| 🩹 Microsoft.AspNetCore.JsonPatch                | JSON Patch support                            |
| 🧩 Microsoft.AspNetCore.Mvc.NewtonsoftJson       | Newtonsoft.Json support                       |
| 📂 Microsoft.EntityFrameworkCore                 | Entity Framework Core ORM                     |
| 🛢️ Microsoft.EntityFrameworkCore.SqlServer       | SQL Server provider for EF Core               |
| 🛠️ Microsoft.EntityFrameworkCore.Tools           | EF Core CLI tools                             |
| 🧾 Serilog.AspNetCore                            | Serilog logging                               |
| ⚙️ Serilog.Settings.Configuration                | Serilog configuration from `appsettings.json` |
| 📁 Serilog.Sinks.File                            | Serilog file sink                             |
| 📖 Swashbuckle.AspNetCore                        | Swagger / OpenAPI support                     |

-----

## 🪪 License

This project is licensed under the [MIT License](LICENSE.txt).
