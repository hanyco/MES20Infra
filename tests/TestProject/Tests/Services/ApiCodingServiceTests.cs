using Library.Coding;

using Xunit.Abstractions;

namespace TestProject.Tests.Services;

public sealed class ApiCodingServiceTests(ITestOutputHelper output, IApiCodingService service)
{
    [Fact]
    public void CrudWithGetAllCustomBody()
    {
        var bodyCode = "return Task.CompletedTask;";
        var viewModel = this.GetMinimalViewModel().With(x => x.GetAllApi.Body = bodyCode);
        var codes = service.GenerateCodes(viewModel).ThrowOnFail().Value;
        Assert.Equal(1, codes.Single()!.Statement.CountOf(bodyCode));
    }

    [Fact]
    public void CrudWithMinimalCustomSettings()
    {
        var codes = service.GenerateCodes(this.GetMinimalViewModel()).ThrowOnFail().Value;
        _ = Assert.Single(codes);
    }

    private ApiCodingViewModel GetMinimalViewModel() =>
        new() { DtoType = "PersonDto", NameSpace = this.GetType().Namespace! };
}