using Library.Cqrs.Models.Commands;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Dtos;
public sealed class DeletePersonCommand : ICommand
{
    public DeletePersonCommand(DeletePerson @params)
    {
        this.Params = @params;
    }

    public DeletePerson Params { get; set; }
}