using System.Threading.Tasks;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Commands
{
    public sealed partial class UpdatePersonCommandHandler
    {
        public Task<UpdatePersonCommandResult> HandleAsync(UpdatePersonCommandParams command)
        {
            var result = new UpdatePersonCommandResult(dbResult);
            return Task.FromResult(result);
        }
    }
}