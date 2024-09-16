namespace HumanResources
{
    using HumanResources.Mappers;
    using HumanResources.Queries;
    using HumanResources.Dtos;
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