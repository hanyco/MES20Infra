namespace Test.HumanResources.Queries
{
}

namespace Test.HumanResources.Dtos
{
}

namespace Test.HumanResources
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed partial class PeopleListComponent
    {
        public void Delete(Int64 id)
        {
        }

        protected override async Task OnLoadAsync()
        {
            await Task.CompletedTask;
        }
    }
}