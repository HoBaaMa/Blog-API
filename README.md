# 📝 Blog API

A comprehensive ASP.NET Core Web API for a blog platform, featuring posts, comments, likes, and user authentication.

-----

## 📋 Table of Contents

  - [✨ Features](https://www.google.com/search?q=%23-features)
  - [🧰 Technologies Used](https://www.google.com/search?q=%23-technologies-used)
  - [🚀 Getting Started](https://www.google.com/search?q=%23-getting-started)
      - [🔧 Prerequisites](https://www.google.com/search?q=%23-prerequisites)
      - [⚙️ Setup](https://www.google.com/search?q=%23setup)
  - [🌐 API Endpoints](https://www.google.com/search?q=%23-api-endpoints)
  - [🧱 Models](https://www.google.com/search?q=%23-models)
  - [❗ Error Handling](https://www.google.com/search?q=%23-error-handling)
  - [📦 Required NuGet Packages](https://www.google.com/search?q=%23-required-nuget-packages)
  - [🪪 License](https://www.google.com/search?q=%23-license)

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

-----

## 🧱 Models

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
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Like> Likes { get; set; }
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
    public BlogPost BlogPost { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public ICollection<Like> Likes { get; set; }
    public Guid? ParentCommentId { get; set; }
    public Comment ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; }
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

This project is licensed under the [MIT License](https://www.google.com/search?q=LICENSE.txt).