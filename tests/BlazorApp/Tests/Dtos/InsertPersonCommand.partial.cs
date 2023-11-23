using Library.Cqrs.Models.Commands;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Dtos;
public sealed class InsertPersonCommand : ICommand
{
    public InsertPersonCommand(InsertPerson @params)
    {
        this.Params = @params;
    }

    public InsertPerson Params { get; set; }
}