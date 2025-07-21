using Microsoft.AspNetCore.Mvc.RazorPages;
using MediatR;
using Autopark.UseCases.Enterprise.Queries.GetAll;
using Autopark.UseCases.Manager.Queries.GetById;
using Autopark.Infrastructure.Database.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LanguageExt;

namespace Autopark.Web.Pages.Manager;

[Authorize(Roles = "Manager")]
public class EnterprisesModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;

    public EnterprisesModel(IMediator mediator, ICurrentUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public string CurrentManagerName { get; set; } = string.Empty;
    public List<EnterprisesResponse> Enterprises { get; set; } = new();
    public int ActiveEnterprisesCount { get; set; }
    public int TotalVehiclesCount { get; set; }
    public int TotalDriversCount { get; set; }
    public string SuccessMessage { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;

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

            // Получаем все предприятия (в реальном приложении здесь должна быть фильтрация по менеджеру)
            var enterprisesQuery = new GetAllEnterprisesQuery();
            var enterprisesResult = await _mediator.Send(enterprisesQuery);

            enterprisesResult.Match(
                enterprises =>
                {
                    Enterprises = enterprises.Items.ToList();
                    ActiveEnterprisesCount = Enterprises.Count; // В реальном приложении нужно проверять статус
                    TotalVehiclesCount = Enterprises.Sum(e => e.VehicleIds.Length);
                    TotalDriversCount = Enterprises.Sum(e => e.DriverIds.Length);
                },
                error =>
                {
                    ErrorMessage = "Ошибка при загрузке предприятий.";
                    Enterprises = new List<EnterprisesResponse>();
                }
            );

            // Проверяем параметры для отображения сообщений
            if (Request.Query.ContainsKey("success"))
            {
                SuccessMessage = Request.Query["success"].ToString();
            }

            if (Request.Query.ContainsKey("error"))
            {
                ErrorMessage = Request.Query["error"].ToString();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Произошла ошибка при загрузке данных.";
            Console.WriteLine($"Ошибка при загрузке предприятий: {ex.Message}");
        }
    }
}