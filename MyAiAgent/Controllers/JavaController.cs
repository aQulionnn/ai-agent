using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using MyAiAgent.Data;

namespace MyAiAgent.Controllers;

[Route("api/java")]
[ApiController]
public class JavaController(Kernel kernel) : ControllerBase
{
    private readonly Kernel _kernel = kernel;

    [HttpPost("roadmap")]
    public async Task<IActionResult> GenerateRoadmap([FromBody] RoadmapRequest request)
    {
        var prompts = _kernel.Plugins["JavaPlugins"];
        var response = await _kernel.InvokeAsync<string>(prompts["GenerateRoadmap"], new() {{ "input", request.Input }});
        return Ok(response);
    }

    [HttpPost("roadmap/structured")]
    public async Task<IActionResult> GenerateRoadmapFromLearningTopics([FromServices] AppDbContext context)
    {
        var prompts = _kernel.Plugins["JavaPlugins"];
        var response = await _kernel.InvokeAsync<string>(prompts["GenerateRoadmapFromLearningTopics"], new() { { "context", context } });
        return Ok(response);
    }
}

public record RoadmapRequest(string Input);