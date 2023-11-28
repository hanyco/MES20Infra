using Library.Cqrs.Models.Commands;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed class UpdatePersonCommand : ICommand
{
    public UpdatePersonCommand(UpdatePerson @params)
    {
        this.Params = @params;
    }

    public UpdatePerson Params { get; set; }
}