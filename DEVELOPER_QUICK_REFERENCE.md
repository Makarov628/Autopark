# Autopark Management System - Developer Quick Reference

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+
- SQL Server or PostgreSQL
- Docker (optional)

### Running the Application

1. **Backend**:
```bash
cd Web
dotnet run
# API available at: https://localhost:5001
```

2. **Frontend**:
```bash
cd Web/client-app
npm install
npm run dev
# App available at: http://localhost:5173
```

3. **Docker**:
```bash
docker-compose up
```

## Most Used API Endpoints

### Authentication
```bash
# Login
POST /api/user/login
Body: {"email": "user@example.com", "password": "password"}

# Get current user
GET /api/user/me
Header: Authorization: Bearer <token>

# Refresh token
POST /api/user/refresh-token
Body: {"refreshToken": "refresh_token_here"}
```

### Vehicles
```bash
# Get all vehicles
GET /api/vehicles?page=1&pageSize=20

# Get vehicle by ID
GET /api/vehicles/{id}

# Create vehicle
POST /api/vehicles
Body: {
  "name": "Car Name",
  "price": 25000,
  "mileageInKilometers": 0,
  "color": "Blue",
  "registrationNumber": "ABC-123",
  "brandModelId": 1,
  "enterpriseId": 1
}

# Update vehicle
PUT /api/vehicles
Body: {
  "id": 1,
  "name": "Updated Name",
  ...
}

# Delete vehicle
DELETE /api/vehicles/{id}
```

### Drivers
```bash
# Get all drivers
GET /api/drivers?page=1&pageSize=20

# Create driver
POST /api/drivers
Body: {
  "userId": 123,
  "licenseNumber": "DL123456789",
  "licenseExpiryDate": "2025-12-31T00:00:00Z",
  "enterpriseId": 1
}
```

### Enterprises
```bash
# Get all enterprises
GET /api/enterprises?page=1&pageSize=20

# Create enterprise
POST /api/enterprises
Body: {
  "name": "Enterprise Name",
  "address": "123 Business St",
  "phone": "+1234567890",
  "email": "contact@enterprise.com"
}
```

## Common Frontend Components

### Basic Form Example
```jsx
import { useState } from 'react';
import { Button, Input, Alert } from './components/ui';

const MyForm = () => {
  const [data, setData] = useState({ name: '', email: '' });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const response = await fetch('/api/endpoint', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
        },
        body: JSON.stringify(data)
      });
      
      if (!response.ok) throw new Error('Failed to submit');
      // Handle success
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      {error && <Alert type="error" message={error} />}
      <Input
        placeholder="Name"
        value={data.name}
        onChange={(e) => setData({...data, name: e.target.value})}
      />
      <Input
        type="email"
        placeholder="Email"
        value={data.email}
        onChange={(e) => setData({...data, email: e.target.value})}
      />
      <Button type="submit" disabled={loading}>
        {loading ? 'Submitting...' : 'Submit'}
      </Button>
    </form>
  );
};
```

### Data Table with Pagination
```jsx
import { useState, useEffect } from 'react';
import { Pagination, LoadingSpinner } from './components/ui';

const DataTable = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    fetchData();
  }, [page]);

  const fetchData = async () => {
    setLoading(true);
    try {
      const response = await fetch(`/api/vehicles?page=${page}&pageSize=20`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
        }
      });
      const result = await response.json();
      setData(result.items);
      setTotalPages(result.totalPages);
    } catch (err) {
      console.error('Failed to fetch data:', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <table className="min-w-full">
        <thead>
          <tr>
            <th>Name</th>
            <th>Price</th>
            <th>Registration</th>
          </tr>
        </thead>
        <tbody>
          {data.map(item => (
            <tr key={item.id}>
              <td>{item.name}</td>
              <td>${item.price}</td>
              <td>{item.registrationNumber}</td>
            </tr>
          ))}
        </tbody>
      </table>
      <Pagination
        currentPage={page}
        totalPages={totalPages}
        onPageChange={setPage}
      />
    </div>
  );
};
```

### Authentication Hook
```jsx
import useAuthStore from './stores/authStore';

const useAuth = () => {
  const { user, isAuthenticated, login, logout } = useAuthStore();

  const handleLogin = async (email, password) => {
    try {
      const response = await fetch('/api/user/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
      });
      
      if (response.ok) {
        const data = await response.json();
        localStorage.setItem('accessToken', data.accessToken);
        localStorage.setItem('refreshToken', data.refreshToken);
        login(data.user);
        return { success: true };
      } else {
        const error = await response.json();
        return { success: false, error: error.message };
      }
    } catch (err) {
      return { success: false, error: 'Network error' };
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    logout();
  };

  return {
    user,
    isAuthenticated,
    login: handleLogin,
    logout: handleLogout
  };
};
```

## User Roles & Permissions

| Endpoint | Admin | Manager | Driver |
|----------|-------|---------|--------|
| `/api/system/*` | ✅ | ✅ | ✅ |
| `/api/user/register` | ✅ | ❌ | ❌ |
| `/api/user/me` | ✅ | ✅ | ✅ |
| `/api/user` (list) | ✅ | ✅ | ❌ |
| `/api/vehicles` (read) | ✅ | ✅ | ✅ |
| `/api/vehicles` (write) | ✅ | ✅ | ❌ |
| `/api/drivers` (read) | ✅ | ✅ | ✅ |
| `/api/drivers` (write) | ✅ | ✅ | ❌ |
| `/api/enterprises` (read) | ✅ | ✅ | ❌ |
| `/api/enterprises` (write) | ✅ | ✅* | ❌ |
| `/api/managers` | ✅ | ✅ | ❌ |
| `/api/brandmodels` (read) | ✅ | ✅ | ✅ |
| `/api/brandmodels` (write) | ✅ | ❌ | ❌ |

*Managers can only update, not create/delete enterprises

## Common Error Codes

| Code | Meaning | Common Causes |
|------|---------|---------------|
| 400 | Bad Request | Invalid input data, validation errors |
| 401 | Unauthorized | Missing or invalid JWT token |
| 403 | Forbidden | Insufficient role permissions |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Resource in use (can't delete) |
| 500 | Server Error | Database issues, unexpected errors |

## Environment Variables

### Backend (.NET)
```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection="Server=localhost;Database=Autopark;Trusted_Connection=true;"
JWT__Secret="your-secret-key-here"
JWT__Issuer="Autopark.API"
JWT__Audience="Autopark.Client"
JWT__ExpiryInMinutes=60
```

### Frontend (React)
```bash
VITE_API_BASE_URL=https://localhost:5001
VITE_APP_NAME="Autopark Management"
```

## Database Entities Overview

```
UserEntity
├── Credentials (1:1)
├── UserRoles (1:N)
├── Devices (1:N)
└── DriverEntity (1:1) [optional]

VehicleEntity
├── BrandModelEntity (N:1)
├── EnterpriseEntity (N:1)
├── DriverEntity (N:N)
└── ActiveDriver (N:1) [optional]

EnterpriseEntity
├── VehicleEntity (1:N)
├── DriverEntity (1:N)
└── ManagerEntity (N:N)

DriverEntity
├── UserEntity (1:1)
├── EnterpriseEntity (N:1)
└── VehicleEntity (N:N)

ManagerEntity
├── UserEntity (1:1)
└── EnterpriseEntity (N:N)
```

## Common CQRS Patterns

### Command Handler Example
```csharp
public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Fin<Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public CreateVehicleCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<Unit>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var vehicle = VehicleEntity.Create(
                CyrillicString.Create(request.Name),
                Price.Create(request.Price),
                Mileage.Create(request.MileageInKilometers),
                CyrillicString.Create(request.Color),
                RegistrationNumber.Create(request.RegistrationNumber),
                BrandModelId.Create(request.BrandModelId),
                EnterpriseId.Create(request.EnterpriseId),
                request.PurchaseDate
            );

            _dbContext.Vehicles.Add(vehicle);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Default;
        }
        catch (Exception ex)
        {
            return Fin<Unit>.Fail(Error.New(ex.Message));
        }
    }
}
```

### Query Handler Example
```csharp
public class GetAllVehiclesQueryHandler : IRequestHandler<GetAllVehiclesQuery, Fin<PagedResult<VehiclesResponse>>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetAllVehiclesQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<PagedResult<VehiclesResponse>>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _dbContext.Vehicles
                .Include(v => v.BrandModel)
                .Include(v => v.Enterprise)
                .AsQueryable();

            // Apply filtering, sorting, pagination...
            
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(v => new VehiclesResponse(
                    v.Id.Value,
                    v.Name.Value,
                    v.Price.Value,
                    v.MileageInKilometers.Value,
                    v.Color.Value,
                    v.RegistrationNumber.Value,
                    v.BrandModelId.Value,
                    v.EnterpriseId.Value,
                    v.ActiveDriverId?.Value,
                    v.PurchaseDate,
                    v.Drivers.Select(d => d.Id.Value).ToArray()
                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<VehiclesResponse>(items, totalCount, request.Page, request.PageSize);
        }
        catch (Exception ex)
        {
            return Fin<PagedResult<VehiclesResponse>>.Fail(Error.New(ex.Message));
        }
    }
}
```

## Useful Commands

### .NET CLI
```bash
# Build solution
dotnet build

# Run tests
dotnet test

# Add migration
dotnet ef migrations add MigrationName --project Infrastructure/Database

# Update database
dotnet ef database update --project Infrastructure/Database

# Generate controllers
dotnet aspnet-codegenerator controller -name VehiclesController -async -api -m VehicleEntity -dc AutoparkDbContext -outDir Controllers
```

### NPM Scripts
```bash
# Development
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Lint code
npm run lint

# Format code
npm run format
```

This quick reference provides the most commonly used APIs, patterns, and code examples for efficient development with the Autopark Management System.