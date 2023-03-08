using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.Results;

namespace InfraTestProject;

public class DtoServiceTest : IClassFixture<DtoServiceFixture>
{
    private readonly DtoServiceFixture _fixture;

    public DtoServiceTest(DtoServiceFixture fixture)
        => this._fixture = fixture;

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
        _ = await this._fixture.Service.UpdateAsync(model.Id!.Value, model);
        var actual = await this._fixture.Service.GetByIdAsync(model.Id!.Value);
        Assert.NotNull(actual);
        Assert.NotNull(actual.Id);
        Assert.Equal(model.Name, actual.Name);
    }

    [Fact]
    public async Task _20_GetByIdTestAsync()
    {
        var model1 = await this.InsertDtoAsync("DTO 1");
        var model2 = await this.InsertDtoAsync("DTO 2");
        var model3 = await this.InsertDtoAsync("DTO 3");
        
        var actual1 = await this._fixture.Service.GetByIdAsync(model1.Value.Id!.Value);
        Assert.NotNull(actual1);
        Assert.NotNull(actual1.Id);
        Assert.Equal(model1.Value.Name, actual1.Name);

        var actual2 = await this._fixture.Service.GetByIdAsync(model2.Value.Id!.Value);
        Assert.NotNull(actual2);
        Assert.NotNull(actual2.Id);
        Assert.Equal(model2.Value.Name, actual2.Name);

        var actual3 = await this._fixture.Service.GetByIdAsync(model3.Value.Id!.Value);
        Assert.NotNull(actual3);
        Assert.NotNull(actual3.Id);
        Assert.Equal(model3.Value.Name, actual3.Name);
    }

    [Fact]
    public async Task _50_DeleteDtoTestAsync()
    {
        var model = (await this.InsertDtoAsync("Test DTO")).Value;
        model.Name = "Delete Test";
        this._fixture.Service.DeleteAsync(model).Wait();
        var actual = await this._fixture.Service.GetByIdAsync(model.Id!.Value);
        Assert.Null(actual);
    }

    private async Task<Result<DtoViewModel>> InsertDtoAsync(string dtoName)
    {
        var module = this._fixture.GetService<IModuleService>().GetByIdAsync(1).Result!;
        var model = new DtoViewModel { Name = dtoName, Module = module };
        return await this._fixture.Service.InsertAsync(model);
    }
}