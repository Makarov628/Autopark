using Autopark.Domain.Common.Models;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Driver.ValueObjects;
using Autopark.Domain.Enterprise.Entities;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Vehicle.Entities;
using Autopark.Domain.Vehicle.ValueObjects;
using Autopark.Domain.User.ValueObjects;
using Autopark.Domain.User.Entities;
using System;

namespace Autopark.Domain.Driver.Entities;

public class DriverEntity : Entity<DriverId>
{
    public UserId UserId { get; set; }
    public virtual UserEntity User { get; set; }

    public decimal Salary { get; protected set; }
    public EnterpriseId EnterpriseId { get; protected set; }
    public EnterpriseEntity Enterprise { get; protected set; }
    public VehicleId? VehicleId { get; protected set; }
    public VehicleEntity? Vehicle { get; protected set; }

    private DriverEntity(
        DriverId id,
        UserId userId,
        decimal salary,
        EnterpriseId enterpriseId) : base(id)
    {
        UserId = userId;
        Salary = salary;
        EnterpriseId = enterpriseId;
    }

    public static DriverEntity Create(
        DriverId id,
        UserId userId,
        decimal salary,
        EnterpriseId enterpriseId)
    {
        return new DriverEntity(id, userId, salary, enterpriseId);
    }

    public void AttachToVehicle(VehicleId vehicleId)
    {
        VehicleId = vehicleId;
    }

    public void DetachFromVehicle()
    {
        VehicleId = null;
    }

    public void UpdateEnterprise(EnterpriseId enterpriseId)
    {
        EnterpriseId = enterpriseId;
    }

    public void UpdateSalary(decimal salary)
    {
        Salary = salary;
    }
}