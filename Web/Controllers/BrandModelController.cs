using Autopark.UseCases.BrandModel.Commands.Create;
using Autopark.UseCases.BrandModel.Commands.Delete;
using Autopark.UseCases.BrandModel.Commands.Update;
using Autopark.UseCases.BrandModel.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandModelController : ControllerBase
{
    private readonly IMediator _mediatr;

    public BrandModelController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<BrandModelResponse>>> GetAll()
    {
        var result = await _mediatr.Send(new GetAllBrandModelQuery(), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateBrandModelCommand createBrandModelCommand)
    {
        var result = await _mediatr.Send(createBrandModelCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => Created(),
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpPut]
    public async Task<ActionResult> Update([FromBody] UpdateBrandModelCommand updateBrandModelCommand)
    {
        var result = await _mediatr.Send(updateBrandModelCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediatr.Send(new DeleteBrandModelCommand(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error => Problem(detail: error.Message, statusCode: 400));
    }
}