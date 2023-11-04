using Library.Results;

namespace Test.HumanResources.Dtos
{
    public sealed class InsertPersonCommandResult
    {
        public InsertPersonCommandResult(Result result)
        {
            this.Result = result;
        }

        public Result Result { get; set; }
    }
}