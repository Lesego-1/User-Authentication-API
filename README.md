# User Authentication & Role-Based Access API

A **secure ASP.NET Core 9 Web API** demonstrating JWT authentication, role-based authorization, and production-ready user management. This project showcases modern backend development skills highly sought by employers, including secure password hashing, token management, and role-based access control.

---

## Features

- **User Registration & Login** with password hashing using `PasswordHasher<T>`  
- **JWT Authentication** with custom claims for secure API access  
- **Role-Based Authorization**  
  - Admin: Full access to user management endpoints  
  - User: Limited access (profile & refresh token)  
- **Refresh Tokens** for maintaining session security  
- **SQL Server + Entity Framework Core** for persistent user data  
- **Validation** on all endpoints for robust input handling  
- **Logging** of key events for debugging and monitoring  

---

## Endpoints

| Endpoint | Method | Access | Description |
|----------|--------|--------|-------------|
| `/auth/register` | POST | Public | Register a new user |
| `/auth/login` | POST | Public | Login and receive JWT + refresh token |
| `/auth/profile` | GET | Authenticated Users | Get your own profile information |
| `/auth/users` | GET | Admin Only | Retrieve a list of all users |
| `/auth/refresh` | POST | Authenticated Users | Refresh JWT using a valid refresh token |

---

## Technologies Used

- **ASP.NET Core 9**  
- **Entity Framework Core 9**  
- **SQL Server / SQL Server Express**  
- **JWT (JSON Web Tokens)**  
- **C# 12**  
- **Visual Studio Code**  

---

## Setup & Run

1. **Clone the repository**:  

```bash
git clone https://github.com/Lesego-1/User-Authentication-API.git
cd AuthApi
````

2. **Configure `appsettings.json`**:

```json
{
  "JwtSettings": {
    "SecretKey": "a-very-long-secret-key-at-least-32-characters",
    "Issuer": "YourApp",
    "Audience": "YourAppUsers",
    "ExpiryMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=AuthApiDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. **Run database migrations**:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. **Run the API**:

```bash
dotnet run
```

5. **Test endpoints** using Postman or any API client.

---

## Usage Tips

* Ensure **SecretKey** is at least 32 characters to meet JWT security requirements.
* Use **Bearer Token** in the Authorization header for protected endpoints.
* Admin access requires a user with `Role = "Admin"`.
