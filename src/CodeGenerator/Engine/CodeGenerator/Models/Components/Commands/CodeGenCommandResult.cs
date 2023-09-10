using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using Library.CodeGeneration.Models;
using Library.Cqrs.Models.Commands;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Commands
{
    [Fluent]
    public sealed record CodeGenCommandResult : CodeGenCqrsSegregateType
    {
        private CodeGenCommandResult(IEnumerable<string>? securityKeys,IEnumerable<CodeGenProp>? props = null)
            : base("Result",securityKeys, null, props)
        {
        }

        public override SegregationRole Role { get; } = SegregationRole.CommandResult;

        public static CodeGenCommandResult New(IEnumerable<string> securityKeys, IEnumerable<CodeGenProp>? props = null)
            => new(securityKeys, props);

        protected override IEnumerable<string> OnGetRequiredInterfaces(string cqrsName)
        {
            yield return $"{TypePath.New(typeof(ICommandResult))}";
        }
    }
}
