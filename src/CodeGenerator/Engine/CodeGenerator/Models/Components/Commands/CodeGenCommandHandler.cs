using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using Library.CodeGeneration.Models;
using Library.Cqrs.Models.Commands;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Commands;

[Fluent]
[Immutable]
public sealed record CodeGenCommandHandler : CodeGenCqrsSegregateType
{
    private CodeGenCommandHandler(in CodeGenCommandParameter CommandParam, in CodeGenCommandResult CommandResult, in IEnumerable<CodeGenProp> props)
        : base("Handler", null, props) => (this.CommandParam, this.CommandResult) = (CommandParam, CommandResult);

    public CodeGenCommandParameter CommandParam { get; }
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

    public static CodeGenCommandHandler New(CodeGenCommandParameter CommandParam, CodeGenCommandResult CommandResult)
        => new(CommandParam, CommandResult, Enumerable.Empty<CodeGenProp>());

    public static CodeGenCommandHandler New(CodeGenCommandParameter CommandParam, CodeGenCommandResult CommandResult, params (string Type, string Name)[] props)
        => new(CommandParam, CommandResult, props.Select(p => CodeGenProp.New(new CodeGenType(p.Type), p.Name, false, false)));

    public static CodeGenCommandHandler New(CodeGenCommandParameter CommandParam, CodeGenCommandResult CommandResult, params (Type Type, string Name)[] props)
        => new(CommandParam, CommandResult, props.Select(p => CodeGenProp.New(new CodeGenType(p.Type), p.Name, false, false)));

    public static CodeGenCommandHandler New(CodeGenCommandParameter CommandParam, CodeGenCommandResult CommandResult, params (Type Type, string Name, bool HasSetter)[] props)
        => new(CommandParam, CommandResult, props.Select(p => CodeGenProp.New(new CodeGenType(p.Type), p.Name, false, false, hasSetter: p.HasSetter)));

    protected override IEnumerable<string> OnGetRequiredIntefaces(string cqrsName)
    {
        yield return $"{TypePath.New(typeof(ICommandHandler<,>))}<{cqrsName}Parameter, {cqrsName}Result>";
    }

    protected override Partials OnGetPartials()
        => base.OnGetPartials() | Partials.Handller;
    protected override Partials OnGetValidPartials()
        => base.OnGetValidPartials() | Partials.Handller;
}
