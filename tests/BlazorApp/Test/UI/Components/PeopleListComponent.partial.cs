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
            this._navigationManager.NavigateTo("/HumanResources/PersonDetails");
        }
        
        public void Edit(long id)
        {
            this._navigationManager.NavigateTo("/HumanResources/PersonDetails/" + id.ToString());
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
