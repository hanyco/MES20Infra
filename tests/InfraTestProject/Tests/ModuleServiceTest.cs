using HanyCo.Infra.UI.Services;

namespace InfraTestProject.Tests;

public sealed class ModuleServiceTest : ServiceTestBase<IModuleService, ModuleServiceFixture>
{
    public ModuleServiceTest(ModuleServiceFixture fixture) : base(fixture)
    {
    }

    [Fact]
    [Trait(nameof(ModuleServiceTest), "CRUD Test")]
    public async Task _10_GetAllAsync()
    {
        var modules = await this.Service.GetAllAsync();
        Assert.NotNull(modules);
        Assert.True(modules.Any());
    }

    [Fact]
    [Trait(nameof(ModuleServiceTest), "CRUD Test")]
    public async Task _10_GetByIdAsync()
    {
        var module = await this.Service.GetByIdAsync(1);
        Assert.NotNull(module);
    }
}