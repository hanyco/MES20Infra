#region Created by HanyCo Infrastructure Code Generator with ♥
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Test.Hr.Dtos
{
    using System;
    using HanyCo.Infra.Markers;
    
    
    /// <summary>
    /// The data transfer object of GetByIdPersonResult
    /// </summary>
    public sealed partial class GetByIdPersonResult : IDto
    {
        
    public Int64 Id
    {
        get;
        set;
    }
        
    public String FirstName
    {
        get;
        set;
    }
        
    public String LastName
    {
        get;
        set;
    }
        
    public DateTime DateOfBirth
    {
        get;
        set;
    }
        
    public Int32 Height
    {
        get;
        set;
    }
    }
}
#endregion
