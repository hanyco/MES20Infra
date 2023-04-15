using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.UI.Services;

using Library.Data.SqlServer.Dynamics;

namespace InfraTestProject.Tests;

public sealed class FunctionalityServiceTest
{
    private readonly IFunctionalityCodeService _codeService;
    private readonly IFunctionalityService _service;

    public FunctionalityServiceTest(IFunctionalityService service, IFunctionalityCodeService codeService)
    {
        this._service = service;
        this._codeService = codeService;
    }

    [Fact]
    [Trait("_Active Tests", "Current")]
    public async void GenerateCodeTest()
    {
        // Assign
        var cts = new CancellationTokenSource();
        _ = CreateModel();

        // Act
        var actual =await this._codeService.GenerateCodesAsync(null!, token: cts.Token);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    [Trait("_Active Tests", "Current")]
    public async Task GenerateModelTest()
    {
        // Assign
        var tokenSource = new CancellationTokenSource();
        var model = CreateModel();

        // Act
        var actual = await this._service.GenerateViewModelAsync(model, tokenSource.Token);

        // Assert
        Assert.True(actual);
    }

    private static FunctionalityViewModel CreateModel()
    {
        const string CS = "InMemoryConnectionString";
        var model = new FunctionalityViewModel()
        {
            DbTable = new(null!, "Person", "dbo", CS),
            DbObject = new("Person") { Id = 1 },
            Name = "TestFunctionality",
            NameSpace = "CodeGen.UnitTest",
            ModuleId = 1,
        };
        model.DbTable.Columns = new(new Column[]
        {
                new(model.DbTable, "Name", CS) { DataType = "nvarchar", MaxLength = 50, },
                new(model.DbTable, "Age", CS) { DataType = "int" },
        });
        return model;
    }
}