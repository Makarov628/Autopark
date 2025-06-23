using Autopark.UseCases.BrandModel.Commands.Create;
using Autopark.UseCases.BrandModel.Commands.Delete;
using Autopark.UseCases.BrandModel.Commands.Update;
using Autopark.UseCases.BrandModel.Queries.GetAll;
using Autopark.UseCases.BrandModel.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Autopark.UseCases.Common.Exceptions;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BrandModelsController : ControllerBase
{
    private readonly IMediator _mediatr;

    public BrandModelsController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Driver")]
    public async Task<ActionResult<List<BrandModelsResponse>>> GetAll()
    {
        var result = await _mediatr.Send(new GetAllBrandModelQuery(), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 500));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager,Driver")]
    public async Task<ActionResult<BrandModelResponse>> GetById(int id)
    {
        var result = await _mediatr.Send(new GetByIdBrandModelQuery(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => NotFound(error.Message));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Create([FromBody] CreateBrandModelCommand createBrandModelCommand)
    {
        var result = await _mediatr.Send(createBrandModelCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => StatusCode(201),
            error => BadRequest(error.Message));
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update([FromBody] UpdateBrandModelCommand updateBrandModelCommand)
    {
        var result = await _mediatr.Send(updateBrandModelCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => Ok(),
            error => NotFound(error.Message));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediatr.Send(new DeleteBrandModelCommand(id), HttpContext.RequestAborted);
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