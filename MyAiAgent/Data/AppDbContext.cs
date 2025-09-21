using Microsoft.EntityFrameworkCore;
using MyAiAgent.Models;

namespace MyAiAgent.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Seed();
    }
    
    public DbSet<Topic> Topics { get; set; }
    public DbSet<LearnedTopic> LearnedTopics { get; set; }

    private void Seed()
    {
        if (!Topics.Any())
        {
            Topics.AddRange(
                new Topic
                {
                    Id = 1,
                    Title = "Java Basics",
                    Description = "Syntax, variables, data types, operators, and control flow",
                    Order = 1
                },
                new Topic
                {
                    Id = 2,
                    Title = "Object-Oriented Programming",
                    Description = "Classes, objects, inheritance, polymorphism, encapsulation",
                    Order = 2    
                },
                new Topic
                {
                    Id = 3,
                    Title = "Collections and Generics",
                    Description = "Lists, sets, maps, iterators, and generic programming",
                    Order = 3    
                });
        }

        if (!LearnedTopics.Any())
        {
            LearnedTopics.AddRange(
                new LearnedTopic
                {
                    Id = 1,
                    Title = "Java Basics",
                    Description = "Syntax, variables, data types, operators, and control flow",
                }
            );
        }
        
        SaveChanges();
    }
}