using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using MyAiAgent.Data;
using MyAiAgent.Prompts.JavaPlugins.GenerateRoadmapFromLearningTopics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Database"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(sp =>
{
    Env.Load();
    
    var kernelBuilder = Kernel.CreateBuilder()
        .AddAzureOpenAIChatCompletion(
            Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME")
                ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME environment variable is required"),
            
            Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
                ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT environment variable is required"),
            
            Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")
                ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY environment variable is required")
        );
    
    var kernel = kernelBuilder.Build();
    
    kernel.ImportPluginFromPromptDirectory("Prompts/JavaPlugins");
    kernel.ImportPluginFromType<RoadmapPlugin>();
    
    return kernel;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();