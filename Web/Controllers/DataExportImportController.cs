using Autopark.UseCases.Enterprise.Commands.ImportData;
using Autopark.UseCases.Enterprise.Queries.ExportData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class DataExportImportController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly IConfiguration _configuration;

    public DataExportImportController(IMediator mediatr, IConfiguration configuration)
    {
        _mediatr = mediatr;
        _configuration = configuration;
    }

    /// <summary>
    /// Экспорт данных предприятия (предприятие, машины, поездки в диапазоне дат)
    /// </summary>
    /// <param name="enterpriseId">ID предприятия</param>
    /// <param name="startDate">Начальная дата для фильтрации поездок (опционально)</param>
    /// <param name="endDate">Конечная дата для фильтрации поездок (опционально)</param>
    /// <param name="format">Формат экспорта: json или csv</param>
    /// <returns>Файл с данными предприятия</returns>
    [HttpGet("export/enterprise/{enterpriseId}")]
    // [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ExportEnterpriseData(
        int enterpriseId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string format = "json")
    {
        var query = new ExportEnterpriseDataQuery(enterpriseId, startDate, endDate, format);
        var result = await _mediatr.Send(query, HttpContext.RequestAborted);

        return result.Match<IActionResult>(
            success =>
            {
                var bytes = Encoding.UTF8.GetBytes(success.Content);
                return File(bytes, success.ContentType);
            },
            error => BadRequest(error.Message));
    }

    /// <summary>
    /// Импорт данных предприятия из JSON файла
    /// </summary>
    /// <param name="file">JSON файл с данными</param>
    /// <param name="updateExisting">Обновлять существующие записи</param>
    /// <returns>Результат импорта</returns>
    [HttpPost("import/json")]
    // [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ImportEnterpriseDataResponse>> ImportFromJson(
        IFormFile file,
        [FromQuery] bool updateExisting = false)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required");
        }

        if (!file.ContentType.Contains("json") && !file.FileName.EndsWith(".json"))
        {
            return BadRequest("Only JSON files are supported");
        }

        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync();

            var geoapifyApiKey = _configuration["Geoapify:ApiKey"];
            var command = new ImportEnterpriseDataCommand(content, "json", updateExisting, geoapifyApiKey);
            var result = await _mediatr.Send(command, HttpContext.RequestAborted);

            return result.Match<ActionResult<ImportEnterpriseDataResponse>>(
                value => Ok(value),
                error => BadRequest(error.Message));
        }
        catch (Exception ex)
        {
            return BadRequest($"Error processing file: {ex.Message}");
        }
    }

    /// <summary>
    /// Импорт данных предприятия из CSV файла
    /// </summary>
    /// <param name="file">CSV файл с данными</param>
    /// <param name="updateExisting">Обновлять существующие записи</param>
    /// <returns>Результат импорта</returns>
    [HttpPost("import/csv")]
    // [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ImportEnterpriseDataResponse>> ImportFromCsv(
        IFormFile file,
        [FromQuery] bool updateExisting = false)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required");
        }

        if (!file.ContentType.Contains("csv") && !file.FileName.EndsWith(".csv"))
        {
            return BadRequest("Only CSV files are supported");
        }

        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync();

            var geoapifyApiKey = _configuration["Geoapify:ApiKey"];
            var command = new ImportEnterpriseDataCommand(content, "csv", updateExisting, geoapifyApiKey);
            var result = await _mediatr.Send(command, HttpContext.RequestAborted);

            return result.Match<ActionResult<ImportEnterpriseDataResponse>>(
                value => Ok(value),
                error => BadRequest(error.Message));
        }
        catch (Exception ex)
        {
            return BadRequest($"Error processing file: {ex.Message}");
        }
    }

    /// <summary>
    /// Импорт данных предприятия из текста (JSON или CSV)
    /// </summary>
    /// <param name="request">Данные для импорта</param>
    /// <returns>Результат импорта</returns>
    [HttpPost("import/text")]
    // [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ImportEnterpriseDataResponse>> ImportFromText(
        [FromBody] ImportTextRequest request)
    {
        if (string.IsNullOrEmpty(request.Content))
        {
            return BadRequest("Content is required");
        }

        if (string.IsNullOrEmpty(request.Format) ||
            (request.Format.ToLower() != "json" && request.Format.ToLower() != "csv"))
        {
            return BadRequest("Format must be 'json' or 'csv'");
        }

        try
        {
            var geoapifyApiKey = _configuration["Geoapify:ApiKey"];
            var command = new ImportEnterpriseDataCommand(
                request.Content,
                request.Format,
                request.UpdateExisting,
                geoapifyApiKey);

            var result = await _mediatr.Send(command, HttpContext.RequestAborted);

            return result.Match<ActionResult<ImportEnterpriseDataResponse>>(
                value => Ok(value),
                error => BadRequest(error.Message));
        }
        catch (Exception ex)
        {
            return BadRequest($"Error processing data: {ex.Message}");
        }
    }
}

public record ImportTextRequest(
    string Content,
    string Format,
    bool UpdateExisting = false
);