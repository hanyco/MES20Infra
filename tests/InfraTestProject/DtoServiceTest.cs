using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.Results;

namespace InfraTestProject;

public class DtoServiceTest : IClassFixture<DtoServiceFixture>
{
    private readonly DtoServiceFixture _fixture;

    public DtoServiceTest(DtoServiceFixture fixture) => this._fixture = fixture;

    [Fact]
    public void _30_InsertDtoTest()
    {
        var actual = this.InsertDto();
        Assert.True(actual);
        Assert.NotNull(actual.Value);
        Assert.NotEqual(0, actual.Value.Id);
    }

    [Fact]
    public void _40_UpdateDtoTest()
    {
        var model = InsertDto().Value;
        model.Name = "Update Test";
        _fixture.Service.UpdateAsync(model.Id!.Value, model).Wait();
        var actual = _fixture.Service.GetByIdAsync(model.Id!.Value).Result;
        Assert.NotNull(actual);
        Assert.NotNull(actual.Id);
        Assert.Equal(model.Name, actual.Name);
    }

    [Fact]
    public void _50_DeleteDtoTest()
    {
        var model = InsertDto().Value;
        model.Name = "Delete Test";
        _fixture.Service.DeleteAsync(model).Wait();
        var actual = _fixture.Service.GetByIdAsync(model.Id!.Value).Result;
        Assert.Null(actual);
    }

    private Result<DtoViewModel> InsertDto()
    {
        var module = this._fixture.GetService<IModuleService>().GetByIdAsync(1).Result!;
        var model = new DtoViewModel { Name = "Test DTO", Module = module };
        return this._fixture.Service.InsertAsync(model).Result;
    }
}