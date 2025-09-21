using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using MyAiAgent.Data;
using MyAiAgent.Prompts.JavaPlugins.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Database"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddSingleton(_ =>
// {
//     Env.Load();
//     
//     var kernelBuilder = Kernel.CreateBuilder()
//         .AddAzureOpenAIChatCompletion(
//             Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME")
//                 ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME environment variable is required"),
//             
//             Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
//                 ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT environment variable is required"),
//             
//             Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")
//                 ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY environment variable is required")
//         );
//     
//     var kernel = kernelBuilder.Build();
//     
//     kernel.ImportPluginFromPromptDirectory("Prompts/JavaPlugins");
//     kernel.ImportPluginFromType<TopicsPlugin>();
//     kernel.ImportPluginFromType<LearnedTopicsPlugin>();
//     
//     return kernel;
// });

builder.Services.AddScoped<TopicsPlugin>();
builder.Services.AddScoped<LearnedTopicsPlugin>();

builder.Services.AddScoped<Kernel>(serviceProvider =>
{
    Env.Load();
    
    var kernelBuilder = Kernel.CreateBuilder()
        .AddAzureOpenAIChatCompletion(
            Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME")!,
            Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!,
            Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")!
        );
    
    var kernel = kernelBuilder.Build();
    
    kernel.ImportPluginFromPromptDirectory("Prompts/JavaPlugins");
    
    var topicsPlugin = serviceProvider.GetRequiredService<TopicsPlugin>();
    kernel.ImportPluginFromObject(topicsPlugin, "TopicsPlugin");
    
    var learnedTopicsPlugin = serviceProvider.GetRequiredService<LearnedTopicsPlugin>();
    kernel.ImportPluginFromObject(learnedTopicsPlugin, "LearnedTopicsPlugin");
    
    return kernel;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();