using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.UI.ViewModels;

namespace InfraTestProject.Tests.Services;

public sealed class FunctionalityServiceTest(IFunctionalityService service, IFunctionalityCodeService codeService)
{
    [Fact]
    [Trait("Category", "__ActiveTest")]
    public async void GenerateCode()
    {
        // Assign
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var model = await service.GenerateViewModelAsync(CreateModel(), tokenSource.Token);

        // Act
        var actual = codeService.GenerateCodes(model!, new(true, tokenSource.Token));

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
    public async Task GenerateModelTest()
    {
        // Assign
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var model = CreateModel();

        // Act
        var actual = await service.GenerateViewModelAsync(model, tokenSource.Token);

        // Assert
        if (!actual.IsSucceed)
        {
            Assert.Fail(actual.Message ?? $"{nameof(service.GenerateViewModelAsync)} failed.");
        }
    }

    [Fact]
    public async void InsertViewModel()
    {
        // Assign
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3600));
        var model = await service.GenerateViewModelAsync(CreateModel(), tokenSource.Token);

        // Act
        var result = await service.InsertAsync(model!, token: tokenSource.Token);

        // Assert
        Assert.True(result);
    }

    private static FunctionalityViewModel CreateModel()
    {
        var personTable = new DbTableViewModel("Person", -1, "dbo");
        var model = new FunctionalityViewModel
        {
            SourceDto = new(-1, "PersonDto") { Module = new(1, "Module"), DbObject = personTable, NameSpace = "UnitTests.CodeGen" },
            Name = "PersonDto"
        };
        model.SourceDto.Properties.AddRange(new PropertyViewModel[]
        {
            new("Id", HanyCo.Infra.Internals.Data.DataSources.PropertyType.Long){ DbObject = new DbColumnViewModel("Id", -8){ DbType ="bigint", IsNullable = false }, IsNullable = false },
            new("FirstName", HanyCo.Infra.Internals.Data.DataSources.PropertyType.String){ DbObject = new DbColumnViewModel("FirstName", -2){ DbType ="nvarchar", IsNullable = true }, IsNullable = true },
            new("LastName", HanyCo.Infra.Internals.Data.DataSources.PropertyType.String){ DbObject = new DbColumnViewModel("LastName", -3){ DbType ="nvarchar", IsNullable = false }, IsNullable = false },
            new("DateOfBirth", HanyCo.Infra.Internals.Data.DataSources.PropertyType.DateTime){ DbObject = new DbColumnViewModel("DateOfBirth", -4){ DbType ="nvarchar", IsNullable = false }, IsNullable = false },
            new("Height", HanyCo.Infra.Internals.Data.DataSources.PropertyType.Integer){ DbObject = new DbColumnViewModel("Height", -5){ DbType ="int", IsNullable = true }, IsNullable = true },
            new("IsMarried", HanyCo.Infra.Internals.Data.DataSources.PropertyType.Boolean){ DbObject = new DbColumnViewModel("IsMarried", -6){ DbType ="bit", IsNullable = false }, IsNullable = false },
            new("CountOfChild", HanyCo.Infra.Internals.Data.DataSources.PropertyType.Integer){ DbObject = new DbColumnViewModel("CountOfChild", -7){ DbType ="int", IsNullable = true }, IsNullable = true },
        });
        return model;
    }
}