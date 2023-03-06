#nullable enable
#region Created by HanyCo Infrastructure Code Generator with ♥
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HanyCo.Infra.Designers.UI.Commands
{
    using HanyCo.Infra.Cqrs.Command;
    using System;
    using HanyCo.Infra.Mapping;
    
    
    /// <summary>
    /// The parameter of DeleteFormCommand
    /// </summary>
    public sealed partial class DeleteFormCommandParameter : ICommandParameter
    {
        
        public Guid Id { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteFormCommandParameter"/> class.
        /// </summary>
        public DeleteFormCommandParameter(Guid id)
        {
            this.Id = id;
        }
    }
    
    /// <summary>
    /// The result of DeleteFormCommand
    /// </summary>
    public sealed partial class DeleteFormCommandResult : ICommandResult
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteFormCommandResult"/> class.
        /// </summary>
        public DeleteFormCommandResult()
        {
        }
    }
    
    /// <summary>
    /// The handler of DeleteFormCommand
    /// </summary>
    public sealed partial class DeleteFormCommandHandler : ICommandHandler<DeleteFormCommandParameter,  DeleteFormCommandResult>
    {
        
        public IMapper Mapper { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteFormCommandHandler"/> class.
        /// </summary>
        public DeleteFormCommandHandler(IMapper mapper)
        {
            this.Mapper = mapper;
        }
    }
}
#endregion
