using Contracts.Services;

using Xunit.Abstractions;

namespace TestProject.Tests.Services;

public sealed class ApiCodingServiceTests(ITestOutputHelper output, IApiCodingService service)
{
    [Fact]
    public void MyTestMethod()
    {
        var codes = service.GenerateCodes(new() { DtoType = "PersonDto", NameSpace = this.GetType().Namespace! });
        Assert.NotNull(codes);
    }
}