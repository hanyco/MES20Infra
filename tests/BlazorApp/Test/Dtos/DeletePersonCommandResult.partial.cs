using Library.Results;

namespace Test.HumanResources.Dtos
{
    public sealed class DeletePersonCommandResult
    {
        public DeletePersonCommandResult(Result result)
        {
            this.Result = result;
        }

        public Result Result { get; set; }
    }
}