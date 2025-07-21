using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Autopark.UseCases.Vehicle.Commands.Update;
using Autopark.UseCases.Vehicle.Queries.GetById;
using Autopark.UseCases.Enterprise.Queries.GetAll;
using Autopark.UseCases.BrandModel.Queries.GetAll;
using Autopark.UseCases.Driver.Queries.GetAll;
using MediatR;

namespace Autopark.Web.Pages.Manager;

public class VehicleEditModel : PageModel
{
    private readonly IMediator _mediator;

    public VehicleEditModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty]
    public VehicleEditViewModel Vehicle { get; set; } = new();

    public List<EnterpriseViewModel> Enterprises { get; set; } = new();
    public List<BrandModelViewModel> BrandModels { get; set; } = new();
    public List<DriverViewModel> Drivers { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            // Загружаем данные автомобиля
            var vehicleQuery = new GetByIdVehicleQuery(id);
            var vehicleResult = await _mediator.Send(vehicleQuery);

            vehicleResult.Match(
                vehicle =>
                {
                    Vehicle = new VehicleEditViewModel
                    {
                        Id = vehicle.Id,
                        Name = vehicle.Name,
                        Price = vehicle.Price,
                        MileageInKilometers = vehicle.MileageInKilometers,
                        Color = "Цвет", // TODO: Добавить цвет в API
                        RegistrationNumber = vehicle.RegistrationNumber,
                        BrandModelId = vehicle.BrandModelId,
                        EnterpriseId = vehicle.EnterpriseId,
                        ActiveDriverId = vehicle.ActiveDriverId,
                        PurchaseDate = vehicle.PurchaseDate?.DateTime
                    };
                },
                error =>
                {
                    ErrorMessage = $"Автомобиль не найден: {error.Message}";
                    return;
                }
            );

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                return Page();
            }

            // Загружаем список предприятий
            var enterprisesQuery = new GetAllEnterprisesQuery(1, 1000);
            var enterprisesResult = await _mediator.Send(enterprisesQuery);

            enterprisesResult.Match(
                enterprises =>
                {
                    Enterprises = enterprises.Items.Select(e => new EnterpriseViewModel
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Address = e.Address,
                        TimeZoneId = e.TimeZoneId
                    }).ToList();
                },
                error =>
                {
                    ErrorMessage = $"Ошибка загрузки предприятий: {error}";
                    return;
                }
            );

            // Загружаем список моделей
            var brandModelsQuery = new GetAllBrandModelQuery();
            var brandModelsResult = await _mediator.Send(brandModelsQuery);

            brandModelsResult.Match(
                brandModels =>
                {
                    BrandModels = brandModels.Select(bm => new BrandModelViewModel
                    {
                        Id = bm.Id,
                        BrandName = bm.BrandName,
                        ModelName = bm.ModelName,
                        FuelType = bm.FuelType.ToString(),
                        TransportType = bm.TransportType.ToString()
                    }).ToList();
                },
                error =>
                {
                    ErrorMessage = $"Ошибка загрузки моделей: {error}";
                    return;
                }
            );

            // Загружаем список водителей
            var driversQuery = new GetAllDriversQuery(1, 1000);
            var driversResult = await _mediator.Send(driversQuery);

            driversResult.Match(
                drivers =>
                {
                    Drivers = drivers.Items.Select(d => new DriverViewModel
                    {
                        Id = d.Id,
                        FirstName = d.FirstName,
                        LastName = d.LastName,
                        Salary = d.Salary,
                        EnterpriseId = d.EnterpriseId
                    }).ToList();
                },
                error =>
                {
                    ErrorMessage = $"Ошибка загрузки водителей: {error}";
                    return;
                }
            );

            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при загрузке данных: {ex.Message}";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var command = new UpdateVehicleCommand(
                Vehicle.Id,
                Vehicle.Name,
                Vehicle.Price,
                Vehicle.MileageInKilometers,
                Vehicle.Color,
                Vehicle.RegistrationNumber,
                Vehicle.BrandModelId,
                Vehicle.EnterpriseId,
                Vehicle.ActiveDriverId,
                Vehicle.PurchaseDate.HasValue ? new DateTimeOffset(Vehicle.PurchaseDate.Value, TimeSpan.Zero) : null
            );

            var result = await _mediator.Send(command);

            result.Match(
                _ => { },
                error =>
                {
                    ErrorMessage = error.ToString();
                }
            );

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                return Page();
            }

            return RedirectToPage("/Manager/Vehicles", new { success = "Автомобиль успешно обновлен" });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при обновлении автомобиля: {ex.Message}";
            return Page();
        }
    }
}

public class VehicleEditViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double MileageInKilometers { get; set; }
    public string Color { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public int BrandModelId { get; set; }
    public int EnterpriseId { get; set; }
    public int? ActiveDriverId { get; set; }
    public DateTime? PurchaseDate { get; set; }
}

public class BrandModelViewModel
{
    public int Id { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public string TransportType { get; set; } = string.Empty;
}

public class DriverViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public int EnterpriseId { get; set; }
}