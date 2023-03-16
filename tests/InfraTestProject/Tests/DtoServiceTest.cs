using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.Helpers;
using Library.Results;

namespace InfraTestProject.Tests;

public sealed class DtoServiceTest : ServiceTestBase<IDtoService, DtoServiceFixture>
{
    public DtoServiceTest(DtoServiceFixture fixture) : base(fixture)
    {
    }

    [Theory()]
    [InlineData("DTO 1")]
    [InlineData("DTO 2")]
    [InlineData("DTO 3")]
    [Trait(nameof(DtoServiceTest), "CRUD Test")]
    public async Task _20_GetByIdTestAsync(string dtoName)
    {
        var model = await this.InsertDtoAsync(dtoName).GetValue();
        var actual = await this.Service.GetByIdAsync(model.Id!.Value);
        Assert.NotNull(actual);
        Assert.NotNull(actual.Id);
        Assert.Equal(model.Name, actual.Name);
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
    [Trait(nameof(DtoServiceTest), "CRUD Test")]
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

        var actualParamsDtos = await this.Fixture.Service.GetAllByCategoryAsync(true, false, false);
        Assert.Equal(10, actualParamsDtos.Count);

        var actualResultDtos = await this.Fixture.Service.GetAllByCategoryAsync(false, true, false);
        Assert.Equal(8, actualResultDtos.Count);

        var actualViewModels = await this.Fixture.Service.GetAllByCategoryAsync(false, false, true);
        Assert.Equal(6, actualViewModels.Count);

        var actualAll = await this.Fixture.Service.GetAllByCategoryAsync(true, true, true);
        Assert.Equal(18, actualAll.Count);

        var actualNone = await this.Fixture.Service.GetAllByCategoryAsync(false, false, false);
        Assert.Equal(0, actualNone.Count);
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