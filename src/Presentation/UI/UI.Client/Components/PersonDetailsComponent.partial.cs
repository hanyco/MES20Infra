//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace HumanResources
{
    using HumanResources.Mappers;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Library.DesignPatterns.Behavioral.Observation;
    using Microsoft.Extensions.Caching.Memory;
    using Library.Interfaces;

    public sealed partial class PersonDetailsComponent
    {
        public PersonDetailsComponent()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            // Call developer's method.
            await this.OnLoadAsync();
        }

        async Task SaveData()
        {
            if (DataContext.Id == default)
            {
                var @params = this.DataContext.ToInsertPerson();
                var cqParams = new HumanResources.Dtos.InsertPersonCommand(@params);
                var cqResult = await this._commandProcessor.ExecuteAsync<HumanResources.Dtos.InsertPersonCommand, HumanResources.Dtos.InsertPersonCommandResult>(cqParams);
            }
            else
            {
                var @params = this.DataContext.ToUpdatePerson();
                var cqParams = new HumanResources.Dtos.UpdatePersonCommand(@params);
                var cqResult = await this._commandProcessor.ExecuteAsync<HumanResources.Dtos.UpdatePersonCommand, HumanResources.Dtos.UpdatePersonCommandResult>(cqParams);
            }

            MessageComponent.Show("Save Data", "Date saved.");
        }

        public BlazorApp.Shared.MessageComponent MessageComponent { get; set; }

        [Microsoft.AspNetCore.Components.Parameter]
        public long? EntityId { get; set; }

        [Microsoft.AspNetCore.Components.Parameter]
        public Microsoft.AspNetCore.Components.EventCallback<long?>? EntityIdChanged { get; set; }

        public void BackButton_OnClick()
        {
            this._navigationManager.NavigateTo("/HumanResources/Person");
        }
    }
}