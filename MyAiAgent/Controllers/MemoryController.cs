using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace MyAiAgent.Controllers;

#pragma warning disable

[Route("api/memories")]
[ApiController]
public class MemoryController(ISemanticTextMemory memory) 
    : ControllerBase
{
    private readonly ISemanticTextMemory _memory = memory;
    
    [HttpPost("save/{userId}")]
    public async Task<IActionResult> SaveFact(string userId, [FromBody] string fact)
    {
        await _memory.SaveInformationAsync(userId, fact, Guid.NewGuid().ToString());
        return Ok("Saved");
    }
    
    [HttpGet("search/{userId}")]
    public async Task<IActionResult> SearchFact(string userId, [FromQuery] string query)
    {
        var results = _memory.SearchAsync(userId, query, limit: 3, minRelevanceScore: 0.8);
        var list = new List<object>();
        await foreach (var item in results)
        {
            list.Add(new { item.Metadata.Id, item.Metadata.Description, item.Relevance });
        }
        return Ok(list);
    }
}