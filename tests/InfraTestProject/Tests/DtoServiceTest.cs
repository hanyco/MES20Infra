using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.Results;

namespace InfraTestProject.Tests;

public sealed class DtoServiceTest : ServiceTestBase<IDtoService, DtoServiceFixture>
{
    public DtoServiceTest(DtoServiceFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task _20_GetByIdTestAsync()
    {
        var model1 = await this.InsertDtoAsync("DTO 1");
        var model2 = await this.InsertDtoAsync("DTO 2");
        var model3 = await this.InsertDtoAsync("DTO 3");

        var actual1 = await this.Service.GetByIdAsync(model1.Value.Id!.Value);
        Assert.NotNull(actual1);
        Assert.NotNull(actual1.Id);
        Assert.Equal(model1.Value.Name, actual1.Name);

        var actual2 = await this.Service.GetByIdAsync(model2.Value.Id!.Value);
        Assert.NotNull(actual2);
        Assert.NotNull(actual2.Id);
        Assert.Equal(model2.Value.Name, actual2.Name);

        var actual3 = await this.Service.GetByIdAsync(model3.Value.Id!.Value);
        Assert.NotNull(actual3);
        Assert.NotNull(actual3.Id);
        Assert.Equal(model3.Value.Name, actual3.Name);
    }

    [Fact]
    public async void _30_InsertDtoTest()
    {
        var actual = await this.InsertDtoAsync("Test DTO");
        Assert.True(actual);
        Assert.NotNull(actual.Value);
        Assert.NotEqual(0, actual.Value.Id);
    }

    [Fact]
    public async Task _40_UpdateDtoTestAsync()
    {
        var model = (await this.InsertDtoAsync("Test DTO")).Value;
        model.Name = "Update Test";
        _ = await this.Fixture.Service.UpdateAsync(model.Id!.Value, model);
        var actual = await this.Fixture.Service.GetByIdAsync(model.Id!.Value);
        Assert.NotNull(actual);
        Assert.NotNull(actual.Id);
        Assert.Equal(model.Name, actual.Name);
    }

    [Fact]
    public async Task _50_DeleteDtoTestAsync()
    {
        var model = (await this.InsertDtoAsync("Test DTO")).Value;
        model.Name = "Delete Test";
        this.Fixture.Service.DeleteAsync(model).Wait();
        var actual = await this.Fixture.Service.GetByIdAsync(model.Id!.Value);
        Assert.Null(actual);
    }

    [Fact]
    public async Task _60_CreateDtoTestAsync()
    {
        var model = await this.Fixture.Service.CreateAsync();
        Assert.NotNull(model);
    }

    [Fact]
    public async Task _70_GetAllByCategoryAsyncTest()
    {
        _ = await this.InsertDtoAsync(x =>
        {
            var model = x.Model;
            model.IsParamsDto = x.Index % 3 == 0;
            model.IsResultDto = x.Index % 4 == 0;
            model.IsViewModel = x.Index % 5 == 0;

            return (model, x.Index < 30);
        });

        var actual1 = await this.Fixture.Service.GetAllByCategoryAsync(true, false, false);
        Assert.Equal(10, actual1.Count);

        var actual2 = await this.Fixture.Service.GetAllByCategoryAsync(false, true, false);
        Assert.Equal(8, actual2.Count);

        var actual3 = await this.Fixture.Service.GetAllByCategoryAsync(false, false, true);
        Assert.Equal(6, actual3.Count);

        var actual4 = await this.Fixture.Service.GetAllByCategoryAsync(true, true, true);
        Assert.Equal(18, actual4.Count);

        var actual5 = await this.Fixture.Service.GetAllByCategoryAsync(false, false, false);
        Assert.Equal(0, actual5.Count);
    }

    private async Task<Result<DtoViewModel>> InsertDtoAsync(string dtoName)
    {
        var module = await this.Fixture.GetService<IModuleService>().GetByIdAsync(1);
        var model = new DtoViewModel { Name = dtoName, Module = module! };
        return await this.Fixture.Service.InsertAsync(model);
    }

    private async Task<Result<int>> InsertDtoAsync(Func<(DtoViewModel Model, int Index), (DtoViewModel Model, bool canContiniue)> process)
    {
        var module = await this.Fixture.GetService<IModuleService>().GetByIdAsync(1);
        var canContinue = true;
        var index = 0;
        while (canContinue)
        {
            var model = new DtoViewModel { Name = $"DTO {index}", Module = module! };
            var (Model, canContiniue) = process((model, index++));
            if (!canContiniue)
            {
                break;
            }

            _ = await this.Fixture.Service.InsertAsync(Model, persist: false);
        }
        return await this.Fixture.Service.SaveChangesAsync();
    }
}