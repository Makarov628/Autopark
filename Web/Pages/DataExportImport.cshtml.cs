using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Autopark.Web.Pages;

// [Authorize(Roles = "Admin,Manager")]
public class DataExportImportModel : PageModel
{
    public void OnGet()
    {
    }
}