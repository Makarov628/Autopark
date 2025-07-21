using Microsoft.AspNetCore.Mvc.RazorPages;
using MediatR;
using Autopark.UseCases.Manager.Queries.GetById;
using Autopark.UseCases.Enterprise.Queries.GetAll;
using Autopark.UseCases.Driver.Queries.GetAll;
using Autopark.UseCases.Vehicle.Queries.GetAll;
using Autopark.UseCases.BrandModel.Queries.GetAll;
using Autopark.Infrastructure.Database.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LanguageExt;

namespace Autopark.Web.Pages.Manager;

[Authorize(Roles = "Manager")]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;

    public IndexModel(IMediator mediator, ICurrentUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public string CurrentManagerName { get; set; } = string.Empty;
    public int EnterprisesCount { get; set; }
    public int DriversCount { get; set; }
    public int VehiclesCount { get; set; }
    public int BrandModelsCount { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            // Получаем информацию о текущем менеджере
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int managerId))
            {
                var managerQuery = new GetByIdManagerQuery(managerId);
                var managerResult = await _mediator.Send(managerQuery);

                managerResult.Match(
                    manager => CurrentManagerName = $"{manager.FirstName} {manager.LastName}",
                    error => CurrentManagerName = "Менеджер"
                );
            }

            // Получаем статистику предприятий
            var enterprisesQuery = new GetAllEnterprisesQuery();
            var enterprisesResult = await _mediator.Send(enterprisesQuery);
            enterprisesResult.Match(
                enterprises => EnterprisesCount = enterprises.Items.Count,
                error => EnterprisesCount = 0
            );

            // Получаем статистику водителей
            var driversQuery = new GetAllDriversQuery();
            var driversResult = await _mediator.Send(driversQuery);
            driversResult.Match(
                drivers => DriversCount = drivers.Items.Count,
                error => DriversCount = 0
            );

            // Получаем статистику транспортных средств
            var vehiclesQuery = new GetAllVehiclesQuery();
            var vehiclesResult = await _mediator.Send(vehiclesQuery);
            vehiclesResult.Match(
                vehicles => VehiclesCount = vehicles.Items.Count,
                error => VehiclesCount = 0
            );

            // Получаем статистику моделей
            var brandModelsQuery = new GetAllBrandModelQuery();
            var brandModelsResult = await _mediator.Send(brandModelsQuery);
            brandModelsResult.Match(
                brandModels => BrandModelsCount = brandModels.Count,
                error => BrandModelsCount = 0
            );
        }
        catch (Exception ex)
        {
            // В реальном приложении здесь должна быть обработка ошибок
            Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
        }
    }
}