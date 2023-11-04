using System.Threading.Tasks;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Commands
{
    public sealed partial class DeletePersonCommandHandler
    {
        public Task<DeletePersonCommandResult> HandleAsync(DeletePersonCommandParams commmand)
        {
            var result = new DeletePersonQueryResult(dbResult);
            return Task.FromResult(result);
        }
    }
}