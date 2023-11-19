using Library.Cqrs.Models.Commands;

namespace Test.HumanResources.Dtos;
public sealed class UpdatePersonCommand : ICommand
{
    public UpdatePersonCommand(UpdatePersonParams @params)
    {
        this.Params = @params;
    }

    public UpdatePersonParams Params { get; set; }
}