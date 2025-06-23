using Autopark.UseCases.Driver.Commands.Create;
using Autopark.UseCases.Driver.Commands.Delete;
using Autopark.UseCases.Driver.Commands.Update;
using Autopark.UseCases.Driver.Queries.GetAll;
using Autopark.UseCases.Driver.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Autopark.UseCases.Common.Exceptions;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DriversController : ControllerBase
{
    private readonly IMediator _mediatr;

    public DriversController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Driver")]
    public async Task<ActionResult<List<DriversResponse>>> GetAll()
    {
        var result = await _mediatr.Send(new GetAllDriversQuery(), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 500));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager,Driver")]
    public async Task<ActionResult<DriverResponse>> GetById(int id)
    {
        var result = await _mediatr.Send(new GetByIdDriverQuery(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => NotFound(error.Message));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> Create([FromBody] CreateDriverCommand createDriverCommand)
    {
        var result = await _mediatr.Send(createDriverCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => StatusCode(201),
            error => BadRequest(error.Message));
    }

    [HttpPut]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> Update([FromBody] UpdateDriverCommand updateDriverCommand)
    {
        var result = await _mediatr.Send(updateDriverCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => Ok(),
            error => NotFound(error.Message));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediatr.Send(new DeleteDriverCommand(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error =>
            {
                if (error.Message.Contains("используется") || error.Message.Contains("связан"))
                    return Conflict(error.Message);
                return NotFound(error.Message);
            });
    }
}