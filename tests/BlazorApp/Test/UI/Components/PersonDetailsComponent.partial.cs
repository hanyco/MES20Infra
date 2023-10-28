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
    
    
    public sealed partial class PersonDetailsComponent
    {
        
        protected override async Task OnInitializedAsync()
        {
            
            // Call developer's method.
            await this.OnLoadAsync();
        }
        
        public async void SaveButton_OnClick()
        {
            this.ValidateForm();
            var dto = this.DataContext;
            var cqParams = new Test.HumanResources.Dtos.InsertPersonParams(dto);
            OnInsertPersonCommandCalling(cqParams);
            var cqResult = await this._commandProcessor.ExecuteAsync<Test.HumanResources.Dtos.InsertPersonParams,Test.HumanResources.Dtos.InsertPersonResult>(cqParams);
            OnInsertPersonCommandCalled(cqParams, cqResult);

        }
        
        public void BackButton_OnClick()
        {
            this._navigationManager.NavigateTo("/HumanResourc
        }
        
        public PersonDetailsComponent()
        {
        }
    }
}
