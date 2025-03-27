using Autopark.UseCases.Vehicle.Commands.Create;
using Autopark.UseCases.Vehicle.Commands.Delete;
using Autopark.UseCases.Vehicle.Commands.Update;
using Autopark.UseCases.Vehicle.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly IMediator _mediatr;

    public VehicleController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<VehiclesResponse>>> GetAll()
    {
        var result = await _mediatr.Send(new GetAllVehiclesQuery(), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateVehicleCommand createVehicleCommand)
    {
        var result = await _mediatr.Send(createVehicleCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => Created(),
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpPut]
    public async Task<ActionResult> Update([FromBody] UpdateVehicleCommand updateVehicleCommand)
    {
        var result = await _mediatr.Send(updateVehicleCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _mediatr.Send(new DeleteVehicleCommand(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error => Problem(detail: error.Message, statusCode: 400));
    }
}