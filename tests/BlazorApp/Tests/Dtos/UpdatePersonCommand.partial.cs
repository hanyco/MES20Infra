using Library.Cqrs.Models.Commands;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Dtos;
public sealed class UpdatePersonCommand : ICommand
{
    public UpdatePersonCommand(UpdatePerson @params)
    {
        this.Params = @params;
    }

    public UpdatePerson Params { get; set; }
}