using TaskManagement.Api.Domain;
namespace TaskManagement.Api.Endpoints;
public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
{ public async Task Invoke(HttpContext context) { try { await next(context); } catch(TaskApiException ex) { var status=ex.Code switch { "TASK_VERSION_CONFLICT"=>StatusCodes.Status409Conflict, "TASK_NOT_FOUND"=>StatusCodes.Status404NotFound, "TASK_ACTION_FORBIDDEN"=>StatusCodes.Status403Forbidden, _=>StatusCodes.Status400BadRequest}; await ApiErrorResults.Error(status,ex.Code,ex.Message,ex.Details).ExecuteAsync(context); } } }
