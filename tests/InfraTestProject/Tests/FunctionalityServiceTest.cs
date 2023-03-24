using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using InfraTestProject.Fixtures;

using Library.Data.SqlServer.Dynamics;
using Library.Logging;
using Library.Threading.MultistepProgress;

using Xunit.Abstractions;

namespace InfraTestProject.Tests;

public class FunctionalityServiceTest : ServiceTestBase<IFunctionalityService, FunctionalityServiceFixture>
{
    private readonly IUnitTestLogger _logger;

    public FunctionalityServiceTest(FunctionalityServiceFixture fixture, ITestOutputHelper output) : base(fixture)
        => this._logger = IUnitTestLogger.New(output).HandleReporterEvents(DI.GetService<IMultistepProcess>());

    [Fact(DisplayName = "Main Test")]
    [Trait(nameof(FunctionalityServiceTest), "Main Test")]
    public async Task Generate_MainTest_Async()
    {
        var model = initializeModel();

        using var blockLogger = CodeBlockLogger.New(this._logger, "Test is starting...", "Test is ended.");
        _ = await this.Service.GenerateAsync(model).ThrowOnFailAsync();
        //this._logger.Log("Test is starting...");
        //_ = await this.Service.GenerateAsync(model).ThrowOnFailAsync();
        //this._logger.Log("Test is ended.");

        static FunctionalityViewModel initializeModel()
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
}