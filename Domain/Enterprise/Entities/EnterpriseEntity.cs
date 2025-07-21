

using Autopark.Domain.Common.Models;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Vehicle.Entities;
using Autopark.Domain.Driver.Entities;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Manager.Entities;

namespace Autopark.Domain.Enterprise.Entities;

public class EnterpriseEntity : Entity<EnterpriseId>
{
    private readonly List<VehicleEntity> _vehicles = new();
    private readonly List<DriverEntity> _drivers = new();
    private readonly List<ManagerEnterpriseEntity> _enterpriseManagers = new();

    public CyrillicString Name { get; protected set; }
    public string Address { get; protected set; }
    public string? TimeZoneId { get; protected set; }

    public IReadOnlyList<VehicleEntity> Vehicles => _vehicles.AsReadOnly();
    public IReadOnlyList<DriverEntity> Drivers => _drivers.AsReadOnly();
    public IReadOnlyList<ManagerEnterpriseEntity> EnterpriseManagers => _enterpriseManagers.AsReadOnly();

    private EnterpriseEntity(
        EnterpriseId id,
        CyrillicString name,
        string address,
        string? timeZoneId) : base(id)
    {
        Name = name;
        Address = address;
        TimeZoneId = timeZoneId;
    }

    public static EnterpriseEntity Create(
        CyrillicString name,
        string address,
        string? timeZoneId = null) =>
            Create(
                EnterpriseId.Empty,
                name,
                address,
                timeZoneId);

    public static EnterpriseEntity Create(
        EnterpriseId id,
        CyrillicString name,
        string address,
        string? timeZoneId = null) =>
            new EnterpriseEntity(
                id,
                name,
                address,
                timeZoneId);

    public void Update(
        CyrillicString name,
        string address,
        string? timeZoneId = null)
    {
        Name = name;
        Address = address;
        TimeZoneId = timeZoneId;
    }

    protected EnterpriseEntity()
    {
    }
}