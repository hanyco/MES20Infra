using Contracts.Services;
using Contracts.ViewModels;

namespace InfraTestProject.Tests.Services;

public sealed class SqlStatementCodeGeneratorTest(ISqlStatementCodeGenerator codeGenerator)
{
    private readonly ISqlStatementCodeGenerator _codeGenerator = codeGenerator;

    [Fact]
    public void _01_GenerateSelectAllSqlStatement()
    {
        var viewModel = CreateSampleViewModel();
        var code = _codeGenerator.GenerateSelectAllSqlStatement(viewModel, "GetAllPeople");
        Assert.True(code.Result.IsSucceed);
    }

    private static DtoViewModel CreateSampleViewModel()
    {
        var model = new DtoViewModel
        {
            DbObject = new("Person", schema: "dto"),
        };
        _ = model.Properties.AddRange(new PropertyViewModel[]
        {
            new()
            {
                DbObject = new("Id"),
            },
            new()
            {
                DbObject = new("Name"),
            },
            new()
            {
                DbObject = new("Age"),
            },
        });
        return model;
    }
}