using Library.Cqrs.Models.Commands;
using HumanResource.Dtos;

namespace HumanResource.Dtos;
public sealed class DeletePersonCommand : ICommand
{
    public DeletePersonCommand(DeletePerson @params)
    {
        this.Params = @params;
    }

    public DeletePerson Params { get; set; }
}