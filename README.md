# Auth API – JWT Authentication with Refresh Tokens and Role-Based Authorization

A secure backend API built with **.NET 9 / ASP.NET Core**, providing user authentication, JWT-based authorization, refresh tokens, and role-based access control. Fully testable via **Swagger UI**.

---

## Features

* **User registration** with hashed passwords.
* **JWT authentication** for secure access.
* **Refresh token support** for long-term sessions.
* **Role-based authorization** (Admin and User roles).
* **Swagger/OpenAPI documentation** for interactive endpoint testing.
* **CORS enabled** for external tools or frontend integration.

---

## Technologies Used

* **.NET 9 / ASP.NET Core**
* **Entity Framework Core** (SQL Server)
* **JWT Authentication**
* **Swagger / Swashbuckle**
* **C#**
* **SQL Server**

---

## Getting Started

### Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [SQL Server](https://www.microsoft.com/en-us/sql-server)
* Optional: [Postman](https://www.postman.com/) for API testing

---

### 1️⃣ Clone the Repository

```bash
git clone <your-repo-url>
cd AuthApi
```

---

### 2️⃣ Configure the Database

Edit `appsettings.json` with your SQL Server connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=AuthApiDb;Trusted_Connection=True;"
},
"JwtSettings": {
  "Issuer": "AuthApi",
  "Audience": "AuthApiClients",
  "SecretKey": "YourSuperSecretKey123!"
}
```

---

### 3️⃣ Run Database Migrations

```bash
dotnet ef database update
```

This creates the necessary database and tables.

---

### 4️⃣ Seed Admin User

The project automatically seeds an admin user if none exists:

| Username | Email                                         | Password          | Role  |
| -------- | --------------------------------------------- | ----------------- | ----- |
| admin    | [admin@example.com](mailto:admin@example.com) | AdminPassword123! | Admin |

Modify `DataSeeder.cs` if you want to change these credentials.

---

### 5️⃣ Run the API

```bash
dotnet run
```

The API runs at:

```
http://localhost:5235
```

---

### 6️⃣ Test with Swagger

Swagger UI is available at:

```
http://localhost:5235/swagger/index.html
```

You can test all endpoints interactively.

#### **Register a new user**

* POST `/auth/register`
* Payload:

```json
{
  "username": "testuser",
  "email": "test@example.com",
  "password": "TestPassword123!"
}
```

* Response:

```json
{
  "username": "testuser",
  "email": "test@example.com",
  "role": "User"
}
```

---

#### **Login**

* POST `/auth/login`
* Payload:

```json
{
  "username": "admin",
  "password": "AdminPassword123!"
}
```

* Response:

```json
{
  "username": "admin",
  "email": "admin@example.com",
  "role": "Admin",
  "token": "<JWT_TOKEN>",
  "refreshToken": "<REFRESH_TOKEN>"
}
```

---

#### **Authorize in Swagger**

1. Click the **Authorize** button.
2. Enter your JWT token:

```
Bearer <JWT_TOKEN>
```

3. You can now access protected endpoints like:

* `GET /auth/users` (Admin-only)
* `GET /auth/profile` (User)

---

#### **Refresh Token**

* POST `/auth/refresh`
* Payload:

```json
{
  "refreshToken": "<REFRESH_TOKEN>"
}
```

* Response includes a new JWT and refresh token.

---

### 7️⃣ Example Screenshots

**Swagger Landing Page:**

![Swagger UI Landing](./screenshots/swagger_landing.png)

**Testing Login Endpoint:**

![Login Endpoint](./screenshots/swagger_login.png)

**Testing Protected Endpoint (Users list):**

![Users Endpoint](./screenshots/swagger_users.png)

---

### 8️⃣ Project Structure

```
AuthApi/
├─ Controllers/       # AuthController.cs
├─ Data/              # AppDbContext.cs, DataSeeder.cs
├─ DTOs/              # LoginRequest.cs, RegisterRequest.cs, RefreshRequest.cs
├─ Models/            # User.cs, RefreshToken.cs
├─ Services/          # IUserService.cs, UserService.cs, ITokenService.cs, TokenService.cs
├─ Configuration/     # JwtSettings.cs
├─ Program.cs         # Entry point with JWT, Swagger, DB, DI
├─ appsettings.json
```

---

### 9️⃣ Security Notes

* Passwords are securely **hashed** using `PasswordHasher`.
* JWTs are validated using **Issuer**, **Audience**, and **SecretKey**.
* Refresh tokens are **one-time use** and stored in the database.
* Admin-only endpoints require **Role = "Admin"**.
