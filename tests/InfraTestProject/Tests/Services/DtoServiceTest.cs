using Library.Coding;
using Library.Results;

namespace InfraTestProject.Tests.Services;

public sealed class DtoServiceTest(IDtoService service, IModuleService moduleService, IDtoCodeService codeService)
{
    private readonly IDtoCodeService _codeService = codeService;
    private readonly IModuleService _moduleService = moduleService;
    private readonly IDtoService _service = service;

    [Fact]
    [Trait(nameof(DtoServiceTest), "Operational Test")]
    public async Task CreateDtoTestAsync()
    {
        var model = await this._service.CreateAsync();
        Assert.NotNull(model);
    }

    [Fact]
    [Trait(nameof(DtoServiceTest), "CRUD Test")]
    public async Task DeleteDtoTestAsync()
    {
        var model = (await this.InsertDtoAsync("Test DTO")).Value;
        model.Name = "Delete Test";
        _ = await this._service.DeleteAsync(model);
        var actual = await this._service.GetByIdAsync(model.Id!.Value);
        Assert.Null(actual);
    }

    [Fact]
    [Trait(nameof(DtoServiceTest), "Operational Test")]
    public void GenerateCodeFromScratch()
    {
        // Assign
        var dtoModel = this.CreateByDbTable();

        // Act
        var codes = this._codeService.GenerateCodes(dtoModel);

        // Assert
        if (!codes.IsSucceed)
        {
            Assert.Fail(codes.ToString());
        }
        else if (codes.Value.Count != 1)
        {
            Assert.Fail("No code generated.");
        }
        else if (codes.Value?[0]?.Statement is null)
        {
            Assert.Fail("Code statement is empty");
        }
    }

    [Fact]
    [Trait(nameof(DtoServiceTest), "CRUD Test")]
    public async Task GetAllByCategoryAsyncTest()
    {
        _ = await insertDtoAsync(x =>
        {
            var model = x.Model;
            model.IsParamsDto = x.Index % 3 == 0;
            model.IsResultDto = x.Index % 4 == 0;
            model.IsViewModel = x.Index % 5 == 0;

            return (model, x.Index < 30);
        });

        var actual1 = await this._service.GetAllByCategoryAsync(true, false, false);
        Assert.Equal(10, actual1.Count);

        var actual2 = await this._service.GetAllByCategoryAsync(false, true, false);
        Assert.Equal(8, actual2.Count);

        var actual3 = await this._service.GetAllByCategoryAsync(false, false, true);
        Assert.Equal(6, actual3.Count);

        var actual4 = await this._service.GetAllByCategoryAsync(true, true, true);
        Assert.Equal(18, actual4.Count);

        var actual5 = await this._service.GetAllByCategoryAsync(false, false, false);
        Assert.Empty(actual5);

        async Task<Result<int>> insertDtoAsync(Func<(DtoViewModel Model, int Index), (DtoViewModel Model, bool canContiniue)> process)
        {
            var module = await this._moduleService.GetByIdAsync(1);
            var canContinue = true;
            var index = 0;
            while (canContinue)
            {
                var model = new DtoViewModel { Name = $"DTO {index}", Module = module! };
                var (Model, canGoOn) = process((model, index++));
                if (!canGoOn)
                {
                    break;
                }

                _ = await this._service.InsertAsync(Model, persist: false);
            }
            return await this._service.SaveChangesAsync();
        }
    }

    [Fact]
    [Trait(nameof(DtoServiceTest), "CRUD Test")]
    public async Task GetByIdAsync()
    {
        var model1 = await this.InsertDtoAsync("DTO 1");
        var model2 = await this.InsertDtoAsync("DTO 2");
        var model3 = await this.InsertDtoAsync("DTO 3");

        var actual1 = await this._service.GetByIdAsync(model1.Value.Id!.Value);
        Assert.NotNull(actual1);
        _ = Assert.NotNull(actual1.Id);
        Assert.Equal(model1.Value.Name, actual1.Name);

        var actual2 = await this._service.GetByIdAsync(model2.Value.Id!.Value);
        Assert.NotNull(actual2);
        _ = Assert.NotNull(actual2.Id);
        Assert.Equal(model2.Value.Name, actual2.Name);

        var actual3 = await this._service.GetByIdAsync(model3.Value.Id!.Value);
        Assert.NotNull(actual3);
        _ = Assert.NotNull(actual3.Id);
        Assert.Equal(model3.Value.Name, actual3.Name);
    }

    [Fact]
    [Trait(nameof(DtoServiceTest), "CRUD Test")]
    public async void InsertDtoTest()
    {
        var actual = await this.InsertDtoAsync("Test DTO");
        Assert.True(actual.IsSucceed);
        Assert.NotNull(actual.Value);
        Assert.NotEqual(0, actual.Value.Id);
    }

    [Fact]
    public async Task LoadDtoAndGenerateCodeAsync()
    {
        var dtoToSave = this.CreateByDbTable();
        var saveResult = await this._service.InsertAsync(dtoToSave);
        var id = saveResult.Value.Id!.Value;
        var dtoToGenCode = await this._service.GetByIdAsync(id);
        var codes = this._codeService.GenerateCodes(dtoToGenCode!);
        if (!codes.IsSucceed)
        {
            Assert.Fail(codes.ToString());
        }
        else if (codes.Value.Count != 1)
        {
            Assert.Fail("No code generated.");
        }
        else if (codes.Value?[0]?.Statement is null)
        {
            Assert.Fail("Code statement is empty");
        }
    }

    [Fact]
    [Trait(nameof(DtoServiceTest), "CRUD Test")]
    public async Task UpdateDtoTestAsync()
    {
        var model = (await this.InsertDtoAsync("Test DTO")).Value;
        model.Name = "Update Test";
        _ = await this._service.UpdateAsync(model.Id!.Value, model);
        var actual = await this._service.GetByIdAsync(model.Id!.Value);
        Assert.NotNull(actual);
        _ = Assert.NotNull(actual.Id);
        Assert.Equal(model.Name, actual.Name);
    }

    private DtoViewModel CreateByDbTable()
        => this._service.CreateByDbTable(new("Person", NumberHelper.RandomNumber(10000), "unit_test"),
                new DbColumnViewModel[] {
                    new("FirstName", NumberHelper.RandomNumber(10000), "nvarchar", false),
                    new("LastName", NumberHelper.RandomNumber(10000), "nvarchar", true),
                    new("Age", NumberHelper.RandomNumber(10000),"int", false),
                    new("Gender", NumberHelper.RandomNumber(10000),"int", true)
                }
            )
        .With(x => x.NameSpace = "unittest")
        .With(x => x.Module = this._moduleService.GetByIdAsync(1).Result!);

    private async Task<Result<DtoViewModel>> InsertDtoAsync(string dtoName)
    {
        var module = await this._moduleService.GetByIdAsync(1);
        var model = new DtoViewModel { Name = dtoName, Module = module! };
        return await this._service.InsertAsync(model);
    }
}