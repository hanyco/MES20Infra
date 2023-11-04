using Library.Interfaces;

namespace Test.HumanResources.Dtos
{
    public sealed class InsertPersonCommandParams : ICommand<InsertPersonCommandResult>
    {
        public InsertPersonCommandParams(InsertPersonParams @params)
        {
            this.Params = @params;
        }

        public InsertPersonParams Params { get; set; }
    }
}