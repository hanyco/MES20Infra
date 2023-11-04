using Library.Interfaces;

namespace Test.HumanResources.Dtos
{
    public sealed class UpdatePersonCommandParams : ICommand<UpdatePersonCommandResult>
    {
        public UpdatePersonCommandParams(UpdatePersonParams @params)
        {
            this.Params = @params;
        }

        public UpdatePersonParams Params { get; set; }
    }
}