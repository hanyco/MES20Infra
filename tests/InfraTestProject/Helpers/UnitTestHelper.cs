using Library.Results;

namespace InfraTestProject.Helpers;

internal static class UnitTestHelper
{
    public static TResult CheckAssertion<TResult>(this TResult result)
        where TResult : ResultBase
    {
        Assert.NotNull(result);
        if (result.IsSucceed)
        {
            Assert.Fail(result.Message!);
        }
        return result;
    }
}