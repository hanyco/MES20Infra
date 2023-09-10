using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using Library.Cqrs.Models.Commands;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Commands
{
    [Fluent]
    public sealed record CodeGenCommandParams : CodeGenCqrsSegregateType
    {
        private CodeGenCommandParams(IEnumerable<string>? securityKeys,IEnumerable<CodeGenProp>? props = null)
            : base("Parameter", securityKeys, null, props)
        {
        }

        public override SegregationRole Role { get; } = SegregationRole.CommandParameter;

        public static CodeGenCommandParams New(IEnumerable<string>? securityKeys, IEnumerable<CodeGenProp>? props = null)
            => new(securityKeys, props);

        protected override IEnumerable<string> OnGetRequiredInterfaces(string cqrsName)
        {
            yield return typeof(ICommand).FullName!;
        }
    }
}
