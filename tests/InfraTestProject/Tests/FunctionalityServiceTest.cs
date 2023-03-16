using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using InfraTestProject.Fixtures;

using Library.Exceptions.Validations;
using Library.Helpers;

namespace InfraTestProject.Tests;

public class FunctionalityServiceTest : ServiceTestBase<IFunctionalityService, FunctionalityServiceFixture>
{
    public FunctionalityServiceTest(FunctionalityServiceFixture fixture) : base(fixture)
    {
    }

    [Fact]
    [Trait(nameof(FunctionalityServiceTest), "Validation")]
    public async Task _05_GenerateValidationTestAsync()
    {
        var actualExcept1ion = await Assert.ThrowsAsync<ArgumentNullException>(() => this.Service.GenerateAsync(null!));
        Assert.Equal("Value cannot be null. (Parameter 'viewModel')", actualExcept1ion?.Message);
    }

    [Fact]
    [Trait(nameof(FunctionalityServiceTest), "Validation")]
    public async Task _10_GenerateValidationTestAsync()
    {
        var actualExcept1ion = await Assert.ThrowsAsync<NullValueValidationException>(() => this.Service.GenerateAsync(new FunctionalityViewModel()).ThrowOnFailAsync());
        Assert.Equal("RootDto cannot be null", actualExcept1ion?.Message);
    }

    [Fact]
    [Trait(nameof(FunctionalityServiceTest), "Validation")]
    public async Task _20_GenerateValidationTestAsync()
    {
        var model = new FunctionalityViewModel()
        {
            RootDto = new()
            {
                DbObject = new("Person") { Id = 1 }
            }
        };
        var actualExcept1ion = await Assert.ThrowsAsync<NullValueValidationException>(() => this.Service.GenerateAsync(model).ThrowOnFailAsync());
        Assert.Equal("Name cannot be null", actualExcept1ion?.Message);
    }

    [Fact]
    [Trait(nameof(FunctionalityServiceTest), "Validation")]
    public async Task _25_GenerateValidationTestAsync()
    {
        var model = new FunctionalityViewModel()
        {
            RootDto = new()
            {
                DbObject = new("Person") { Id = 1 }
            },
            Name = "Test Functionality"
        };
        var actualExcept1ion = await Assert.ThrowsAsync<NullValueValidationException>(() => this.Service.GenerateAsync(model).ThrowOnFailAsync());
        Assert.Equal("NameSpace cannot be null", actualExcept1ion?.Message);
    }

    [Fact]
    [Trait(nameof(FunctionalityServiceTest), "Validation")]
    public async Task _30_GenerateValidationTestAsync()
    {
        var model = new FunctionalityViewModel()
        {
            RootDto = new()
            {
                DbObject = new("Person") { Id = 1 }
            },
            Name = "Test Functionality",
            NameSpace = "TestNameSpaceForFunctionalityTest"
        };
        var actualExcept1ion = await Assert.ThrowsAsync<ValidationException>(() => this.Service.GenerateAsync(model).ThrowOnFailAsync());
        Assert.Equal("Module is not selected.", actualExcept1ion?.Message);
    }
}