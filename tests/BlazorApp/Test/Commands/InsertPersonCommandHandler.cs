using System.Threading.Tasks;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Commands;
public sealed partial class InsertPersonCommandHandler
{
    public Task<InsertPersonCommandResult> HandleAsync(InsertPersonCommand command)
    {
        return Task.FromResult<InsertPersonCommandResult>(null !);
    }
}