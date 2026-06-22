namespace TaskManagement.Api.Contracts;
public sealed record ErrorResponse(string Code, string Message, object Details);
public static class ErrorCodes { public const string TitleRequired="TASK_TITLE_REQUIRED", AssigneeInvalid="TASK_ASSIGNEE_INVALID", NotFound="TASK_NOT_FOUND", Forbidden="TASK_ACTION_FORBIDDEN", VersionConflict="TASK_VERSION_CONFLICT"; }
