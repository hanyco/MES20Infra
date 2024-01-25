


using Library.CodeGeneration;

namespace InfraTestProject.Tests.Services;

[Trait("Category", nameof(IAdvancedSearchService))]
public sealed class AdvancedSearchServiceTest(IAdvancedSearchService service)
{
    [Fact]
    public void SimpleTest()
    {
        // Assign
        var model = new AdvancedSearchViewModel
        {
            GetSampleFields().First()
        };

        // Act
        var codes = service.GenerateCodes(model);

        // Assert
        Assert.True(codes.IsSucceed);
    }

    [Fact]
    public void ValidationTest()
    {
        // Assign
        var model = new AdvancedSearchViewModel();

        // Act
        var codes = service.GenerateCodes(model);

        // Assert
        Assert.False(codes.IsSucceed);
    }

    private static IEnumerable<AdvancedSearchOperation> GetSampleFields()
    {
        yield return new AdvancedSearchOperation(new("Id", TypePath.New<long>()), AdvancedSearchFieldOperator.IsBiggerThan, [2]);
        yield return new AdvancedSearchOperation(new("Id", TypePath.New<long>()), AdvancedSearchFieldOperator.IsLessThan, [10]);
        yield return new AdvancedSearchOperation(new("FirstName", TypePath.New<string>()), AdvancedSearchFieldOperator.Contains, ["Ali"]);
        yield return new AdvancedSearchOperation(new("LastName", TypePath.New<string>()), AdvancedSearchFieldOperator.StartsWith, ["Alavi"]);
    }
}