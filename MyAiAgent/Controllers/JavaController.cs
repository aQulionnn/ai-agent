using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace MyAiAgent.Controllers;

[Route("api/java")]
[ApiController]
public class JavaController(Kernel kernel) : ControllerBase
{
    private readonly Kernel _kernel = kernel;

    [HttpPost("roadmap")]
    public async Task<IActionResult> GenerateRoadmap([FromBody] GenerateRoadmapRequest request)
    {
        var prompts = _kernel.Plugins["JavaPlugins"];
        var response = await _kernel.InvokeAsync<string>(prompts["GenerateRoadmap"], new() {{ "input", request.Input }});
        return Ok(response);
    }

    [HttpPost("roadmap/structured")]
    public async Task<IActionResult> GenerateRoadmapFromTopics()
    {
        var prompts = _kernel.Plugins["JavaPlugins"];
        var response = await _kernel.InvokeAsync<string>(prompts["GenerateRoadmapFromTopics"]);
        return Ok(response);
    }

    [HttpPost("topic")]
    public async Task<IActionResult> SuggestTopic()
    {
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        
        var history = new ChatHistory();
        history.AddUserMessage("Based on the user's learned topics, suggest a topic");
        
        var openAiPromptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
        
        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAiPromptExecutionSettings,
            kernel: _kernel);

        return Ok(result);
    }
}

public record GenerateRoadmapRequest(string Input);