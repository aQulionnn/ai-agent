using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;

namespace MyAiAgent.Controllers;

#pragma warning disable

[Route("api/chats")]
[ApiController]
public class ChatController(Kernel kernel, ISemanticTextMemory memory) 
    : ControllerBase
{
    private readonly Kernel _kernel = kernel;
    private readonly ISemanticTextMemory _memory = memory;
    private static readonly Dictionary<string, string> _history = new();
    
    [HttpPost("{userId}")]
    public async Task<IActionResult> Chat(string userId, [FromBody] string input)
    {
        var memoryPlugin = new TextMemoryPlugin(_memory);
        _kernel.ImportPluginFromObject(memoryPlugin);

        var skPrompt = @"
            ChatBot can have a conversation with you about any topic.
            Information about me, from previous conversations:
            - {{fact1}} {{recall}} {{fact1}}
            - {{fact2}} {{recall}} {{fact2}}
            - {{fact3}} {{recall}} {{fact3}}
            - {{fact4}} {{recall}} {{fact4}}
            - {{fact5}} {{recall}} {{fact5}}

            Chat:
            - {{history}}
            User: {{userInput}}
            ChatBot:";

        var chatFunction = _kernel.CreateFunctionFromPrompt(skPrompt, new OpenAIPromptExecutionSettings
        {
            MaxTokens = 200,
            Temperature = 0.8
        });

        var arguments = new KernelArguments
        {
            ["fact1"] = "What is my name?",
            ["fact2"] = "Where do I live?",
            ["fact3"] = "What is my profession?",
            ["fact4"] = "Which technology do I use for backend?",
            ["fact5"] = "Which technology do I use for frontend?",
            [TextMemoryPlugin.CollectionParam] = userId,
            [TextMemoryPlugin.LimitParam] = "2",
            [TextMemoryPlugin.RelevanceParam] = "0.8",
            ["history"] = _history.GetValueOrDefault(userId, "")
        };

        arguments["userInput"] = input;

        var answer = await chatFunction.InvokeAsync(_kernel, arguments);
        var result = $"Response: {answer}";

        _history[userId] = _history.GetValueOrDefault(userId, "") + result;
        arguments["history"] = _history[userId];

        return Ok(result);
    }
}