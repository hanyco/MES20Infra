//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Test.Hr.Queries
{
    using HanyCo.Infra.Cqrs;
    using Test.Hr.Dtos;
    using System.Collections.Generic;
    
    
    /// <summary>
    /// The result of GetAllPeopleQuery
    /// </summary>
    public sealed partial class GetAllPeopleQueryResult : IQueryResult<IEnumerable<GetAllPeopleResult>>
    {
        
    public IEnumerable<GetAllPeopleResult> Result
    {
        get;
        
    }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllPeopleQueryResult"/> class.
        /// </summary>
        public GetAllPeopleQueryResult(IEnumerable<GetAllPeopleResult> result)
        {
            this.Result = result;
        }
    }
}
