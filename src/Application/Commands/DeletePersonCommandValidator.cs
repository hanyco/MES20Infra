using Library.Cqrs.Models.Commands;
using HumanResource.Dtos;
using System.Threading.Tasks;

namespace HumanResource.Commands;
public sealed class DeletePersonCommandValidator : ICommandValidator<DeletePersonCommand>
{
    public ValueTask ValidateAsync(DeletePersonCommand command)
    {
        return ValueTask.CompletedTask;
    }
}