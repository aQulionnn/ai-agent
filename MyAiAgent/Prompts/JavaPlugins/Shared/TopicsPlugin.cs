using System.ComponentModel;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using MyAiAgent.Data;
using MyAiAgent.Models;

namespace MyAiAgent.Prompts.JavaPlugins.Shared;

public class TopicsPlugin(AppDbContext context)
{
    private readonly AppDbContext _context = context;
    
    [KernelFunction("get_topics")]
    [Description("Gets a list of topics")]
    public async Task<List<Topic>> GetTopics()
    {
        var topics = await _context.Topics.OrderBy(t => t.Order).ToListAsync();
        return topics;
    }
}