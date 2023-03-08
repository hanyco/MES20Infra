using HanyCo.Infra.Internals.Data.DataSources;

namespace InfraTestProject;

public class DtoServiceTest : IClassFixture<ServicesFixture>
{
    private readonly ServicesFixture _fixture;

    public DtoServiceTest(ServicesFixture fixture)
    {
        this._fixture = fixture;
    }

    [Fact]
    public void TestOne()
    {
        this._fixture.WriteDbContext.Add(new Dto { Id = 1, Guid = Guid.NewGuid(), Name = "TestDto" });
        this._fixture.WriteDbContext.SaveChanges();
    }
}