using TaskManagement.Api.Contracts.Tasks;
using TaskManagement.Api.Domain;
namespace TaskManagement.Api.Endpoints;
public static class TaskCommandEndpoints
{
 public static IEndpointRouteBuilder MapTaskCommandEndpoints(this IEndpointRouteBuilder routes)
 {
  var group=routes.MapGroup("/api/tasks").RequireAuthorization();
  group.MapPost("",async(CreateTaskRequest r,TaskCommandService s,CancellationToken ct)=> { var task=await s.Create(r,ct); return Results.Created($"/api/tasks/{task.Id}",TaskResponse.From(task)); });
  group.MapGet("/{id:guid}",async(Guid id,TaskCommandService s,CancellationToken ct)=>Results.Ok(TaskResponse.From(await s.Get(id,ct))));
  group.MapPatch("/{id:guid}",async(Guid id,UpdateTaskRequest r,TaskCommandService s,CancellationToken ct)=>Results.Ok(TaskResponse.From(await s.Update(id,r,ct))));
  group.MapPatch("/{id:guid}/status",async(Guid id,UpdateStatusRequest r,TaskCommandService s,CancellationToken ct)=>Results.Ok(TaskResponse.From(await s.Status(id,r,ct))));
  return routes;
 }
}
