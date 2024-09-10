using Library.Cqrs.Models.Commands;
using HumanResource.Dtos;
using System.Threading.Tasks;
using Library.Validations;

namespace HumanResource.Commands;
public sealed class UpdatePersonCommandValidator : ICommandValidator<UpdatePersonCommand>
{
    public ValueTask ValidateAsync(UpdatePersonCommand command)
    {
        _ = command.ArgumentNotNull().Params.Check().RuleFor(x => x.Id > 0, () => "Id cannot be null, zero or less than zero.").NotNull(x => x.LastName).NotNull(x => x.DateOfBirth).ThrowOnFail();
        return ValueTask.CompletedTask;
    }
}