namespace MyAiAgent.Models;

public class Topic
{
    public int Id { get; init; }
    public string Title  { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public int Order { get; set; }
}