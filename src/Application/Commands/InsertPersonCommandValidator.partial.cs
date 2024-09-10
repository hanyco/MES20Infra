using Library.Cqrs.Models.Commands;
using HumanResource.Dtos;
using System.Threading.Tasks;
using Library.Validations;

namespace HumanResource.Commands;
public sealed class InsertPersonCommandValidator : ICommandValidator<InsertPersonCommand>
{
    public ValueTask ValidateAsync(InsertPersonCommand command)
    {
        _ = command.ArgumentNotNull().Params.Check().NotNull(x => x.LastName).NotNull(x => x.DateOfBirth).ThrowOnFail();
        return ValueTask.CompletedTask;
    }
}