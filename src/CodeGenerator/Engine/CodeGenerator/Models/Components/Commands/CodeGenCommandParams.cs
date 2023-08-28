using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using Library.Cqrs.Models.Commands;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Commands
{
    [Fluent]
    public sealed record CodeGenCommandParams : CodeGenCqrsSegregateType
    {
        private CodeGenCommandParams(IEnumerable<CodeGenProp>? props = null)
            : base("Parameter", null, props)
        {
        }

        public override SegregationRole Role { get; } = SegregationRole.CommandParameter;

        public static CodeGenCommandParams New(IEnumerable<CodeGenProp>? props = null)
            => new(props);

        protected override IEnumerable<string> OnGetRequiredInterfaces(string cqrsName)
        {
            yield return typeof(ICommand).FullName!;
        }
    }
}
