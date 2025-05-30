using Autopark.UseCases.Enterprise.Commands.Create;
using Autopark.UseCases.Enterprise.Commands.Delete;
using Autopark.UseCases.Enterprise.Commands.Update;
using Autopark.UseCases.Enterprise.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnterpriseController : ControllerBase
{
    private readonly IMediator _mediatr;

    public EnterpriseController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<EnterprisesResponse>>> GetAll()
    {
        var result = await _mediatr.Send(new GetAllEnterprisesQuery(), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateEnterpriseCommand createEnterpriseCommand)
    {
        var result = await _mediatr.Send(createEnterpriseCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => Created(),
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpPut]
    public async Task<ActionResult> Update([FromBody] UpdateEnterpriseCommand updateEnterpriseCommand)
    {
        var result = await _mediatr.Send(updateEnterpriseCommand, HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediatr.Send(new DeleteEnterpriseCommand(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            _ => NoContent(),
            error => Problem(detail: error.Message, statusCode: 400));
    }
}