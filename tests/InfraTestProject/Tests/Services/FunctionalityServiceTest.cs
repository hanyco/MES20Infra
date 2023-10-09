using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.UI.ViewModels;

using Library.BusinessServices;
using Library.Coding;

namespace InfraTestProject.Tests.Services;

public sealed class FunctionalityServiceTest(IFunctionalityService service, IFunctionalityCodeService codeService)
{
    [Fact]
    public async Task _10_GenerateModelTest()
    {
        // Assign
        var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var model = CreateModel();

        // Act
        var actual = await service.GenerateViewModelAsync(model);

        // Assert
        if (!actual.IsSucceed)
        {
            Assert.Fail(actual.Message ?? $"{nameof(service.GenerateViewModelAsync)} failed.");
        }
    }

    [Fact]
    [Trait("Category", "__ActiveTest")]
    public async void _20_GenerateCode()
    {
        // Assign
        var model = await service.GenerateViewModelAsync(CreateModel());

        // Act
        var actual = await codeService.GenerateCodesAsync(model!);

        // Assert
        if (!actual.IsSucceed)
        {
            Assert.Fail(actual.ToString());
        }

        var codes = actual.Value;
        foreach (var code in codes)
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
        var model = await service.GenerateViewModelAsync(CreateModel());

        // Act
        var result = await service.SaveViewModelAsync(model!);

        // Assert
        Assert.True(result);
    }

    private static FunctionalityViewModel CreateModel()
    {
        var personTable = new DbTableViewModel("Person", -1, "dbo");
        var model = new FunctionalityViewModel
        {
            SourceDto = new(-1, "PersonDto") { Module = new(1, "Module"), DbObject = personTable, NameSpace = "CodeGen.UnitTests.Dtos" },
            Name = "PersonDto"
        }.With(x => x.SourceDto.NameSpace = "CodeGen.UnitTests");
        _ = model.SourceDto.Properties.AddRange(new PropertyViewModel[]
        {
                new("Id", HanyCo.Infra.Internals.Data.DataSources.PropertyType.Long),
                new("Name", HanyCo.Infra.Internals.Data.DataSources.PropertyType.String),
                new("Age", HanyCo.Infra.Internals.Data.DataSources.PropertyType.Integer),
        });
        return model;
    }
}