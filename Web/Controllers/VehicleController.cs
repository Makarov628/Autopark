using Autopark.UseCases.Vehicle.Commands.Create;
using Autopark.UseCases.Vehicle.Commands.Delete;
using Autopark.UseCases.Vehicle.Commands.Update;
using Autopark.UseCases.Vehicle.Queries.GetAll;
using Autopark.UseCases.Vehicle.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediatr;

    public VehiclesController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    [HttpGet]
    public async Task<ActionResult<List<VehiclesResponse>>> GetAll()
    {
        var result = await _mediatr.Send(new GetAllVehiclesQuery(), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleResponse>> GetById(int id)
    {
        var result = await _mediatr.Send(new GetByIdVehicleQuery(id), HttpContext.RequestAborted);
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
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediatr.Send(new DeleteVehicleCommand(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error => Problem(detail: error.Message, statusCode: 400));
    }
}