using Library.Cqrs.Models.Commands;
using HumanResource.Dtos;

namespace HumanResource.Dtos;
public sealed class InsertPersonCommand : ICommand
{
    public InsertPersonCommand(InsertPerson @params)
    {
        this.Params = @params;
    }

    public InsertPerson Params { get; set; }
}