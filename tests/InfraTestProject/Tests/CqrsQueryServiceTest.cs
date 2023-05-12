using HanyCo.Infra.UI.Services;

using InfraTestProject.Helpers;

using Xunit.Abstractions;

namespace InfraTestProject.Tests;

public sealed class CqrsQueryServiceTest
{
    private readonly ITestOutputHelper _output;
    private readonly ICqrsQueryService _service;
    private readonly IModuleService _moduleService;

    public CqrsQueryServiceTest(ITestOutputHelper output, ICqrsQueryService service, IModuleService moduleService)
    {
        this._output = output;
        this._service = service;
        this._moduleService = moduleService;
    }

    [Fact]
    public async Task _30_InsertAsync()
    {
        // Assign
        var sampleQuery = await _service.CreateAsync();
        sampleQuery.Name = "Unit Test CQRS Query 1";
        sampleQuery.Module = await _moduleService.GetByIdAsync(1);


        // Act
        var insetResult = await this._service.InsertAsync(sampleQuery);
        var actual = insetResult.Value;

        // Assert
        insetResult.CheckAssertion();
        Assert.True(actual.Id > 0);
    }

    [Fact]
    public async Task _10_GetByIdAsync()
    {
        // Act
        var actual = await this._service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(actual);
    }
}