# Autopark Management System - API Documentation

## Table of Contents

1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Authentication & Authorization](#authentication--authorization)
4. [REST API Endpoints](#rest-api-endpoints)
5. [Domain Models](#domain-models)
6. [Use Cases (Commands & Queries)](#use-cases-commands--queries)
7. [Frontend Components](#frontend-components)
8. [Error Handling](#error-handling)
9. [Examples](#examples)

## Overview

The Autopark Management System is a comprehensive vehicle fleet management application built with:

- **Backend**: ASP.NET Core Web API with Clean Architecture
- **Frontend**: React.js with Vite
- **Architecture Pattern**: CQRS with MediatR
- **Database**: Entity Framework Core
- **Authentication**: JWT tokens with role-based authorization

## Architecture

The system follows Clean Architecture principles with the following layers:

- **Domain**: Core business entities and value objects
- **UseCases**: Application logic using CQRS pattern
- **Infrastructure**: Data access and external services
- **Web**: API controllers and React frontend

## Authentication & Authorization

### JWT Token Authentication

The system uses JWT tokens for authentication with the following roles:

- **Admin**: Full system access
- **Manager**: Enterprise management and user oversight
- **Driver**: Vehicle and trip access

### Authorization Endpoints

All authenticated endpoints require the `Authorization` header:
```
Authorization: Bearer <jwt_token>
```

## REST API Endpoints

### System Controller (`/api/system`)

#### Check System Setup Status
- **GET** `/api/system/check-setup`
- **Description**: Checks if the system has been initially configured
- **Authorization**: None required
- **Response**: 
```json
{
  "isSetupComplete": true,
  "message": "System is configured"
}
```

#### Initial System Setup
- **POST** `/api/system/initial-setup`
- **Description**: Performs initial system configuration with admin user
- **Authorization**: None required
- **Request Body**:
```json
{
  "adminData": {
    "email": "admin@example.com",
    "firstName": "Admin",
    "lastName": "User",
    "phone": "+1234567890",
    "password": "SecurePassword123!"
  }
}
```

#### Get System Information
- **GET** `/api/system/info`
- **Description**: Returns system information and version
- **Authorization**: None required
- **Response**:
```json
{
  "name": "Autopark Management System",
  "version": "1.0.0",
  "environment": "Development",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

#### Health Check
- **GET** `/api/system/health`
- **Description**: Returns system health status
- **Authorization**: None required
- **Response**:
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-01T00:00:00Z",
  "uptime": 123456789
}
```

### User Controller (`/api/user`)

#### User Registration
- **POST** `/api/user/register`
- **Description**: Register a new user account
- **Authorization**: None required
- **Request Body**:
```json
{
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phone": "+1234567890",
  "password": "SecurePassword123!",
  "dateOfBirth": "1990-01-01"
}
```

#### User Activation
- **POST** `/api/user/activate`
- **Description**: Activate user account with token
- **Authorization**: None required
- **Request Body**:
```json
{
  "token": "activation-token-here",
  "userId": 123
}
```

#### User Login
- **POST** `/api/user/login`
- **Description**: Authenticate user and receive JWT tokens
- **Authorization**: None required
- **Request Body**:
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```
- **Response**:
```json
{
  "accessToken": "jwt-access-token",
  "refreshToken": "jwt-refresh-token",
  "user": {
    "id": 123,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["Driver"]
  }
}
```

#### User Logout
- **POST** `/api/user/logout`
- **Description**: Logout user and invalidate tokens
- **Authorization**: Required
- **Request Body**:
```json
{
  "refreshToken": "jwt-refresh-token"
}
```

#### Refresh Token
- **POST** `/api/user/refresh-token`
- **Description**: Refresh access token using refresh token
- **Authorization**: None required
- **Request Body**:
```json
{
  "refreshToken": "jwt-refresh-token"
}
```

#### Set Push Token
- **POST** `/api/user/push-token`
- **Description**: Set push notification token for user
- **Authorization**: Required
- **Request Body**:
```json
{
  "pushToken": "firebase-push-token",
  "deviceId": "device-unique-id"
}
```

#### Get Current User
- **GET** `/api/user/me`
- **Description**: Get current authenticated user information
- **Authorization**: Required
- **Response**:
```json
{
  "id": 123,
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phone": "+1234567890",
  "isActive": true,
  "roles": ["Driver"]
}
```

#### Get All Users (Admin/Manager)
- **GET** `/api/user?page=1&pageSize=20&sortBy=firstName&sortDirection=asc`
- **Description**: Get paginated list of users
- **Authorization**: Admin, Manager
- **Query Parameters**:
  - `page` (optional): Page number (default: 1)
  - `pageSize` (optional): Items per page (default: 20)
  - `sortBy` (optional): Sort field
  - `sortDirection` (optional): "asc" or "desc"

#### Get User by ID (Admin/Manager)
- **GET** `/api/user/{id}`
- **Description**: Get specific user by ID
- **Authorization**: Admin, Manager

#### Get Available Users (Admin/Manager)
- **GET** `/api/user/available?notHasRole=Driver`
- **Description**: Get users without specific role
- **Authorization**: Admin, Manager
- **Query Parameters**:
  - `notHasRole`: Role to exclude (Admin, Manager, Driver)

### Vehicle Controller (`/api/vehicles`)

#### Get All Vehicles
- **GET** `/api/vehicles?page=1&pageSize=20&sortBy=name&sortDirection=asc`
- **Description**: Get paginated list of vehicles
- **Authorization**: Admin, Manager, Driver
- **Query Parameters**:
  - `page` (optional): Page number (default: 1)
  - `pageSize` (optional): Items per page (default: 20)
  - `sortBy` (optional): Sort field
  - `sortDirection` (optional): "asc" or "desc"
- **Response**:
```json
{
  "items": [
    {
      "id": 1,
      "name": "Company Car 1",
      "price": 25000.00,
      "mileageInKilometers": 15000.5,
      "color": "Blue",
      "registrationNumber": "ABC-123",
      "brandModelId": 1,
      "enterpriseId": 1,
      "activeDriverId": 123,
      "purchaseDate": "2023-01-01T00:00:00Z",
      "driverIds": [123, 456]
    }
  ],
  "totalCount": 50,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

#### Get Vehicle by ID
- **GET** `/api/vehicles/{id}`
- **Description**: Get specific vehicle by ID
- **Authorization**: Admin, Manager, Driver
- **Response**:
```json
{
  "id": 1,
  "name": "Company Car 1",
  "price": 25000.00,
  "mileageInKilometers": 15000.5,
  "color": "Blue",
  "registrationNumber": "ABC-123",
  "brandModelId": 1,
  "enterpriseId": 1,
  "activeDriverId": 123,
  "purchaseDate": "2023-01-01T00:00:00Z"
}
```

#### Create Vehicle
- **POST** `/api/vehicles`
- **Description**: Create new vehicle
- **Authorization**: Admin, Manager
- **Request Body**:
```json
{
  "name": "New Company Car",
  "price": 30000.00,
  "mileageInKilometers": 0,
  "color": "Red",
  "registrationNumber": "XYZ-789",
  "brandModelId": 2,
  "enterpriseId": 1,
  "purchaseDate": "2024-01-01T00:00:00Z"
}
```

#### Update Vehicle
- **PUT** `/api/vehicles`
- **Description**: Update existing vehicle
- **Authorization**: Admin, Manager
- **Request Body**:
```json
{
  "id": 1,
  "name": "Updated Car Name",
  "price": 28000.00,
  "mileageInKilometers": 16000.0,
  "color": "Green",
  "registrationNumber": "ABC-123",
  "brandModelId": 1,
  "enterpriseId": 1,
  "activeDriverId": 456,
  "purchaseDate": "2023-01-01T00:00:00Z"
}
```

#### Delete Vehicle
- **DELETE** `/api/vehicles/{id}`
- **Description**: Delete vehicle by ID
- **Authorization**: Admin, Manager
- **Response**: 204 No Content on success, 409 Conflict if vehicle is in use

### Driver Controller (`/api/drivers`)

#### Get All Drivers
- **GET** `/api/drivers?page=1&pageSize=20&sortBy=firstName&sortDirection=asc`
- **Description**: Get paginated list of drivers
- **Authorization**: Admin, Manager, Driver

#### Get Driver by ID
- **GET** `/api/drivers/{id}`
- **Description**: Get specific driver by ID
- **Authorization**: Admin, Manager, Driver

#### Create Driver
- **POST** `/api/drivers`
- **Description**: Create new driver
- **Authorization**: Admin, Manager
- **Request Body**:
```json
{
  "userId": 123,
  "licenseNumber": "DL123456789",
  "licenseExpiryDate": "2025-12-31T00:00:00Z",
  "enterpriseId": 1
}
```

#### Update Driver
- **PUT** `/api/drivers`
- **Description**: Update existing driver
- **Authorization**: Admin, Manager

#### Delete Driver
- **DELETE** `/api/drivers/{id}`
- **Description**: Delete driver by ID
- **Authorization**: Admin, Manager

### Enterprise Controller (`/api/enterprises`)

#### Get All Enterprises
- **GET** `/api/enterprises?page=1&pageSize=20&sortBy=name&sortDirection=asc`
- **Description**: Get paginated list of enterprises
- **Authorization**: Admin, Manager

#### Get Enterprise by ID
- **GET** `/api/enterprises/{id}`
- **Description**: Get specific enterprise by ID
- **Authorization**: Admin, Manager

#### Create Enterprise
- **POST** `/api/enterprises`
- **Description**: Create new enterprise
- **Authorization**: Admin
- **Request Body**:
```json
{
  "name": "New Enterprise",
  "address": "123 Business St",
  "phone": "+1234567890",
  "email": "contact@enterprise.com"
}
```

#### Update Enterprise
- **PUT** `/api/enterprises`
- **Description**: Update existing enterprise
- **Authorization**: Admin, Manager

#### Delete Enterprise
- **DELETE** `/api/enterprises/{id}`
- **Description**: Delete enterprise by ID
- **Authorization**: Admin

### Manager Controller (`/api/managers`)

#### Get All Managers
- **GET** `/api/managers?page=1&pageSize=20&sortBy=firstName&sortDirection=asc`
- **Description**: Get paginated list of managers
- **Authorization**: Admin, Manager

#### Get Manager by ID
- **GET** `/api/managers/{id}`
- **Description**: Get specific manager by ID
- **Authorization**: Admin, Manager

#### Create Manager
- **POST** `/api/managers`
- **Description**: Create new manager
- **Authorization**: Admin
- **Request Body**:
```json
{
  "userId": 123,
  "enterpriseIds": [1, 2, 3]
}
```

#### Delete Manager
- **DELETE** `/api/managers/{id}`
- **Description**: Delete manager by ID
- **Authorization**: Admin

#### Update Manager Enterprises
- **PUT** `/api/managers/{userId}/enterprises`
- **Description**: Update enterprises assigned to manager
- **Authorization**: Admin
- **Request Body**:
```json
[1, 2, 3]
```

### Brand Model Controller (`/api/brandmodels`)

#### Get All Brand Models
- **GET** `/api/brandmodels`
- **Description**: Get list of all brand models
- **Authorization**: Admin, Manager, Driver
- **Response**:
```json
[
  {
    "id": 1,
    "brand": "Toyota",
    "model": "Camry"
  },
  {
    "id": 2,
    "brand": "Honda",
    "model": "Accord"
  }
]
```

#### Get Brand Model by ID
- **GET** `/api/brandmodels/{id}`
- **Description**: Get specific brand model by ID
- **Authorization**: Admin, Manager, Driver

#### Create Brand Model
- **POST** `/api/brandmodels`
- **Description**: Create new brand model
- **Authorization**: Admin
- **Request Body**:
```json
{
  "brand": "Ford",
  "model": "Focus"
}
```

#### Update Brand Model
- **PUT** `/api/brandmodels`
- **Description**: Update existing brand model
- **Authorization**: Admin

#### Delete Brand Model
- **DELETE** `/api/brandmodels/{id}`
- **Description**: Delete brand model by ID
- **Authorization**: Admin

### Trips Controller (`/api/trips`)

#### Get Vehicle Track by Date Range
- **GET** `/api/trips/{vehicleId}/track?from=2024-01-01&to=2024-01-31&tz=UTC&format=json`
- **Description**: Get vehicle tracking data for date range
- **Authorization**: Varies by implementation
- **Query Parameters**:
  - `from`: Start date (ISO 8601)
  - `to`: End date (ISO 8601)
  - `tz` (optional): Timezone (default: UTC)
  - `format` (optional): "json" or "geojson" (default: json)
- **Response (JSON)**:
```json
{
  "trackPoints": [
    {
      "latitude": 40.7128,
      "longitude": -74.0060,
      "timestamp": "2024-01-01T10:00:00Z",
      "speed": 60.5
    }
  ]
}
```
- **Response (GeoJSON)**:
```json
{
  "type": "FeatureCollection",
  "features": [
    {
      "type": "Feature",
      "geometry": {
        "type": "LineString",
        "coordinates": [[-74.0060, 40.7128], [-74.0050, 40.7130]]
      },
      "properties": {}
    }
  ]
}
```

### Time Zone Controller (`/api/timezone`)

#### Get Available Time Zones
- **GET** `/api/timezone`
- **Description**: Get list of available time zones
- **Authorization**: Varies by implementation

## Domain Models

### Vehicle Entity

```csharp
public class VehicleEntity : Entity<VehicleId>
{
    public CyrillicString Name { get; protected set; }
    public Price Price { get; protected set; }
    public Mileage MileageInKilometers { get; set; }
    public CyrillicString Color { get; protected set; }
    public RegistrationNumber RegistrationNumber { get; protected set; }
    public DateTimeOffset? PurchaseDate { get; protected set; }
    public BrandModelId BrandModelId { get; protected set; }
    public EnterpriseId EnterpriseId { get; protected set; }
    public DriverId? ActiveDriverId { get; protected set; }
    public IReadOnlyList<DriverEntity> Drivers { get; }
}
```

### User Entity

```csharp
public class UserEntity : Entity<UserId>
{
    public string Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime? EmailConfirmed { get; set; }
    public DateTime? PhoneConfirmed { get; set; }
    public CyrillicString FirstName { get; set; }
    public CyrillicString LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public virtual Credentials Credentials { get; set; }
    public virtual ICollection<UserRole> Roles { get; set; }
    public virtual ICollection<Device> Devices { get; set; }
}
```

### Driver Entity

```csharp
public class DriverEntity : Entity<DriverId>
{
    public UserId UserId { get; protected set; }
    public string LicenseNumber { get; protected set; }
    public DateTime LicenseExpiryDate { get; protected set; }
    public EnterpriseId EnterpriseId { get; protected set; }
    // Navigation properties...
}
```

### Enterprise Entity

```csharp
public class EnterpriseEntity : Entity<EnterpriseId>
{
    public CyrillicString Name { get; protected set; }
    public string Address { get; protected set; }
    public string Phone { get; protected set; }
    public string Email { get; protected set; }
    // Navigation properties...
}
```

### Brand Model Entity

```csharp
public class BrandModelEntity : Entity<BrandModelId>
{
    public CyrillicString Brand { get; protected set; }
    public CyrillicString Model { get; protected set; }
}
```

## Use Cases (Commands & Queries)

### Vehicle Use Cases

#### Commands
- `CreateVehicleCommand`: Create new vehicle
- `UpdateVehicleCommand`: Update existing vehicle
- `DeleteVehicleCommand`: Delete vehicle

#### Queries
- `GetAllVehiclesQuery`: Get paginated vehicles list
- `GetByIdVehicleQuery`: Get vehicle by ID

### User Use Cases

#### Commands
- `CreateUserCommand`: Register new user
- `ActivateUserCommand`: Activate user account
- `LoginUserCommand`: Authenticate user
- `LogoutCommand`: Logout user
- `RefreshTokenCommand`: Refresh access token
- `SetPushTokenCommand`: Set push notification token

#### Queries
- `GetAllUserQuery`: Get paginated users list
- `GetByIdUserQuery`: Get user by ID

### Driver Use Cases

#### Commands
- `CreateDriverCommand`: Create new driver
- `UpdateDriverCommand`: Update existing driver
- `DeleteDriverCommand`: Delete driver

#### Queries
- `GetAllDriversQuery`: Get paginated drivers list
- `GetByIdDriverQuery`: Get driver by ID

### Enterprise Use Cases

#### Commands
- `CreateEnterpriseCommand`: Create new enterprise
- `UpdateEnterpriseCommand`: Update existing enterprise
- `DeleteEnterpriseCommand`: Delete enterprise

#### Queries
- `GetAllEnterprisesQuery`: Get paginated enterprises list
- `GetByIdEnterpriseQuery`: Get enterprise by ID

### Manager Use Cases

#### Commands
- `CreateManagerCommand`: Create new manager
- `DeleteManagerCommand`: Delete manager
- `UpdateManagerEnterprisesCommand`: Update manager's enterprises

#### Queries
- `GetAllManagerQuery`: Get paginated managers list
- `GetByIdManagerQuery`: Get manager by ID

### Brand Model Use Cases

#### Commands
- `CreateBrandModelCommand`: Create new brand model
- `UpdateBrandModelCommand`: Update existing brand model
- `DeleteBrandModelCommand`: Delete brand model

#### Queries
- `GetAllBrandModelQuery`: Get all brand models list
- `GetByIdBrandModelQuery`: Get brand model by ID

### System Use Cases

#### Commands
- `CheckSetupCommand`: Check system setup status
- `InitialSetupCommand`: Perform initial system setup

### Trip Use Cases

#### Queries
- `GetTripsByDateRangeQuery`: Get vehicle trips by date range

## Frontend Components

### React Application Structure

The frontend is a React application built with Vite, featuring:

- **Routing**: React Router for navigation
- **State Management**: Zustand stores
- **Styling**: Tailwind CSS
- **Authentication**: JWT token management

### Main Components

#### App.jsx
Main application component with routing and authentication logic.

#### Layout Components
- `Layout`: Main application layout with navigation
- `ProtectedRoute`: Route wrapper for authenticated pages
- `RequireManagerRole`: Role-based route protection

#### Page Components
- `DashboardPage`: Main dashboard
- `VehiclesPage`: Vehicle management
- `DriversPage`: Driver management
- `EnterprisesPage`: Enterprise management
- `ManagersPage`: Manager management
- `UsersPage`: User management
- `BrandModelsPage`: Brand model management
- `LoginPage`: User authentication
- `RegisterPage`: User registration
- `ActivatePage`: Account activation
- `InitialSetupPage`: System initial setup

#### UI Components

##### Button.jsx
Reusable button component with variants.

```jsx
<Button variant="primary" size="md" onClick={handleClick}>
  Click Me
</Button>
```

##### Input.jsx
Form input component with validation support.

```jsx
<Input
  type="text"
  placeholder="Enter value"
  value={value}
  onChange={setValue}
  error={error}
/>
```

##### Select.jsx
Dropdown select component.

```jsx
<Select
  options={options}
  value={selectedValue}
  onChange={setSelectedValue}
  placeholder="Select option"
/>
```

##### Alert.jsx
Alert/notification component.

```jsx
<Alert type="success" message="Operation completed successfully" />
```

##### LoadingSpinner.jsx
Loading indicator component.

```jsx
<LoadingSpinner size="lg" />
```

##### Pagination.jsx
Pagination component for data tables.

```jsx
<Pagination
  currentPage={page}
  totalPages={totalPages}
  onPageChange={setPage}
/>
```

##### DateTimeWithTimeZone.jsx
Date/time picker with timezone support.

```jsx
<DateTimeWithTimeZone
  value={dateTime}
  onChange={setDateTime}
  timezone={timezone}
/>
```

##### TimeZoneSelect.jsx
Timezone selection component.

```jsx
<TimeZoneSelect
  value={timezone}
  onChange={setTimezone}
/>
```

### Services

#### authService.js
Authentication service for token management and API calls.

```javascript
// Login user
const loginResponse = await authService.login(email, password);

// Logout user
await authService.logout();

// Get current user
const user = await authService.getCurrentUser();

// Check if user is authenticated
const isAuthenticated = authService.isAuthenticated();
```

### Stores (Zustand)

#### authStore.js
Authentication state management.

```javascript
const useAuthStore = () => {
  const { user, isAuthenticated, isLoading, login, logout } = useAuthStore();
  
  // Use authentication state
  if (isAuthenticated) {
    // User is logged in
  }
};
```

## Error Handling

### HTTP Status Codes

- **200 OK**: Successful GET, PUT requests
- **201 Created**: Successful POST requests
- **204 No Content**: Successful DELETE requests
- **400 Bad Request**: Validation errors
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **409 Conflict**: Resource conflict (e.g., entity in use)
- **500 Internal Server Error**: Server errors

### Error Response Format

```json
{
  "message": "Error description",
  "details": "Additional error details",
  "statusCode": 400
}
```

### Validation Errors

```json
{
  "message": "Validation failed",
  "errors": {
    "Email": ["Email is required", "Email format is invalid"],
    "Password": ["Password must be at least 8 characters"]
  }
}
```

## Examples

### Complete User Registration Flow

1. **Register User**
```bash
curl -X POST "https://api.autopark.com/api/user/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phone": "+1234567890",
    "password": "SecurePassword123!",
    "dateOfBirth": "1990-01-01"
  }'
```

2. **Activate Account**
```bash
curl -X POST "https://api.autopark.com/api/user/activate" \
  -H "Content-Type: application/json" \
  -d '{
    "token": "activation-token-received-via-email",
    "userId": 123
  }'
```

3. **Login**
```bash
curl -X POST "https://api.autopark.com/api/user/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "password": "SecurePassword123!"
  }'
```

### Vehicle Management Example

1. **Create Vehicle**
```bash
curl -X POST "https://api.autopark.com/api/vehicles" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "name": "Company Fleet Car #1",
    "price": 25000.00,
    "mileageInKilometers": 0,
    "color": "Blue",
    "registrationNumber": "ABC-123",
    "brandModelId": 1,
    "enterpriseId": 1,
    "purchaseDate": "2024-01-01T00:00:00Z"
  }'
```

2. **Get Vehicles List**
```bash
curl -X GET "https://api.autopark.com/api/vehicles?page=1&pageSize=20&sortBy=name&sortDirection=asc" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

3. **Update Vehicle**
```bash
curl -X PUT "https://api.autopark.com/api/vehicles" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "id": 1,
    "name": "Updated Company Car",
    "price": 23000.00,
    "mileageInKilometers": 5000.0,
    "color": "Red",
    "registrationNumber": "ABC-123",
    "brandModelId": 1,
    "enterpriseId": 1,
    "activeDriverId": 456
  }'
```

### Frontend Component Usage Example

```jsx
import React, { useState, useEffect } from 'react';
import { Button, Input, Alert, LoadingSpinner } from './components/ui';
import useAuthStore from './stores/authStore';

const VehicleForm = () => {
  const [vehicle, setVehicle] = useState({
    name: '',
    price: '',
    color: '',
    registrationNumber: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  
  const { isAuthenticated } = useAuthStore();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    
    try {
      const response = await fetch('/api/vehicles', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
        },
        body: JSON.stringify(vehicle)
      });
      
      if (response.ok) {
        setSuccess('Vehicle created successfully!');
        setVehicle({ name: '', price: '', color: '', registrationNumber: '' });
      } else {
        const errorData = await response.json();
        setError(errorData.message || 'Failed to create vehicle');
      }
    } catch (err) {
      setError('Network error occurred');
    } finally {
      setLoading(false);
    }
  };

  if (!isAuthenticated) {
    return <div>Please login to access this page</div>;
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <h2 className="text-2xl font-bold">Create New Vehicle</h2>
      
      {error && <Alert type="error" message={error} />}
      {success && <Alert type="success" message={success} />}
      
      <Input
        type="text"
        placeholder="Vehicle Name"
        value={vehicle.name}
        onChange={(e) => setVehicle({...vehicle, name: e.target.value})}
        required
      />
      
      <Input
        type="number"
        placeholder="Price"
        value={vehicle.price}
        onChange={(e) => setVehicle({...vehicle, price: e.target.value})}
        required
      />
      
      <Input
        type="text"
        placeholder="Color"
        value={vehicle.color}
        onChange={(e) => setVehicle({...vehicle, color: e.target.value})}
        required
      />
      
      <Input
        type="text"
        placeholder="Registration Number"
        value={vehicle.registrationNumber}
        onChange={(e) => setVehicle({...vehicle, registrationNumber: e.target.value})}
        required
      />
      
      <Button type="submit" disabled={loading}>
        {loading ? <LoadingSpinner size="sm" /> : 'Create Vehicle'}
      </Button>
    </form>
  );
};

export default VehicleForm;
```

This documentation provides comprehensive coverage of all public APIs, domain models, use cases, and frontend components in the Autopark Management System, including practical examples and usage instructions for developers.