using LanguageExt;
using Autopark.Domain.Common;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Vehicle.ValueObjects;
using LanguageExt.Common;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.BrandModel.ValueObjects;
using Autopark.Domain.BrandModel.Entities;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Enterprise.Entities;
using Autopark.Domain.Driver.Entities;
using Autopark.Domain.Driver.ValueObjects;

namespace Autopark.Domain.Vehicle.Entities;

public class VehicleEntity : Entity<VehicleId>
{
    private readonly List<DriverEntity> _drivers = new();

    public CyrillicString Name { get; protected set; }
    public Price Price { get; protected set; }
    public Mileage MileageInKilometers { get; set; }
    public CyrillicString Color { get; protected set; }
    public RegistrationNumber RegistrationNumber { get; protected set; }
    public DateTimeOffset? PurchaseDate { get; protected set; }

    public BrandModelId BrandModelId { get; protected set; }
    public BrandModelEntity BrandModel { get; protected set; }

    public EnterpriseId EnterpriseId { get; protected set; }
    public EnterpriseEntity Enterprise { get; protected set; }

    public DriverId? ActiveDriverId { get; protected set; }

    public IReadOnlyList<DriverEntity> Drivers => _drivers.AsReadOnly();

    private VehicleEntity(
        VehicleId id,
        CyrillicString name,
        Price price,
        Mileage mileageInKilometers,
        CyrillicString color,
        RegistrationNumber registrationNumber,
        BrandModelId brandModelId,
        EnterpriseId enterpriseId,
        DateTimeOffset? purchaseDate = null) : base(id)
    {
        Name = name;
        Price = price;
        MileageInKilometers = mileageInKilometers;
        Color = color;
        RegistrationNumber = registrationNumber;
        BrandModelId = brandModelId;
        EnterpriseId = enterpriseId;
        PurchaseDate = purchaseDate;
    }

    public static VehicleEntity Create(
        CyrillicString name,
        Price price,
        Mileage mileageInKilometers,
        CyrillicString color,
        RegistrationNumber registrationNumber,
        BrandModelId brandModelId,
        EnterpriseId enterpriseId,
        DateTimeOffset? purchaseDate = null) =>
            Create(
                VehicleId.Empty,
                name,
                price,
                mileageInKilometers,
                color,
                registrationNumber,
                brandModelId,
                enterpriseId,
                purchaseDate);

    public static VehicleEntity Create(
        VehicleId id,
        CyrillicString name,
        Price price,
        Mileage mileageInKilometers,
        CyrillicString color,
        RegistrationNumber registrationNumber,
        BrandModelId brandModelId,
        EnterpriseId enterpriseId,
        DateTimeOffset? purchaseDate = null) =>
            new VehicleEntity(
                id,
                name,
                price,
                mileageInKilometers,
                color,
                registrationNumber,
                brandModelId,
                enterpriseId,
                purchaseDate);

    // TODO: Temp method. Needs to decompose to separate methods
    public void Update(
        CyrillicString name,
        Price price,
        Mileage mileageInKilometers,
        CyrillicString color,
        RegistrationNumber registrationNumber,
        BrandModelId brandModelId,
        EnterpriseId enterpriseId,
        DriverId? activeDriverId,
        DateTimeOffset? purchaseDate = null)
    {
        Name = name;
        Price = price;
        MileageInKilometers = mileageInKilometers;
        Color = color;
        RegistrationNumber = registrationNumber;
        BrandModelId = brandModelId;
        EnterpriseId = enterpriseId;
        ActiveDriverId = activeDriverId;
        PurchaseDate = purchaseDate;
    }

    protected VehicleEntity()
    {
    }
}
