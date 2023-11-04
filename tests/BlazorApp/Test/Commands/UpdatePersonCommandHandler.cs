using System.Threading.Tasks;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Commands
{
    public sealed partial class UpdatePersonCommandHandler
    {
        public Task<UpdatePersonCommandResult> HandleAsync(UpdatePersonCommandParams commmand)
        {
            var result = new UpdatePersonQueryResult(dbResult);
            return Task.FromResult(result);
        }
    }
}