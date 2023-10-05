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
            this._navigationManager.NavigateTo("/HumanResources/Person/details" + "/" + id.ToString());
        }
        
        protected override async Task OnInitializedAsync()
        {
            // Setup segregation parameters
            var paramsParams = new Dtos.GetAllPeopleParams();
            var cqParams = new Queries.GetAllPeopleQueryParameter(paramsParams);
            
            // Let the developer know what's going on.
            cqParams = OnCallingGetAllPeopleQuery(cqParams);
            
            // Invoke the query handler to retrieve all entities
            var cqResult = await this._queryProcessor.ExecuteAsync(cqParams);
            
            // Let's inform the developer about the result.
            cqResult = OnCalledGetAllPeopleQuery(cqParams, cqResult);
            
            // Now, set the data context.
            this.DataContext = cqResult.Result.ToPersonDto();
            
            
            // Call developer's method.
            await this.OnLoadAsync();
        }
        
    public System.Int64 Id
    {
        get;
        set;
    }
        
    public System.String FirstName
    {
        get;
        set;
    }
        
    public System.String LastName
    {
        get;
        set;
    }
        
    public System.DateTime DateOfBirth
    {
        get;
        set;
    }
        
    public System.Int32 Height
    {
        get;
        set;
    }
        
        public PeopleListComponent()
        {
        }
    }
}
