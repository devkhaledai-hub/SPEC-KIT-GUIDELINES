namespace TaskManagement.Api.Tests.Integration;
public sealed class HealthEndpointTests(TaskManagementWebApplicationFactory factory) : IClassFixture<TaskManagementWebApplicationFactory>
{ [Fact] public async Task Health_is_anonymous() => Assert.True((await factory.CreateClient().GetAsync("/health")).IsSuccessStatusCode); }
