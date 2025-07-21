using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Autopark.Infrastructure.Database.Services;
using Autopark.UseCases.Enterprise.Commands.Update;
using Autopark.UseCases.Enterprise.Queries.GetById;
using MediatR;
using System.Globalization;

namespace Autopark.Web.Pages.Manager;

public class EnterpriseEditModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ITimeZoneService _timeZoneService;

    public EnterpriseEditModel(IMediator mediator, ITimeZoneService timeZoneService)
    {
        _mediator = mediator;
        _timeZoneService = timeZoneService;
    }

    [BindProperty]
    public EnterpriseEditViewModel Enterprise { get; set; } = new();

    public IEnumerable<TimeZoneInfo> AvailableTimeZones { get; set; } = Enumerable.Empty<TimeZoneInfo>();

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Получаем список доступных таймзон
        AvailableTimeZones = _timeZoneService.GetAvailableTimeZones();

        // Загружаем данные предприятия
        var enterpriseQuery = new GetByIdEnterpriseQuery(id);
        var enterpriseResult = await _mediator.Send(enterpriseQuery);

        enterpriseResult.Match(
            enterprise =>
            {
                Enterprise = new EnterpriseEditViewModel
                {
                    Id = enterprise.Id,
                    Name = enterprise.Name,
                    Address = enterprise.Address,
                    TimeZoneId = enterprise.TimeZoneId
                };
            },
            error =>
            {
                ErrorMessage = $"Предприятие не найдено: {error.Message}";
                return;
            }
        );

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            AvailableTimeZones = _timeZoneService.GetAvailableTimeZones();
            return Page();
        }

        try
        {
            var command = new UpdateEnterpriseCommand(
                Enterprise.Id,
                Enterprise.Name,
                Enterprise.Address,
                Enterprise.TimeZoneId
            );

            var result = await _mediator.Send(command);

            result.Match(
                _ => { },
                error =>
                {
                    ErrorMessage = error.ToString();
                    AvailableTimeZones = _timeZoneService.GetAvailableTimeZones();
                }
            );

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                return Page();
            }

            return RedirectToPage("/Manager/Enterprises", new { success = "Предприятие успешно обновлено" });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при обновлении предприятия: {ex.Message}";
            AvailableTimeZones = _timeZoneService.GetAvailableTimeZones();
            return Page();
        }
    }
}

public class EnterpriseEditViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? TimeZoneId { get; set; }
}