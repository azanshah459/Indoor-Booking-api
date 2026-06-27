# Indoor Booking Management API

A RESTful Web API built with ASP.NET Core for managing indoor sports facility bookings. Supports Cricket and Futsal ground bookings with role-based access control.

## Tech Stack

- **Backend:** ASP.NET Core 8 Web API
- **Database:** SQL Server with Entity Framework Core
- **Authentication:** JWT Bearer Tokens
- **Password Hashing:** BCrypt
- **Documentation:** Swagger / OpenAPI

## Features

### Customer
- Register and login with JWT authentication
- Browse available grounds (Cricket / Futsal)
- Select a date and view auto-generated time slots
- Book available slots
- View booking history
- Cancel bookings

### Admin
- Manage grounds (add / delete)
- View all bookings across all customers
- Dashboard with stats (total bookings, revenue, today's bookings)
- Monthly revenue reports

## Architecture

- **Service Pattern** with interfaces for clean separation of concerns
- **DTOs** (Data Transfer Objects) to separate API contracts from database models
- **Dependency Injection** via ASP.NET Core's built-in DI container
- **Repository-free EF Core** with DbContext injected into services

## Database Schema
Users        → UserId, Name, Email, PasswordHash, Role
Grounds      → GroundId, Name, Type, Description, HourlyRate
Slots        → SlotId, GroundId, Date, StartTime, EndTime, IsAvailable
Bookings     → BookingId, UserId, SlotId, BookingDate, Status

## Auto Slot Generation

When a customer searches for available slots on a date, the system
automatically generates 13 slots per ground (12 AM–6 AM and 5 PM–11 PM)
if they do not already exist. No manual slot creation needed.

## API Endpoints

### Auth
| Method | Endpoint | Access |
|--------|----------|--------|
| POST | /api/auth/register | Public |
| POST | /api/auth/login | Public |

### Grounds
| Method | Endpoint | Access |
|--------|----------|--------|
| GET | /api/grounds | Public |
| GET | /api/grounds/{id} | Public |
| POST | /api/grounds | Admin only |
| PUT | /api/grounds/{id} | Admin only |
| DELETE | /api/grounds/{id} | Admin only |

### Slots
| Method | Endpoint | Access |
|--------|----------|--------|
| GET | /api/slots/ground/{id}/available?date= | Public |
| POST | /api/slots | Admin only |
| DELETE | /api/slots/{id} | Admin only |

### Bookings
| Method | Endpoint | Access |
|--------|----------|--------|
| POST | /api/bookings | Customer |
| GET | /api/bookings/my | Customer |
| GET | /api/bookings | Admin only |
| PUT | /api/bookings/{id}/cancel | Customer |

### Admin
| Method | Endpoint | Access |
|--------|----------|--------|
| GET | /api/admin/dashboard | Admin only |
| GET | /api/admin/users | Admin only |
| GET | /api/admin/bookings/today | Admin only |
| GET | /api/admin/revenue/monthly | Admin only |

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server Express
- Visual Studio 2022 or VS Code

### Setup

1. Clone the repository
git clone https://github.com/azanshah459/indoor-booking-api.git

2. Update the connection string in `appsettings.json`
```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=IndoorManagementDB;Trusted_Connection=True;TrustServerCertificate=True"
   }
```
3. Apply database migrations
Update-Database

4. Run the project
dotnet run

5. Open Swagger at `http://localhost:5000/swagger`

## Default Roles

- Register normally to get a **Customer** account
- To create an **Admin**, manually update the Role field in the Users table to `Admin`