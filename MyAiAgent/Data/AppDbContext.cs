using Microsoft.EntityFrameworkCore;
using MyAiAgent.Models;

namespace MyAiAgent.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Seed();
    }
    
    public DbSet<LearningTopic>  LearningTopics { get; set; }

    private void Seed()
    {
        if (!LearningTopics.Any())
        {
            LearningTopics.AddRange(
                new LearningTopic
                {
                    Id = 1,
                    Title = "Java Basics",
                    Description = "Syntax, variables, data types, operators, and control flow",
                    Order = 1
                },
                new LearningTopic
                {
                    Id = 2,
                    Title = "Object-Oriented Programming",
                    Description = "Classes, objects, inheritance, polymorphism, encapsulation",
                    Order = 2    
                },
                new LearningTopic
                {
                    Id = 3,
                    Title = "Collections and Generics",
                    Description = "Lists, sets, maps, iterators, and generic programming",
                    Order = 3    
                });
        }
        
        SaveChanges();
    }
}