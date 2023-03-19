using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using InfraTestProject.Fixtures;

using Library.Data.SqlServer.Dynamics;
using Library.Exceptions.Validations;

using Microsoft.VisualStudio.TestPlatform.Utilities;

using Xunit.Abstractions;

namespace InfraTestProject.Tests;

public class FunctionalityServiceTest : ServiceTestBase<IFunctionalityService, FunctionalityServiceFixture>
{
    private readonly ITestOutputHelper _output;

    public FunctionalityServiceTest(FunctionalityServiceFixture fixture, ITestOutputHelper output) : base(fixture)
    {
        this._output = output;
    }

    [Fact(DisplayName ="Main Test")]
    [Trait(nameof(FunctionalityServiceTest), "Main Test")]
    public async Task Generate_MainTest_Async()
    {
        const string cs = "InMemoryConnectionString";
        var model = new FunctionalityViewModel()
        {
            DbTable = new(null!, "Person", "dbo", cs),
            DbObject = new("Person") { Id = 1 },
            Name = "TestFunctionality",
            NameSpace = "CodeGen.UnitTest",
            ModuleId = 1,
        };
        model.DbTable.Columns = new(new Column[]
        {
            new(model.DbTable, "Name", cs) { DataType = "nvarchar", MaxLength=50, },
            new(model.DbTable, "Age", cs) { DataType = "int" },
        });
        _output.WriteLine("Starting...");
        var actual = await this.Service.GenerateAsync(model).ThrowOnFailAsync();
        _output.WriteLine("Done.");
    }
}