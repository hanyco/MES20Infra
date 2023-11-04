using Library.Cqrs.Models.Commands;

namespace Test.HumanResources.Dtos
{
    public sealed class InsertPersonCommandParams : ICommand
    {
        public InsertPersonCommandParams(InsertPersonParams @params)
        {
            this.Params = @params;
        }

        public InsertPersonParams Params { get; set; }
    }
}