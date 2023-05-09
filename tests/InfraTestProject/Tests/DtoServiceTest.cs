using Contracts.ViewModels;

using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.Coding;
using Library.Results;

using Xunit.Abstractions;

namespace InfraTestProject.Tests;

public sealed class DtoServiceTest
{
    private readonly IDtoCodeService _codeService;
    private readonly IModuleService _moduleService;
    private readonly ITestOutputHelper _output;
    private readonly IDtoService _service;

    public DtoServiceTest(ITestOutputHelper output, IDtoService service, IModuleService moduleService, IDtoCodeService codeService)
    {
        this._output = output;
        this._service = service;
        this._moduleService = moduleService;
        this._codeService = codeService;
    }

    [Fact]
    [Trait(nameof(DtoServiceTest), "CRUD Test")]
    public async Task _20_GetByIdTestAsync()
    {
        var model1 = await this.InsertDtoAsync("DTO 1");
        var model2 = await this.InsertDtoAsync("DTO 2");
        var model3 = await this.InsertDtoAsync("DTO 3");

        var actual1 = await this._service.GetByIdAsync(model1.Value.Id!.Value);
        Assert.NotNull(actual1);
        Assert.NotNull(actual1.Id);
        Assert.Equal(model1.Value.Name, actual1.Name);

        var actual2 = await this._service.GetByIdAsync(model2.Value.Id!.Value);
        Assert.NotNull(actual2);
        Assert.NotNull(actual2.Id);
        Assert.Equal(model2.Value.Name, actual2.Name);

        var actual3 = await this._service.GetByIdAsync(model3.Value.Id!.Value);
        Assert.NotNull(actual3);
        Assert.NotNull(actual3.Id);
        Assert.Equal(model3.Value.Name, actual3.Name);
    }

    [Fact]
    [Trait(nameof(DtoServiceTest), "CRUD Test")]
    public async void _30_InsertDtoTest()
    {
        var actual = await this.InsertDtoAsync("Test DTO");
        Assert.True(actual);
        Assert.NotNull(actual.Value);
        Assert.NotEqual(0, actual.Value.Id);
    }

    [Fact]
    [Trait(nameof(DtoServiceTest), "CRUD Test")]
    public async Task _40_UpdateDtoTestAsync()
    {
        var model = (await this.InsertDtoAsync("Test DTO")).Value;
        model.Name = "Update Test";
        _ = await this._service.UpdateAsync(model.Id!.Value, model);
        var actual = await this._service.GetByIdAsync(model.Id!.Value);
        Assert.NotNull(actual);
        Assert.NotNull(actual.Id);
        Assert.Equal(model.Name, actual.Name);
    }

    [Fact]
    [Trait(nameof(DtoServiceTest), "CRUD Test")]
    public async Task _50_DeleteDtoTestAsync()
    {
        var model = (await this.InsertDtoAsync("Test DTO")).Value;
        model.Name = "Delete Test";
        this._service.DeleteAsync(model).Wait();
        var actual = await this._service.GetByIdAsync(model.Id!.Value);
        Assert.Null(actual);
    }

    [Fact]
    [Trait(nameof(DtoServiceTest), "Operational Test")]
    public async Task _60_CreateDtoTestAsync()
    {
        var model = await this._service.CreateAsync();
        Assert.NotNull(model);
    }

    [Fact]
    [Trait(nameof(DtoServiceTest), "CRUD Test")]
    public async Task _70_GetAllByCategoryAsyncTest()
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
        Assert.Equal(0, actual5.Count);

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
    [Trait(nameof(DtoServiceTest), "Operational Test")]
    public void _80_CodeGenerationTest()
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

    private DtoViewModel CreateByDbTable()
    {
        return this._service.CreateByDbTable(new("Person", objectId(), "unit_test"),
            new DbColumnViewModel[] {
                new("Name", objectId(), "nvarchar", false),
                new("Age", objectId(),"int", false)
            }).With(x => x.NameSpace = "unittest");

        static long objectId() 
            => NumberHelper.RandomNumber(10000);
    }

    private async Task<Result<DtoViewModel>> InsertDtoAsync(string dtoName)
    {
        var module = await this._moduleService.GetByIdAsync(1);
        var model = new DtoViewModel { Name = dtoName, Module = module! };
        return await this._service.InsertAsync(model);
    }
}