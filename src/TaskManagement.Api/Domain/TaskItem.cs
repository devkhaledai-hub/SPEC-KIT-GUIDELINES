namespace TaskManagement.Api.Domain;
public sealed class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string TeamId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateOnly? DueDate { get; set; }
    public required string CreatorId { get; set; }
    public required string AssigneeId { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Backlog;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public long Version { get; set; } = 1;
}
