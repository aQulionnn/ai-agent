using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using MyAiAgent.Services;
using Refit;

namespace MyAiAgent.Controllers;

[Route("api/java")]
[ApiController]
public class JavaController(Kernel kernel) 
    : ControllerBase
{
    private readonly Kernel _kernel = kernel;
    private readonly IPlatonusService _platonusService = RestService.For<IPlatonusService>("https://aiu.c-platonus.kz");

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

    [HttpPost("courses")]
    public async Task<IActionResult> SuggestCourses([FromBody] SuggestCoursesRequest request)
    {
        var loginResponse = await _platonusService.Login(new LoginRequest
        {
            AuthForDeductedStudentsAndGraduates = false,
            IcNumber = request.Iin,
            Iin = request.Iin, 
            Login = null,
            Password = request.Password,
        }); 
        
        var prompts = _kernel.Plugins["JavaPlugins"];
        var response = await _kernel.InvokeAsync<string>(prompts["SuggestCourses"], new()
        {
            { "sid", loginResponse.Sid },
            { "token", loginResponse.AuthToken }
        });
        
        return Ok(response);
    }
}

public record GenerateRoadmapRequest(string Input);
public record SuggestCoursesRequest(string Iin, string Password);
public record LoginResponse(
    [property: JsonPropertyName("auth_token")] string AuthToken,
    [property: JsonPropertyName("login_status")] string LoginStatus,
    [property: JsonPropertyName("sid")] string Sid
);
