using Contracts.Services;

using HanyCo.Infra.UI.ViewModels;

namespace InfraTestProject.Tests;

public sealed class ModuleServiceTest
{
    private readonly IModuleService _service;

    public ModuleServiceTest(IModuleService service) => this._service = service;

    [Fact]
    [Trait(nameof(ModuleServiceTest), "CRUD Test")]
    public async Task _10_GetAllAsync()
    {
        var modules = await this._service.GetAllAsync();
        Assert.NotNull(modules);
        Assert.True(modules.Any());
    }

    [Fact]
    [Trait(nameof(ModuleServiceTest), "CRUD Test")]
    public async Task _10_GetByIdAsync()
    {
        var module = await this._service.GetByIdAsync(1);
        Assert.NotNull(module);
    }
}