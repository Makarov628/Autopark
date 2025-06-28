namespace Autopark.DataGenerator.Models;

public class GeneratedData
{
    public List<GeneratedEnterprise> Enterprises { get; set; } = new();
    public List<GeneratedBrandModel> BrandModels { get; set; } = new();
    public List<GeneratedUser> Users { get; set; } = new();
    public List<GeneratedManager> Managers { get; set; } = new();
}

public class GeneratedEnterprise
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<GeneratedVehicle> Vehicles { get; set; } = new();
    public List<GeneratedDriver> Drivers { get; set; } = new();
}

public class GeneratedVehicle
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MileageInKilometers { get; set; }
    public string Color { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string BrandModelId { get; set; } = string.Empty;
    public string EnterpriseId { get; set; } = string.Empty;
    public string? ActiveDriverId { get; set; }
}

public class GeneratedBrandModel
{
    public string Id { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public string TransportType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public uint SeatsNumber { get; set; }
    public uint MaximumLoadCapacityInKillograms { get; set; }
}

public class GeneratedDriver
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string? VehicleId { get; set; }
}

public class GeneratedUser
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
}

public class GeneratedManager
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public List<string> EnterpriseIds { get; set; } = new();
}