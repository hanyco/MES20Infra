
using Library.Cqrs.Models.Commands;

namespace Test.HumanResources.Dtos
{
    public sealed class DeletePersonCommandParams : ICommand
    {
        public DeletePersonCommandParams(DeletePersonParams @params)
        {
            this.Params = @params;
        }

        public DeletePersonParams Params { get; set; }
    }
}