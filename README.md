# E-Commerce Microservices Application

A comprehensive microservices-based e-commerce platform built with .NET 8, featuring authentication, product management, order processing, and API gateway integration.

## 🏗️ Architecture Overview

This application follows a microservices architecture pattern with the following services:

- **Authentication API** - User registration, login, and JWT token management
- **Product API** - Product catalog management (CRUD operations)
- **Order API** - Order processing and management
- **API Gateway** - Central entry point using Ocelot for routing and rate limiting
- **Shared Library** - Common utilities, middleware, and interfaces

## 🚀 Technology Stack

- **.NET 8** - Core framework
- **Entity Framework Core 8** - ORM for database operations
- **SQL Server** - Database
- **Ocelot** - API Gateway
- **JWT Bearer Authentication** - Security
- **Serilog** - Logging
- **BCrypt.Net** - Password hashing
- **Polly** - Resilience and fault handling
- **xUnit, FakeItEasy, FluentAssertions** - Testing
- **.NET Aspire** - Microservices orchestration

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

## 🔧 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/7usseinel8areb/ecommerce-microservice-app.git
cd ecommerce-microservice-app
```

### 2. Database Setup

Update the connection string in each API's `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "eCommerceConnection": "Server=.;Database=ECommerceDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 3. Apply Database Migrations

Run migrations for each service:

```bash
# Authentication API
cd DemoECommerce.AuthenticationApiSolution/AuthenticationApi.Presentation
dotnet ef database update

# Product API
cd ../../DemoECommerce.ProductSolution/ProductApi.Presentation
dotnet ef database update

# Order API
cd ../../DemoECommerce.OrderApiSolution/OrderApi.Presentation
dotnet ef database update
```

### 4. Configure JWT Settings

Ensure all services use the same JWT configuration in `appsettings.json`:

```json
{
  "Authentication": {
    "Audience": "http://localhost:5000",
    "Issuer": "http://localhost:5000",
    "Key": "K7x!pZ9#sT2wL@q8R^bU4nE6vF$gJ1mD0cH3zP5yN"
  }
}
```

### 5. Run the Application

#### Option A: Using .NET Aspire (Recommended)

```bash
cd MicroservicesHost
dotnet run
```

Access the Aspire Dashboard at: `https://localhost:17007`

#### Option B: Manual Start

Open multiple terminals and run each service:

```bash
# Terminal 1 - Authentication API (Port 5000)
cd DemoECommerce.AuthenticationApiSolution/AuthenticationApi.Presentation
dotnet run

# Terminal 2 - Product API (Port 5001)
cd DemoECommerce.ProductSolution/ProductApi.Presentation
dotnet run

# Terminal 3 - Order API (Port 5002)
cd DemoECommerce.OrderApiSolution/OrderApi.Presentation
dotnet run

# Terminal 4 - API Gateway (Port 5003)
cd DemoECommerce.ApiGatewaySolution/ApiGateway.Presentation
dotnet run
```

## 🌐 API Endpoints

### API Gateway (Port 5003)

All requests should go through the API Gateway at `http://localhost:5003`

### Authentication Endpoints

```
POST   /api/authentication/register    - Register new user
POST   /api/authentication/login       - User login (returns JWT token)
GET    /api/authentication/{id}        - Get user by ID (requires auth)
```

### Product Endpoints

```
GET    /api/products                   - Get all products
GET    /api/products/{id}              - Get product by ID
POST   /api/products                   - Create product (Admin only)
PUT    /api/products                   - Update product (Admin only)
DELETE /api/products                   - Delete product (Admin only)
```

### Order Endpoints

```
GET    /api/orders                     - Get all orders (requires auth)
GET    /api/orders/{id}                - Get order by ID (requires auth)
GET    /api/orders/client/{clientId}   - Get orders by client ID (requires auth)
GET    /api/orders/details/{orderId}   - Get order details (requires auth)
POST   /api/orders                     - Create order (requires auth)
PUT    /api/orders                     - Update order (requires auth)
DELETE /api/orders/{id}                - Delete order (requires auth)
```

## 🔐 Authentication

### Register a User

```bash
POST http://localhost:5003/api/authentication/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "SecurePassword123",
  "telephoneNumber": "1234567890",
  "address": "123 Main St",
  "role": "User"
}
```

### Login

```bash
POST http://localhost:5003/api/authentication/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePassword123"
}
```

Response includes JWT token:
```json
{
  "flag": true,
  "message": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Using the Token

Include the token in the Authorization header:

```bash
GET http://localhost:5003/api/orders
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 🏛️ Project Structure

```
ecommerce-microservice-app/
├── DemoECommerce.AuthenticationApiSolution/
│   ├── AuthenticationApi.Application/       # Business logic
│   ├── AuthenticationApi.Domain/           # Domain entities
│   ├── AuthenticationApi.Infrastructure/   # Data access & services
│   └── AuthenticationApi.Presentation/     # API controllers
├── DemoECommerce.ProductSolution/
│   ├── ProductApi.Application/
│   ├── ProductApi.Domain/
│   ├── ProductApi.Infrastructure/
│   ├── ProductApi.Presentation/
│   └── UnitTest.ProductAPI/               # Unit tests
├── DemoECommerce.OrderApiSolution/
│   ├── OrderApi.Application/
│   ├── OrderApi.Domain/
│   ├── OrderApi.Infrastructure/
│   ├── OrderApi.Presentation/
│   └── UnitTest.OrderApi/                 # Unit tests
├── DemoECommerce.ApiGatewaySolution/
│   └── ApiGateway.Presentation/           # Ocelot gateway
├── DemoECommerce.SharedLibrarySolution/
│   └── eCommerce.SharedLibrary/           # Shared utilities
└── MicroservicesHost/                     # Aspire orchestration
```

## 🧪 Running Tests

```bash
# Product API Tests
cd DemoECommerce.ProductSolution/UnitTest.ProductAPI
dotnet test

# Order API Tests
cd ../../DemoECommerce.OrderApiSolution/UnitTest.OrderApi
dotnet test
```

## 🔒 Security Features

- **JWT Token Authentication** - Secure API endpoints
- **BCrypt Password Hashing** - Encrypted password storage
- **API Gateway Signature** - Only gateway-signed requests are accepted
- **Role-Based Authorization** - Admin-only endpoints for sensitive operations
- **Rate Limiting** - Prevent API abuse (configured in Ocelot)

## 🛡️ Middleware

### Global Exception Handler
Handles all unhandled exceptions and returns user-friendly error messages.

### Listen to Only API Gateway
Blocks direct calls to microservices - only accepts requests from the API Gateway.

### JWT Authentication
Validates JWT tokens for protected endpoints.

## 📊 Logging

Logs are written to:
- **Console** - Real-time monitoring
- **Debug Output** - Development debugging
- **File** - Persistent logs in `{ServiceName}-{Date}.text`

Example log location: `ProductApi-20251214.text`

## 🔄 Resilience Patterns

The application uses **Polly** for resilience:

- **Retry Policy** - Automatic retry on transient failures
- **Circuit Breaker** - Prevents cascading failures
- **Timeout Policy** - Prevents hanging requests

Configuration in `OrderApi.Application`:
```csharp
RetryStrategyOptions
{
    MaxRetryAttempts = 3,
    Delay = TimeSpan.FromMilliseconds(500),
    BackoffType = DelayBackoffType.Constant
}
```

## 🌟 Features

- ✅ Clean Architecture (Domain, Application, Infrastructure, Presentation)
- ✅ Repository Pattern
- ✅ Dependency Injection
- ✅ Global Error Handling
- ✅ Request/Response DTOs
- ✅ AutoMapper for object mapping
- ✅ Entity Framework Core with Migrations
- ✅ Swagger/OpenAPI Documentation
- ✅ Unit Testing with xUnit
- ✅ Mocking with FakeItEasy
- ✅ Fluent Assertions for readable tests

## 📝 API Gateway Configuration

The Ocelot gateway (`ocelot.json`) handles:

- **Routing** - Directs requests to appropriate microservices
- **Rate Limiting** - Controls request frequency
- **Caching** - Improves performance for GET requests
- **Authentication** - Enforces JWT validation
- **Load Balancing** - Distributes traffic (when scaled)

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👤 Author

**Hussein El Ghareb** - [7usseinel8areb](https://github.com/7usseinel8areb)

## 🙏 Acknowledgments

- .NET Community for excellent documentation
- Ocelot team for the API Gateway
- Polly team for resilience patterns
- All contributors who help improve this project

## 📞 Support

For issues, questions, or contributions, please open an issue on GitHub or contact the repository owner.

---

⭐ If you find this project helpful, please give it a star on GitHub!
