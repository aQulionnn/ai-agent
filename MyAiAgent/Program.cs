using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using MyAiAgent.Data;
using MyAiAgent.Prompts.JavaPlugins.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Database"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<TopicsPlugin>();
builder.Services.AddScoped<LearnedTopicsPlugin>();
builder.Services.AddScoped<PlatonusPlugin>();

#pragma warning disable

builder.Services.AddScoped<Kernel>(serviceProvider =>
{
    Env.Load();
    
    #pragma warning disable CS0618

    var kernelBuilder = Kernel.CreateBuilder()
        .AddAzureOpenAIChatCompletion(
            Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME")!,
            Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!,
            Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")!
        )
        .AddAzureOpenAITextEmbeddingGeneration(
            Environment.GetEnvironmentVariable("AZURE_OPENAI_EMBEDDING_DEPLOYMENT_NAME")!,
            Environment.GetEnvironmentVariable("AZURE_OPENAI_EMBEDDING_ENDPOINT")!,
            Environment.GetEnvironmentVariable("AZURE_OPENAI_EMBEDDING_API_KEY")!
        );
    
    var kernel = kernelBuilder.Build();
    
    kernel.ImportPluginFromPromptDirectory("Prompts/JavaPlugins");
    
    var topicsPlugin = serviceProvider.GetRequiredService<TopicsPlugin>();
    kernel.ImportPluginFromObject(topicsPlugin, "TopicsPlugin");
    
    var learnedTopicsPlugin = serviceProvider.GetRequiredService<LearnedTopicsPlugin>();
    kernel.ImportPluginFromObject(learnedTopicsPlugin, "LearnedTopicsPlugin");
    
    var platonusPlugin = serviceProvider.GetRequiredService<PlatonusPlugin>();
    kernel.ImportPluginFromObject(platonusPlugin, "PlatonusPlugin");
    
    return kernel;
});

builder.Services.AddScoped<AzureAISearchMemoryStore>(_ => 
    new AzureAISearchMemoryStore(
        Environment.GetEnvironmentVariable("AZURE_AI_SEARCH_ENDPOINT")!,
        Environment.GetEnvironmentVariable("AZURE_AI_SEARCH_API_KEY")!
    ));


builder.Services.AddScoped<MemoryBuilder>(serviceProvider =>
{
    var kernel = serviceProvider.GetRequiredService<Kernel>();
    var memoryStore = serviceProvider.GetRequiredService<AzureAISearchMemoryStore>();

    var memoryBuilder = new MemoryBuilder();
    
    memoryBuilder.WithTextEmbeddingGeneration(kernel.GetRequiredService<ITextEmbeddingGenerationService>());
    memoryBuilder.WithMemoryStore(memoryStore);

    return memoryBuilder;
});

builder.Services.AddScoped<ISemanticTextMemory>(serviceProvider =>
{
    var memoryBuilder = serviceProvider.GetRequiredService<MemoryBuilder>();
    return memoryBuilder.Build();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();