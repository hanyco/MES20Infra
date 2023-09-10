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
    using HanyCo.Infra.Security.Markers;
    
    
    /// <summary>
    /// The handler of GetByIdPersonQuery
    /// </summary>
    [SecurityDescriptorAttribute(Key="GetByIdPersonQuery")]
    public sealed partial class GetByIdPersonQueryHandler : IQueryHandler<GetByIdPersonQueryParameter, GetByIdPersonQueryResult>
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
        /// Initializes a new instance of the <see cref="GetByIdPersonQueryHandler"/> class.
        /// </summary>
        public GetByIdPersonQueryHandler(ICommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            this.CommandProcessor = commandProcessor;
            this.QueryProcessor = queryProcessor;
        }
    }
}
