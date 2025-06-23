using Autopark.UseCases.Vehicle.Commands.Create;
using Autopark.UseCases.Vehicle.Commands.Delete;
using Autopark.UseCases.Vehicle.Commands.Update;
using Autopark.UseCases.Vehicle.Queries.GetAll;
using Autopark.UseCases.Vehicle.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Autopark.UseCases.Common.Exceptions;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediatr;

    public VehiclesController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Driver")]
    public async Task<ActionResult<List<VehiclesResponse>>> GetAll()
    {
        var result = await _mediatr.Send(new GetAllVehiclesQuery(), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 500));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager,Driver")]
    public async Task<ActionResult<VehicleResponse>> GetById(int id)
    {
        var result = await _mediatr.Send(new GetByIdVehicleQuery(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => NotFound(error.Message));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> Create([FromBody] CreateVehicleCommand createVehicleCommand)
    {
        var result = await _mediatr.Send(createVehicleCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => StatusCode(201),
            error => BadRequest(error.Message));
    }

    [HttpPut]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> Update([FromBody] UpdateVehicleCommand updateVehicleCommand)
    {
        var result = await _mediatr.Send(updateVehicleCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => Ok(),
            error => NotFound(error.Message));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediatr.Send(new DeleteVehicleCommand(id), HttpContext.RequestAborted);
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