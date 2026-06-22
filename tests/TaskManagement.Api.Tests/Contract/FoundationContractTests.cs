using System.Net;
namespace TaskManagement.Api.Tests.Integration;
public sealed class FoundationContractTests(TaskManagementWebApplicationFactory factory) : IClassFixture<TaskManagementWebApplicationFactory>
{ [Fact] public async Task Protected_routes_return_shared_401_error() { var r=await factory.CreateClient().GetAsync("/api/tasks/not-a-guid"); Assert.Equal(HttpStatusCode.Unauthorized,r.StatusCode); Assert.Contains("AUTHENTICATION_REQUIRED",await r.Content.ReadAsStringAsync()); } }
