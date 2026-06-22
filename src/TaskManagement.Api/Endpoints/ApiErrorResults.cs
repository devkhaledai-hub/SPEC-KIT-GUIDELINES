using TaskManagement.Api.Contracts;
namespace TaskManagement.Api.Endpoints;
public static class ApiErrorResults
{ public static IResult Error(int status,string code,string message,object details) => Results.Json(new ErrorResponse(code,message,details),statusCode:status); }
