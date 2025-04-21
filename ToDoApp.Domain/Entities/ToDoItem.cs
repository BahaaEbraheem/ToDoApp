using System.Text.Json.Serialization;

namespace ToDoApp.Domain.Entities;

public class ToDoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public string Priority { get; set; }
    public string Category { get; set; }


    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

}
