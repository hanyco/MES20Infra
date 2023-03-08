using HanyCo.Infra.Internals.Data.DataSources;

namespace InfraTestProject;

public class DtoServiceTest : IClassFixture<DtoServiceFixture>
{
    private readonly DtoServiceFixture _fixture;

    public DtoServiceTest(DtoServiceFixture fixture)
    {
        this._fixture = fixture;
    }

    [Fact]
    public void TestOne()
    {
        var test = _fixture.Service.GetAllAsync().Result;
    }
}