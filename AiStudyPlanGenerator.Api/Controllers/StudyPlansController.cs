using AiStudyPlanGenerator.Api.Models;
using AiStudyPlanGenerator.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiStudyPlanGenerator.Api.Controllers;

[ApiController]
[Route("api/study-plans")]
public sealed class StudyPlansController(StudyPlanService studyPlanService) : ControllerBase
{
    [HttpPost("generate")]
    [ProducesResponseType(typeof(StudyPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudyPlanResponse>> Generate(
        [FromBody] StudyPlanRequest request,
        CancellationToken cancellationToken)
    {
        var result = await studyPlanService.GenerateAsync(
            request,
            cancellationToken);

        return Ok(result);
    }
}