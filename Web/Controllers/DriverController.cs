using Autopark.UseCases.Driver.Commands.Create;
using Autopark.UseCases.Driver.Commands.Delete;
using Autopark.UseCases.Driver.Commands.Update;
using Autopark.UseCases.Driver.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriverController : ControllerBase
{
    private readonly IMediator _mediatr;

    public DriverController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<DriversResponse>>> GetAll()
    {
        var result = await _mediatr.Send(new GetAllDriversQuery(), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateDriverCommand createDriverCommand)
    {
        var result = await _mediatr.Send(createDriverCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => Created(),
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpPut]
    public async Task<ActionResult> Update([FromBody] UpdateDriverCommand updateDriverCommand)
    {
        var result = await _mediatr.Send(updateDriverCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediatr.Send(new DeleteDriverCommand(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error => Problem(detail: error.Message, statusCode: 400));
    }
}