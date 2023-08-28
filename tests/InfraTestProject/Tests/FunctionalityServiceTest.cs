using Contracts.Services;
using Contracts.ViewModels;

using Library.BusinessServices;

namespace InfraTestProject.Tests;

public sealed class FunctionalityServiceTest(IFunctionalityService service, IFunctionalityCodeService codeService)
{
    private readonly IFunctionalityCodeService _codeService = codeService;
    private readonly IFunctionalityService _service = service;

    [Fact]
    public async Task _10_GenerateModelTest()
    {
        // Assign
        var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var model = CreateModel();

        // Act
        var actual = await this._service.GenerateViewModelAsync(model);

        // Assert
        if (!actual.IsSucceed)
        {
            Assert.Fail(actual.Message ?? $"{nameof(this._service.GenerateViewModelAsync)} failed.");
        }
    }

    [Fact]
    public async void _20_GenerateCodeTest()
    {
        // Assign
        var model = await this._service.GenerateViewModelAsync(CreateModel());

        // Act
        var actual = await this._codeService.GenerateCodesAsync(model!);

        // Assert
        if (!actual.IsSucceed)
        {
            Assert.Fail(actual.ToString());
        }

        foreach (var code in actual.Value)
        {
            if (code?.props().Category == null)
            {
                Assert.Fail($"Code: `{code}` has no Category");
            }
        }
    }

    [Fact]
    public async void _30_SaveModelTest()
    {
        // Assign
        var model = await this._service.GenerateViewModelAsync(CreateModel());

        // Act
        var result = await this._service.SaveViewModelAsync(model!);

        // Assert
        Assert.True(result);
    }

    private static FunctionalityViewModel CreateModel()
    {
        var model = new FunctionalityViewModel
        {
            SourceDto = new(-1, "PersonDTO") { Module = new(1, "Module") },
            Name = "TestFunctionality",
            NameSpace = "CodeGen.UnitTests",
        };
        _ = model.SourceDto.Properties.AddRange(new PropertyViewModel[]
        {
                new("Id", HanyCo.Infra.Internals.Data.DataSources.PropertyType.Long),
                new("Name", HanyCo.Infra.Internals.Data.DataSources.PropertyType.String),
                new("Age", HanyCo.Infra.Internals.Data.DataSources.PropertyType.Interger),
        });
        return model;
    }
}