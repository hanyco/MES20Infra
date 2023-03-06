using HanyCo.Infra.Internals.Data.DataSources;

using Library.Validations;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace InfraTestProject;

// This is an empty interface that links the fixture class with the test classes
public interface ICollectionFixture<T> { }

// This is the fixture class
[CollectionDefinition(nameof(DbServicesFixture))]
public class DbServicesFixture: ICollectionFixture<DbServicesFixture>
{
    // This constructor will run once before all tests
    public DbServicesFixture()
    {
        // Put your setup code here
    }

    public DbServicesFixture InitializeDbForDtoServiceTest(IServiceProvider serviceProvider)
    {
        _ = GetDbContexts(serviceProvider);
        return this;
    }

    private static (InfraWriteDbContext, InfraReadDbContext) GetDbContexts(IServiceProvider serviceProvider) => (serviceProvider.GetService<InfraWriteDbContext>()!, serviceProvider.GetService<InfraReadDbContext>()!);
}

public class TestBase
{
    public TestBase(IServiceProvider serviceProvider)
        => this.ServiceProvider = serviceProvider;

    public TestBase() 
        => this.ServiceProvider = TestStartup.GetServiceProvider();

    protected IServiceProvider ServiceProvider { get; }

    protected TService GetService<TService>()
        => this.ServiceProvider.GetService<TService>().NotNull();
}

//// This attribute defines the name of the collection
////[CollectionDefinition("Database collection")]
//public class DatabaseFixture : IDisposable//, ICollectionFixture<DatabaseFixture>
//{
//    // This constructor initializes the connection to the database
//    public DatabaseFixture()
//    {
//        Connection = new SqlConnection("connection string");
//        Connection.Open();
//    }

//    // This property exposes the connection to the test classes
//    public SqlConnection Connection { get; private set; }

//    // This method disposes the connection when the fixture is no longer needed
//    public void Dispose()
//    {
//        Connection.Close();
//    }
//}

//// This attribute specifies the name of the collection that this test class belongs to
//[Collection("Database collection")]
//public class DatabaseTestClass
//{
//    // This constructor receives the fixture as a parameter
//    public DatabaseTestClass(DatabaseFixture fixture)
//    {
//        // This field stores the reference to the fixture
//        Fixture = fixture;
//    }

//    // This property exposes the fixture to the test methods
//    public DatabaseFixture Fixture { get; private set; }

//    // This method tests the number of records in a table using the connection from the fixture
//    [Fact]
//    public void TestRecordCount()
//    {
//        // Arrange
//        var expected = 10;
//        var command = new SqlCommand("SELECT COUNT(*) FROM Table", Fixture.Connection);

//        // Act
//        var actual = (int)command.ExecuteScalar();

//        // Assert
//        Assert.Equal(expected, actual);
//    }
//}

//[CollectionDefinition("Database collection")]
//public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
//{
//    // This class has no code, and is never created. Its purpose is simply
//    // to be the place to apply [CollectionDefinition] and all the
//    // ICollectionFixture<> interfaces.
//}

public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        Db = new SqlConnection("MyConnectionString");

        // ... initialize data in the test database ...
    }

    public void Dispose()
    {
        // ... clean up test data from the database ...
    }

    public SqlConnection Db { get; private set; }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

[Collection("Database collection")]
public class DatabaseTestClass1
{
    DatabaseFixture fixture;

    public DatabaseTestClass1(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void Test()
    {

    }
}

[Collection("Database collection")]
public class DatabaseTestClass2
{
    // ...
}