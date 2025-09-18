using DotNetEnv;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Plugins.Core;

Env.Load(Path.Combine(AppContext.BaseDirectory, @"..\..\..\.env"));
    
var builder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") 
            ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME environment variable is required"),

        Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
            ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT environment variable is required"),

        Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")
            ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY environment variable is required")
    );

var kernel = builder.Build();

kernel.ImportPluginFromType<ConversationSummaryPlugin>();

var prompts = kernel.ImportPluginFromPromptDirectory("Prompts/JavaPlugins");

ChatHistory history = [];

string input = @"Я хочу научиться программировать на Java";

var response = await kernel.InvokeAsync<string>(prompts["GetRoadmap"], new() {{ "input", input }});

Console.WriteLine(response);

history.AddUserMessage(input);
history.AddAssistantMessage(response ?? "");

