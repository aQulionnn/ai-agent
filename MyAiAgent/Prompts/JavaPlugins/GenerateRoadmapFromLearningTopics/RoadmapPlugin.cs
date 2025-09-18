using System.ComponentModel;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using MyAiAgent.Data;

namespace MyAiAgent.Prompts.JavaPlugins.GenerateRoadmapFromLearningTopics;

public class RoadmapPlugin
{
    [KernelFunction, Description("Get a list of learning topics")]
    public static async Task<string> GetLearningTopics(AppDbContext context)
    {
        var topics = await context.LearningTopics.OrderBy(t => t.Order).ToListAsync();
        var sb = new StringBuilder("Your personal Java learning Plan:\n");
        
        foreach (var topic in topics) 
            sb.AppendLine($"{topic.Order}. {topic.Title} - {topic.Description}");
        
        return sb.ToString();
    }
}