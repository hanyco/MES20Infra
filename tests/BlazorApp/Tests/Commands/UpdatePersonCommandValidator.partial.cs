using Library.Cqrs.Models.Commands;
using HumanResources.Dtos;
using System.Threading.Tasks;
using Library.Validations;

namespace HumanResources.Commands;
public sealed class UpdatePersonCommandValidator : ICommandValidator<UpdatePersonCommand>
{
    public ValueTask ValidateAsync(UpdatePersonCommand command)
    {
        _ = command.Check().RuleFor(x => x.Params.Id <= 0, () => "Id cannot be null, zero or less than zero.").NotNull(x => x.Params.Id).NotNull(x => x.Params.LastName).NotNull(x => x.Params.DateOfBirth).ThrowOnFail();
        return ValueTask.CompletedTask;
    }
}