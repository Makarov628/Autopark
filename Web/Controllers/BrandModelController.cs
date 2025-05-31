using Autopark.UseCases.BrandModel.Commands.Create;
using Autopark.UseCases.BrandModel.Commands.Delete;
using Autopark.UseCases.BrandModel.Commands.Update;
using Autopark.UseCases.BrandModel.Queries.GetAll;
using Autopark.UseCases.BrandModel.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandModelsController : ControllerBase
{
    private readonly IMediator _mediatr;

    public BrandModelsController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    [HttpGet]
    public async Task<ActionResult<List<BrandModelsResponse>>> GetAll()
    {
        var result = await _mediatr.Send(new GetAllBrandModelQuery(), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BrandModelResponse>> GetById(int id)
    {
        var result = await _mediatr.Send(new GetByIdBrandModelQuery(id), HttpContext.RequestAborted);
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