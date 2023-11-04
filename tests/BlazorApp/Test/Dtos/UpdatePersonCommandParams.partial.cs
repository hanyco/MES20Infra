
using Library.Cqrs.Models.Commands;

namespace Test.HumanResources.Dtos
{
    public sealed class UpdatePersonCommandParams : ICommand
    {
        public UpdatePersonCommandParams(UpdatePersonParams @params)
        {
            this.Params = @params;
        }

        public UpdatePersonParams Params { get; set; }
    }
}