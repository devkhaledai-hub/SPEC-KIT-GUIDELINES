using System.Security.Claims;
namespace TaskManagement.Api.Authorization;
public interface ICurrentTeamPrincipal { string UserId { get; } string TeamId { get; } bool IsAdministrator { get; } bool IsCurrentMember(string userId); }
public sealed class CurrentTeamPrincipal(IHttpContextAccessor accessor) : ICurrentTeamPrincipal
{
    ClaimsPrincipal Principal => accessor.HttpContext?.User ?? throw new InvalidOperationException("No request principal");
    public string UserId => Principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? Principal.FindFirstValue("sub") ?? throw new InvalidOperationException("Missing subject claim");
    public string TeamId => Principal.FindFirstValue("team_id") ?? throw new InvalidOperationException("Missing team claim");
    public bool IsAdministrator => Principal.IsInRole("Administrator") || Principal.HasClaim("role","Administrator");
    public bool IsCurrentMember(string userId) => userId == UserId || Principal.FindAll("team_member").Any(c=>c.Value==userId);
}
