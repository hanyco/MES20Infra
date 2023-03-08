using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Services;

namespace InfraTestProject;

public class DtoServiceTest : IClassFixture<DtoServiceFixture>
{
    private readonly DtoServiceFixture _fixture;

    public DtoServiceTest(DtoServiceFixture fixture)
    {
        this._fixture = fixture;
    }

    [Fact]
    public void InsertDto()
    {
        var module = this._fixture.GetService<IModuleService>().GetByIdAsync(1).Result!;
        var model = new HanyCo.Infra.UI.ViewModels.DtoViewModel { Name = "Test DTO", Module = module };
        var actual = this._fixture.Service.InsertAsync(model).Result;
        Assert.True(actual);
    }
}