using System.Threading.Tasks;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Commands
{
    public sealed partial class DeletePersonCommandHandler
    {
        public Task<DeletePersonCommandResult> HandleAsync(DeletePersonCommandParams command)
        {
            return Task.FromResult<DeletePersonCommandResult>(null!);
        }
    }
}