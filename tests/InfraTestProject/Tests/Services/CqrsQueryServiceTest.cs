using Contracts.Services;

using HanyCo.Infra.UI.Services;

using InfraTestProject.Helpers;

using Xunit.Abstractions;

namespace InfraTestProject.Tests.Services;

public sealed class CqrsQueryServiceTest(ITestOutputHelper output, ICqrsQueryService service, IModuleService moduleService)
{
    [Fact]
    public async Task _10_GetAllAsync()
    {
        // Act
        var actual = await service.GetAllAsync();

        // Assert
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task _20_GetByIdAsync()
    {
        // Act
        var actual = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task _30_InsertAsync()
    {
        // Assign
        var sampleQuery = await service.CreateAsync();
        sampleQuery.Name = "Unit Test CQRS Query 1";
        sampleQuery.Module = await moduleService.GetByIdAsync(1);

        // Act
        var insetResult = await service.InsertAsync(sampleQuery);
        var actual = insetResult.Value;

        // Assert
        _ = insetResult.CheckAssertion();
        Assert.True(actual.Id > 0);
    }
}