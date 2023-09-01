#region Created by HanyCo Infrastructure Code Generator with ♥
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
    using Library.Cqrs.Models.Queries;
    using Library.Cqrs.Models.Commands;
    
    
    /// <summary>
    /// The handler of GetAllPeopleQuery
    /// </summary>
    public sealed partial class GetAllPeopleQueryHandler : IQueryHandler<GetAllPeopleQueryParameter, GetAllPeopleQueryResult>
    {
        
    public ICommandProcessor CommandProcessor
    {
        get;
        
    }
        
    public IQueryProcessor QueryProcessor
    {
        get;
        
    }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllPeopleQueryHandler"/> class.
        /// </summary>
        public GetAllPeopleQueryHandler(ICommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            this.CommandProcessor = commandProcessor;
            this.QueryProcessor = queryProcessor;
        }
    }
}
#endregion
