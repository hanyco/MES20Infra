#region Created by HanyCo Infrastructure Code Generator with ♥
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
    using Test.Hr.Dtos;
    
    
    /// <summary>
    /// The result of DeletePersonCommand
    /// </summary>
    public sealed partial class DeletePersonCommandResult : ICommandResult
    {
        
    public DeleteDeletePersonResult Result
    {
        get;
        
    }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DeletePersonCommandResult"/> class.
        /// </summary>
        public DeletePersonCommandResult(DeleteDeletePersonResult result)
        {
            this.Result = result;
        }
    }
}
#endregion
