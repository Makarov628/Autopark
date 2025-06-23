using Autopark.UseCases.Manager.Commands.Create;
using Autopark.UseCases.Manager.Commands.Delete;
using Autopark.UseCases.Manager.Queries.GetAll;
using Autopark.UseCases.Manager.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LanguageExt;
using GetAllManager = Autopark.UseCases.Manager.Queries.GetAll;
using GetByIdManager = Autopark.UseCases.Manager.Queries.GetById;
using Microsoft.AspNetCore.Authorization;
using Autopark.UseCases.Common.Exceptions;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ManagersController : ControllerBase
{
    private readonly IMediator _mediatr;

    public ManagersController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<GetAllManager.ManagersResponse>>> GetAll()
    {
        var result = await _mediatr.Send(new GetAllManagerQuery(), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 500));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<GetByIdManager.ManagerResponse>> GetById(int id)
    {
        var result = await _mediatr.Send(new GetByIdManagerQuery(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => NotFound(error.Message));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Create([FromBody] CreateManagerCommand createManagerCommand)
    {
        try
        {
            await _mediatr.Send(createManagerCommand, HttpContext.RequestAborted);
            return StatusCode(201);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediatr.Send(new DeleteManagerCommand(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error =>
            {
                if (error.Message.Contains("используется") || error.Message.Contains("связан"))
                    return Conflict(error.Message);
                return NotFound(error.Message);
            });
    }

    [HttpPut("{userId}/enterprises")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateEnterprises(int userId, [FromBody] List<int> enterpriseIds)
    {
        var result = await _mediatr.Send(new UpdateManagerEnterprisesCommand(userId, enterpriseIds), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => Ok(),
            error => NotFound(error.Message));
    }
}