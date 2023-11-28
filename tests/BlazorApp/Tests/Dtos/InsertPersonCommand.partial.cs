using Library.Cqrs.Models.Commands;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed class InsertPersonCommand : ICommand
{
    public InsertPersonCommand(InsertPerson @params)
    {
        this.Params = @params;
    }

    public InsertPerson Params { get; set; }
}