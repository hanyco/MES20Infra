using System.Threading.Tasks;
using Test.HumanResources.Dtos;

namespace Test.HumanResources.Commands
{
    public sealed partial class InsertPersonCommandHandler
    {
        public Task<InsertPersonCommandResult> HandleAsync(InsertPersonCommandParams command)
        {
            //var result = new InsertPersonCommandResult(dbResult);
            //return Task.FromResult(result);
        }
    }
}