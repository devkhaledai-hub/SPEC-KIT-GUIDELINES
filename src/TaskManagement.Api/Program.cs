using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Authorization;
using TaskManagement.Api.Data;
using TaskManagement.Api.Domain;
using TaskManagement.Api.Endpoints;

var builder=WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTeamPrincipal,CurrentTeamPrincipal>();
builder.Services.AddScoped<TaskCommandService>();
builder.Services.AddDbContext<TaskManagementDbContext>(o=>o.UseNpgsql(builder.Configuration.GetConnectionString("TaskManagement")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o=>{o.Authority=builder.Configuration["Jwt:Authority"];o.Audience=builder.Configuration["Jwt:Audience"];o.RequireHttpsMetadata=true;o.Events=new JwtBearerEvents { OnChallenge=async c=>{c.HandleResponse();await ApiErrorResults.Error(401,"AUTHENTICATION_REQUIRED","A valid bearer token is required",new{}).ExecuteAsync(c.HttpContext);}, OnForbidden=async c=>await ApiErrorResults.Error(403,"TASK_ACTION_FORBIDDEN","You do not have permission to perform this action",new{}).ExecuteAsync(c.HttpContext)};});
builder.Services.AddAuthorization(o=>o.FallbackPolicy=new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
builder.Services.AddHealthChecks().AddDbContextCheck<TaskManagementDbContext>();
builder.Services.AddOpenApi();
var app=builder.Build();
using (var scope=app.Services.CreateScope()) { await scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>().Database.MigrateAsync(); }
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();app.UseAuthorization();
app.MapOpenApi();app.MapHealthChecks("/health").AllowAnonymous();
app.MapTaskCommandEndpoints();
app.Run();
public partial class Program { }
