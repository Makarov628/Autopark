using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace Autopark.Web.Pages.Manager;

public class TestModel : PageModel
{
    [BindProperty]
    public string? Test { get; set; }

    public string? Message { get; set; }

    public void OnGet()
    {
        Message = "GET запрос обработан";
    }

    public void OnPost()
    {
        Message = $"POST запрос обработан. Тест: {Test}";
    }
}