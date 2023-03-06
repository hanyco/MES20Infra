//using HanyCo.Infra.Internals.Data.DataSources;

//using Microsoft.EntityFrameworkCore;

//namespace InfraTestProject;

//[Collection(nameof(DbServicesFixture))]
//public class ModuleServiceTest 
//{
//    private readonly DbServicesFixture _fixture;
    
//    public ModuleServiceTest(DbServicesFixture fixture)
//        => this._fixture = fixture;//.InitializeDbForDtoServiceTest(this.ServiceProvider);

//    [Fact]
//    public void Test2()
//    {
//        // Test something else using _fixture
//    }

//    // A helper method to create a new context with In-Memory options
//    private static InfraWriteDbContext CreateNewContext()
//    {
//        var options = new DbContextOptionsBuilder<InfraWriteDbContext>()
//            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Use a unique name for each test
//            .Options;
//        return new InfraWriteDbContext(options);
//    }
//}