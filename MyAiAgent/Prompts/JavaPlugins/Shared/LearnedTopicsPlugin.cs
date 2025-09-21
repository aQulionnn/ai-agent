using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using MyAiAgent.Data;
using MyAiAgent.Models;

namespace MyAiAgent.Prompts.JavaPlugins.Shared;

public class LearnedTopicsPlugin(AppDbContext context)
{
    private readonly AppDbContext _context = context;
    
    [KernelFunction("get_learned_topics")]
    [Description("Gets a list of learned topics")]
    public async Task<List<LearnedTopic>> GetLearnedTopics()
    {
        var learnedTopics = await _context.LearnedTopics.ToListAsync();
        return learnedTopics;
    }
}