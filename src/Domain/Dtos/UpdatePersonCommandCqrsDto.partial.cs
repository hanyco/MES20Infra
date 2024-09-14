using Library.Cqrs.Models.Commands;
using HumanResource.Dtos;

namespace HumanResource.Dtos;
public sealed class UpdatePersonCommand : ICommand
{
    public UpdatePersonCommand(UpdatePerson @params)
    {
        this.Params = @params;
    }

    public UpdatePerson Params { get; set; }
}