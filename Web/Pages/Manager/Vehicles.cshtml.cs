using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Autopark.UseCases.Vehicle.Queries.GetAll;
using Autopark.UseCases.Enterprise.Queries.GetAll;
using MediatR;
using Autopark.Infrastructure.Database.Services;

namespace Autopark.Web.Pages.Manager;

public class VehiclesModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ITimeZoneService _timeZoneService;

    public VehiclesModel(IMediator mediator, ITimeZoneService timeZoneService)
    {
        _mediator = mediator;
        _timeZoneService = timeZoneService;
    }

    public List<VehicleViewModel> Vehicles { get; set; } = new();
    public List<EnterpriseViewModel> Enterprises { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public string CurrentManagerName { get; set; } = "Менеджер";

    // Фильтры
    public int? SelectedEnterpriseId { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }

    public async Task<IActionResult> OnGetAsync(int? enterpriseId = null, string? search = null, string? sortBy = null)
    {
        try
        {
            SelectedEnterpriseId = enterpriseId;
            SearchTerm = search;
            SortBy = sortBy;

            // Получаем список предприятий
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
                    ErrorMessage = error.ToString();
                    Enterprises = new List<EnterpriseViewModel>();
                }
            );

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                return Page();
            }

            // Получаем список автомобилей
            var vehiclesQuery = new GetAllVehiclesQuery(1, 1000, sortBy, "asc", enterpriseId);
            var vehiclesResult = await _mediator.Send(vehiclesQuery);

            vehiclesResult.Match(
                vehicles =>
                {
                    Vehicles = vehicles.Items.Select(v => new VehicleViewModel
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Price = v.Price,
                        MileageInKilometers = v.MileageInKilometers,
                        Color = "Цвет", // TODO: Добавить цвет в API
                        RegistrationNumber = v.RegistrationNumber,
                        BrandModelId = v.BrandModelId,
                        EnterpriseId = v.EnterpriseId,
                        ActiveDriverId = v.ActiveDriverId,
                        PurchaseDate = v.PurchaseDate,
                        EnterpriseName = Enterprises.FirstOrDefault(e => e.Id == v.EnterpriseId)?.Name ?? "Неизвестно",
                        EnterpriseTimeZoneId = Enterprises.FirstOrDefault(e => e.Id == v.EnterpriseId)?.TimeZoneId
                    }).ToList();
                },
                error =>
                {
                    ErrorMessage = error.ToString();
                    Vehicles = new List<VehicleViewModel>();
                }
            );

            // Применяем поиск
            if (!string.IsNullOrEmpty(search))
            {
                Vehicles = Vehicles.Where(v =>
                    v.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    v.RegistrationNumber.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при загрузке данных: {ex.Message}";
            return Page();
        }
    }
}

public class VehicleViewModel
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
    public DateTimeOffset? PurchaseDate { get; set; }
    public string EnterpriseName { get; set; } = string.Empty;
    public string? EnterpriseTimeZoneId { get; set; }
}

public class EnterpriseViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? TimeZoneId { get; set; }
}