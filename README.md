# Banking System

A secure banking application built with ASP.NET Core Web API, Entity Framework Core, and ASP.NET Core Identity for user management and authentication.

##  Tech Stack

### Backend
- **Framework**: ASP.NET Core 9.0 Web API
- **Database**: SQL Server with Entity Framework Core 9.0
- **Authentication**: ASP.NET Core Identity + JWT Bearer tokens
- **Testing**: xUnit, Moq,

### Security Features
- JWT-based authentication
- Role-based authorization (User/Admin roles)
- Password hashing and validation
- Secure API endpoints

##  Architecture & Design Choices

### Project Structure
```
Banking.sln
├── Banking/                    # Main Web API project
│   ├── Controllers/           # API Controllers
│   ├── Repositories/         # Data access layer
│   └── Migrations/           # EF Core migrations
├── Banking.DAL/              # Data Access Layer
│   ├── DbContext/           # Database context
│   ├── Entity/              # Database entities
│   └── DTO/                 # Data Transfer Objects
└── Banking.Tests/            # Unit and integration tests
```

### Design Patterns Used

1. **Repository Pattern**: Abstracts data access logic
   - `IAccountRepository`, `ITransactionRepository`
   - Enables easier testing and maintainability

2. **Dependency Injection**: Used throughout the application
   - Services registered in `Program.cs`
   - Promotes loose coupling and testability

3. **DTO Pattern**: Separates internal entities from API contracts
   - `LoginDto`, `TransferDto`, `AccountResponseDto`, etc.
   - Provides better API versioning and security

### Database Design

- **Users**: ASP.NET Core Identity tables for user management
- **Accounts**: Bank accounts linked to users
- **Transactions**: Implicit through business operations (deposit, withdraw, transfer)

##  Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) or SQL Server
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Installation & Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/vlad3958/Banking-system.git
   cd Banking-system
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string** (if needed)
   
   Edit `Banking/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BankingDB;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Apply database migrations**
   ```bash
   cd Banking
   dotnet ef database update
   ```

5. **Build the solution**
   ```bash
   dotnet build
   ```

6. **Run the application**
   ```bash
   dotnet run --project Banking
   ```

   The API will be available at:
   - HTTPS: `https://localhost:7xxx`
   - HTTP: `http://localhost:5xxx`
   - Swagger UI: `https://localhost:7xxx/swagger`

##  Running Tests

### Run all tests
```bash
dotnet test
```

##  API Documentation

### Authentication Endpoints

#### POST `/api/auth/register`
Register a new user and create a bank account.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe",
  "initialBalance": 1000.00
}
```

#### POST `/api/auth/login`
Authenticate user and receive JWT token.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response:**
```json
{
  "isSuccessful": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "isAdmin": false
}
```

### Account Endpoints

#### GET `/api/accounts/GetAllAccounts`
Get all accounts (simplified view).

#### GET `/api/accounts/GetAccountByAccountNumber/{accountNumber}`
Get account details by account number (Admin only).

#### GET `/api/accounts/myAccountInfo`
Get current user's account information.

### Transaction Endpoints

#### POST `/api/transactions/transfer`
Transfer money between accounts.

**Request:**
```json
{
  "fromAccountNumber": "ABC123456789",
  "toAccountNumber": "XYZ987654321",
  "amount": 100.00
}
```

#### POST `/api/transactions/deposit`
Deposit money to an account.

#### POST `/api/transactions/withdraw`
Withdraw money from an account.
