

using Autopark.Domain.Common.Models;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Driver.ValueObjects;
using Autopark.Domain.Enterprise.Entities;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Vehicle.Entities;
using Autopark.Domain.Vehicle.ValueObjects;

namespace Autopark.Domain.Driver.Entities;

public class DriverEntity : Entity<DriverId>
{
    public CyrillicString FirstName { get; protected set; }
    public CyrillicString LastName { get; protected set; }
    public DateTime DateOfBirth { get; protected set; }
    public decimal Salary { get; protected set; }

    public EnterpriseId EnterpriseId { get; protected set; }
    public EnterpriseEntity Enterprise { get; protected set; }

    public VehicleId? VehicleId { get; protected set; }
    public VehicleEntity? Vehicle { get; protected set; }

    private DriverEntity(
        DriverId id,
        CyrillicString firstName,
        CyrillicString lastName,
        DateTime dateOfBirth,
        decimal salary,
        EnterpriseId enterpriseId) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Salary = salary;
        EnterpriseId = enterpriseId;
    }

    public static DriverEntity Create(
        CyrillicString firstName,
        CyrillicString lastName,
        DateTime dateOfBirth,
        decimal salary,
        EnterpriseId enterpriseId) =>
            Create(
                DriverId.Empty,
                firstName,
                lastName,
                dateOfBirth,
                salary,
                enterpriseId);

    public static DriverEntity Create(
        DriverId id,
        CyrillicString firstName,
        CyrillicString lastName,
        DateTime dateOfBirth,
        decimal salary,
        EnterpriseId enterpriseId) =>
            new DriverEntity(
                id,
                firstName,
                lastName,
                dateOfBirth,
                salary,
                enterpriseId);

    public void Update(
        CyrillicString firstName,
        CyrillicString lastName,
        DateTime dateOfBirth,
        decimal salary,
        EnterpriseId enterpriseId,
        VehicleId? vehicleId)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Salary = salary;
        EnterpriseId = enterpriseId;
        VehicleId = vehicleId;
    }

    protected DriverEntity()
    {
    }
}