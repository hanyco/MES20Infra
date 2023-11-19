using Library.Cqrs.Models.Commands;

namespace Test.HumanResources.Dtos;
public sealed class InsertPersonCommand : ICommand
{
    public InsertPersonCommand(InsertPersonParams @params)
    {
        this.Params = @params;
    }

    public InsertPersonParams Params { get; set; }
}