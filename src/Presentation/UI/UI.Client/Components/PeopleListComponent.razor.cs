namespace HumanResources.Dtos
{
    using HumanResource.Mapper;
    using HumanResource.Queries;
    using HumanResource.Dtos;
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