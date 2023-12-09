using Library.Cqrs.Models.Commands;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed class DeletePersonCommand : ICommand
{
    public DeletePersonCommand(DeletePerson @params)
    {
        this.Params = @params;
    }

    public DeletePerson Params { get; set; }
}