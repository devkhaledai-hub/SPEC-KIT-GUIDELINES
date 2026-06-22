namespace TaskManagement.Api.Tests.Integration;
public sealed class TaskStatusContractTests { [Fact] public void Status_contract_reserves_conflict_code() => Assert.Equal("TASK_VERSION_CONFLICT",TaskManagement.Api.Contracts.ErrorCodes.VersionConflict); }
