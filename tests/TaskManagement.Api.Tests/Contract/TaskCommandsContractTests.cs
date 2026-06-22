namespace TaskManagement.Api.Tests.Integration;
public sealed class TaskCommandsContractTests(TaskManagementWebApplicationFactory factory) : IClassFixture<TaskManagementWebApplicationFactory>
{ [Fact] public async Task Create_requires_authentication() => Assert.Equal(System.Net.HttpStatusCode.Unauthorized,(await factory.CreateClient().PostAsync("/api/tasks",null)).StatusCode); }
