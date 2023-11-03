//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Test.HumanResources
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Library.DesignPatterns.Behavioral.Observation;
    using Microsoft.Extensions.Caching.Memory;
    using Library.Interfaces;

    public sealed partial class PeopleListComponent
    {
        public void NewButton_OnClick()
        {
            this._navigationManager.NavigateTo("/HumanResources/Person/details");
        }

        public void Edit(long id)
        {
            this._navigationManager.NavigateTo($"/HumanResources/Person/details{id.ToString()}");
        }

        protected override async Task OnInitializedAsync()
        {
            // Setup segregation parameters
            var @params = new GetAllPeopleParams();
            var cqParams = new Test.HumanResources.Dtos.GetAllPeopleQueryParams(@params);
            // Invoke the query handler to retrieve all entities
            var cqResult = await this._queryProcessor.ExecuteAsync<Test.HumanResources.Dtos.GetAllPeopleQueryResult>(cqParams);
            // Now, set the data context.
            this.DataContext = cqResult.Result.ToViewModel();
            // Call developer's method.
            await this.OnLoadAsync();
        }

        public PeopleListComponent()
        {
        }
    }
}