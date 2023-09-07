//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Test.Hr.Commands
{
    using Library.Cqrs.Models.Commands;
    using Library.Cqrs.Models.Queries;
    
    
    /// <summary>
    /// The handler of DeletePersonCommand
    /// </summary>
    public sealed partial class DeletePersonCommandHandler : ICommandHandler<DeletePersonCommandParameter, DeletePersonCommandResult>
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
        /// Initializes a new instance of the <see cref="DeletePersonCommandHandler"/> class.
        /// </summary>
        public DeletePersonCommandHandler(ICommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            this.CommandProcessor = commandProcessor;
            this.QueryProcessor = queryProcessor;
        }
    }
}
