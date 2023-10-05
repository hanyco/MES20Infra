using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;

using Library.CodeGeneration;
using Library.Cqrs.Models.Commands;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Commands;

[Fluent]
[Immutable]
public sealed record CodeGenCommandHandler : CodeGenCqrsSegregateType
{
    private CodeGenCommandHandler(in CodeGenCommandParams CommandParam, in CodeGenCommandResult CommandResult, in IEnumerable<CodeGenProp> props, IEnumerable<string> securityKeys)
        : base("Handler", securityKeys, null, props) => (this.CommandParam, this.CommandResult) = (CommandParam, CommandResult);

    public CodeGenCommandParams CommandParam { get; }
    public CodeGenCommandResult CommandResult { get; }

    protected override bool HasPartialClass
    {
        get => true;
        set
        {
            if (!value)
            {
                throw new("The Handler class must have partial");
            }

            base.HasPartialClass = value;
        }
    }

    public override SegregationRole Role { get; } = SegregationRole.CommandHandler;

    public static CodeGenCommandHandler New(CodeGenCommandParams CommandParam, CodeGenCommandResult CommandResult, IEnumerable<string> securityKeys, params (Type Type, string Name)[] props)
        => new(CommandParam, CommandResult, props.Select(p => CodeGenProp.New(new CodeGenType(p.Type), p.Name, false, false)), securityKeys);

    protected override IEnumerable<string> OnGetRequiredInterfaces(string cqrsName)
    {
        yield return $"{TypePath.New(typeof(ICommandHandler<,>))}<{cqrsName}Parameter, {cqrsName}Result>";
    }

    protected override Partials OnGetPartials()
        => base.OnGetPartials() | Partials.Handller;
    protected override Partials OnGetValidPartials()
        => base.OnGetValidPartials() | Partials.Handller;
}