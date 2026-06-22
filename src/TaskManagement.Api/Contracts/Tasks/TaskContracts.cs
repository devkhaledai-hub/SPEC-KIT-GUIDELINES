using DomainTaskStatus = TaskManagement.Api.Domain.TaskStatus;
namespace TaskManagement.Api.Contracts.Tasks;
public sealed record CreateTaskRequest(string? Title, string? Description, DateOnly? DueDate, string? AssigneeId);
public sealed record UpdateTaskRequest(string? Title, string? Description, DateOnly? DueDate, string? AssigneeId, long Version);
public sealed record UpdateStatusRequest(DomainTaskStatus Status, long Version);
public sealed record TaskResponse(Guid Id,string TeamId,string Title,string? Description,DateOnly? DueDate,string CreatorId,string AssigneeId,DomainTaskStatus Status,DateTimeOffset CreatedAt,DateTimeOffset UpdatedAt,long Version)
{ public static TaskResponse From(Domain.TaskItem t) => new(t.Id,t.TeamId,t.Title,t.Description,t.DueDate,t.CreatorId,t.AssigneeId,t.Status,t.CreatedAt,t.UpdatedAt,t.Version); }
