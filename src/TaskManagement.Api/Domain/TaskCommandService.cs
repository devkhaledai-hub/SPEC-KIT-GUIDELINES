using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Authorization;
using TaskManagement.Api.Contracts;
using TaskManagement.Api.Contracts.Tasks;
using TaskManagement.Api.Data;
namespace TaskManagement.Api.Domain;
public sealed class TaskCommandService(TaskManagementDbContext db, ICurrentTeamPrincipal current)
{
    static string Title(string? value) => string.IsNullOrWhiteSpace(value) ? throw new TaskApiException(ErrorCodes.TitleRequired,"Title is required",new { field="title" }) : value.Trim();
    void RequireAssignee(string? id) { if (string.IsNullOrWhiteSpace(id) || !current.IsCurrentMember(id)) throw new TaskApiException(ErrorCodes.AssigneeInvalid,"Assignee must be a current team member",new { field="assigneeId" }); }
    public async Task<TaskItem> Create(CreateTaskRequest r, CancellationToken ct) { RequireAssignee(r.AssigneeId); var t=new TaskItem{TeamId=current.TeamId,CreatorId=current.UserId,AssigneeId=r.AssigneeId!,Title=Title(r.Title),Description=r.Description?.Trim(),DueDate=r.DueDate}; db.Tasks.Add(t); await db.SaveChangesAsync(ct); return t; }
    async Task<TaskItem> Find(Guid id,CancellationToken ct) => await db.Tasks.SingleOrDefaultAsync(t=>t.Id==id && t.TeamId==current.TeamId,ct) ?? throw new TaskApiException(ErrorCodes.NotFound,"Task was not found",new { taskId=id });
    public Task<TaskItem> Get(Guid id,CancellationToken ct) => Find(id,ct);
    public async Task<TaskItem> Update(Guid id,UpdateTaskRequest r,CancellationToken ct) { var t=await Find(id,ct); if(!current.IsAdministrator && t.CreatorId!=current.UserId) throw new TaskApiException(ErrorCodes.Forbidden,"You do not have permission to update this task",new{taskId=id}); RequireAssignee(r.AssigneeId); Check(t,r.Version); t.Title=Title(r.Title);t.Description=r.Description?.Trim();t.DueDate=r.DueDate;t.AssigneeId=r.AssigneeId!; return await Save(t,ct); }
    public async Task<TaskItem> Status(Guid id,UpdateStatusRequest r,CancellationToken ct) { var t=await Find(id,ct); if(!current.IsAdministrator && t.CreatorId!=current.UserId && t.AssigneeId!=current.UserId) throw new TaskApiException(ErrorCodes.Forbidden,"You do not have permission to update this task status",new{taskId=id}); Check(t,r.Version); t.Status=r.Status; return await Save(t,ct); }
    static void Check(TaskItem t,long version) { if(version!=t.Version) throw new TaskApiException(ErrorCodes.VersionConflict,"Task changed before your update could be saved; refresh and retry",new{taskId=t.Id}); }
    async Task<TaskItem> Save(TaskItem t,CancellationToken ct) { t.Version++;t.UpdatedAt=DateTimeOffset.UtcNow; try { await db.SaveChangesAsync(ct); } catch(DbUpdateConcurrencyException) { throw new TaskApiException(ErrorCodes.VersionConflict,"Task changed before your update could be saved; refresh and retry",new{taskId=t.Id}); } return t; }
}
public sealed class TaskApiException(string code,string message,object details) : Exception(message) { public string Code { get; }=code; public object Details { get; }=details; }
