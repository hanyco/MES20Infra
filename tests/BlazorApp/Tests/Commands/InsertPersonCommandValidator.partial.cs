using Library.Cqrs.Models.Commands;
using HumanResources.Dtos;
using System.Threading.Tasks;
using Library.Validations;

namespace HumanResources.Commands;
public sealed class InsertPersonCommandValidator : ICommandValidator<InsertPersonCommand>
{
    public ValueTask ValidateAsync(InsertPersonCommand command)
    {
        _ = command.Check().NotNull(x => x.Params.LastName).NotNull(x => x.Params.DateOfBirth).ThrowOnFail();
        return ValueTask.CompletedTask;
    }
}