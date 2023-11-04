using Library.Interfaces;

namespace Test.HumanResources.Dtos
{
    public sealed class DeletePersonCommandParams : ICommand<DeletePersonCommandResult>
    {
        public DeletePersonCommandParams(DeletePersonParams @params)
        {
            this.Params = @params;
        }

        public DeletePersonParams Params { get; set; }
    }
}