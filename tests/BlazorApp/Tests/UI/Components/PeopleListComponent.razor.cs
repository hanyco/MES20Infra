namespace Test.HumanResources
{
    using Test.HumanResources.Mapper;
    using Test.HumanResources.Queries;
    using Test.HumanResources.Dtos;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed partial class PeopleListComponent
    {
        protected override async Task OnLoadAsync()
        {
            await Task.CompletedTask;
        }
    }
}