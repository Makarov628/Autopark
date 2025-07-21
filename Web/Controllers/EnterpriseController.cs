using Autopark.UseCases.Enterprise.Commands.Create;
using Autopark.UseCases.Enterprise.Commands.Delete;
using Autopark.UseCases.Enterprise.Commands.Update;
using Autopark.UseCases.Enterprise.Queries.GetAll;
using Autopark.UseCases.Enterprise.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Autopark.UseCases.Common.Exceptions;
using Autopark.UseCases.Common.Models;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnterprisesController : ControllerBase
{
    private readonly IMediator _mediatr;

    public EnterprisesController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<PagedResult<EnterprisesResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null)
    {
        var result = await _mediatr.Send(new GetAllEnterprisesQuery(page, pageSize, sortBy, sortDirection), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 500));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<EnterpriseResponse>> GetById(int id)
    {
        var result = await _mediatr.Send(new GetByIdEnterpriseQuery(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => NotFound(error.Message));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Create([FromBody] CreateEnterpriseCommand createEnterpriseCommand)
    {
        var result = await _mediatr.Send(createEnterpriseCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => StatusCode(201),
            error => BadRequest(error.Message));
    }

    [HttpPut]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult> Update([FromBody] UpdateEnterpriseCommand updateEnterpriseCommand)
    {
        var result = await _mediatr.Send(updateEnterpriseCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => Ok(),
            error => NotFound(error.Message));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var result = await _mediatr.Send(new DeleteEnterpriseCommand(id), HttpContext.RequestAborted);
            return result.Match<ActionResult>(
                _ => NoContent(),
                error => NotFound(error.Message));
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
    }
}