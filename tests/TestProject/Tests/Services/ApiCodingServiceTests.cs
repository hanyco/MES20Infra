using Contracts.Services;
using Contracts.ViewModels;

using Library.Coding;

using Xunit.Abstractions;

namespace TestProject.Tests.Services;

public sealed class ApiCodingServiceTests(ITestOutputHelper output, IApiCodingService service)
{
    [Fact]
    public void CrudWithGetAllCustomBody()
    {
        var viewModel = this.GetMinimalViewModel().With(x => x.GetAllApi.Body = "return Task.CompletedTask;");
        var codes = service.GenerateCodes(viewModel);
        Assert.NotNull(codes);
    }

    [Fact]
    public void CrudWithMinimalCustomSettings()
    {
        var codes = service.GenerateCodes(this.GetMinimalViewModel());
        Assert.NotNull(codes);
    }

    private ApiCodingViewModel GetMinimalViewModel() => new() { DtoType = "PersonDto", NameSpace = this.GetType().Namespace! };
}