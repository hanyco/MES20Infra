using Library.Cqrs.Models.Commands;
using HumanResources.Dtos;
using System.Threading.Tasks;

namespace HumanResources.Commands;
public sealed class DeletePersonCommandValidator : ICommandValidator<DeletePersonCommand>
{
    public ValueTask ValidateAsync(DeletePersonCommand command)
    {
        return ValueTask.CompletedTask;
    }
}