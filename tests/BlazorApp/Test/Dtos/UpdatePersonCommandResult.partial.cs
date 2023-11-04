using Library.Results;

namespace Test.HumanResources.Dtos
{
    public sealed class UpdatePersonCommandResult
    {
        public UpdatePersonCommandResult(Result result)
        {
            this.Result = result;
        }

        public Result Result { get; set; }
    }
}