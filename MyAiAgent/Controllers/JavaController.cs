using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace MyAiAgent.Controllers;

[Route("api/java")]
[ApiController]
public class JavaController(Kernel kernel) 
    : ControllerBase
{
    private readonly Kernel _kernel = kernel;
    private readonly KernelPlugin _prompts = kernel.ImportPluginFromPromptDirectory("Prompts/JavaPlugins");

    [HttpPost("roadmap")]
    public async Task<IActionResult> GenerateRoadmap([FromBody] RoadmapRequest request)
    {
        var response = await _kernel.InvokeAsync<string>(_prompts["GetRoadmap"], new() {{ "input", request.Input }});
        
        return Ok(response);
    }
}

public record RoadmapRequest(string Input);