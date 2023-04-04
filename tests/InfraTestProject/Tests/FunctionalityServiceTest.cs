using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.Data.SqlServer.Dynamics;

namespace InfraTestProject.Tests;

public sealed class FunctionalityServiceTest //: FunctionalityServiceTestBase<IFunctionalityService, FunctionalityServiceFixture>
{
    private readonly IFunctionalityCodeService _codeService;
    private readonly IFunctionalityService _service;

    //public FunctionalityServiceTest(FunctionalityServiceFixture fixture, ITestOutputHelper output) : base(fixture, output)
    //{
    //    this._codeService = fixture.GetService<IFunctionalityCodeService>();
    //}
    public FunctionalityServiceTest(IFunctionalityService service, IFunctionalityCodeService codeService)
    {
        this._service = service;
        this._codeService = codeService;
    }

    [Fact]
    [Trait("_Active Tests", "Current")]
    public void GenerateCodeTest()
    {
        // Assign
        _ = new CancellationTokenSource();
        _ = CreateModel();

        // Act
        var actual = this._codeService.GenerateCodes(null!);

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
        var actual = await this._service.GenerateAsync(model, tokenSource.Token);

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