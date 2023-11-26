namespace HumanResources
{
    using HumanResources.Mapper;
    using HumanResources.Queries;
    using HumanResources.Dtos;
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