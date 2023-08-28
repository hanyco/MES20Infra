using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Commands;

using Library.Validations;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.AggregatedModels;

[Fluent]
[Immutable]
public readonly struct CodeGenCommandModel : ICodeGenCqrsModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeGenCommandModel"/> class.
    /// </summary>
    /// <param name="name">The CQRS Command name.</param>
    /// <param name="cqrsNameSpace">The namespace.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="param">The parameter.</param>
    /// <param name="result">The result.</param>
    /// <param name="dtos">The dtos.</param>
    private CodeGenCommandModel(
        in string name,
        in string? cqrsNameSpace,
        in string? dtoNameSpace,
        in CodeGenCommandHandler handler,
        in CodeGenCommandParams param,
        in CodeGenCommandResult result,
        params CodeGenDto[] dtos)
    {
        this.Name = name;
        this.CqrsNameSpace = cqrsNameSpace;
        this.DtoNameSpace = dtoNameSpace;
        this.Handler = handler;
        this.Param = param;
        this.Result = result;
        this.Dtos = dtos;
    }

    /// <summary>
    /// Gets the CQRS name space.
    /// </summary>
    /// <value>The CQRS name space.</value>
    public string? CqrsNameSpace { get; }

    /// <summary>
    /// Gets the dto name space.
    /// </summary>
    /// <value>The dto name space.</value>
    public string? DtoNameSpace { get; }

    /// <summary>
    /// Gets the data transfer objects.
    /// </summary>
    /// <value>The dtos.</value>
    public IEnumerable<CodeGenDto> Dtos { get; }

    /// <summary>
    /// Gets the Command handler.
    /// </summary>
    /// <value>The handler.</value>
    public CodeGenCommandHandler Handler { get; }

    /// <summary>
    /// Gets the name of Command.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }

    /// <summary>
    /// Gets the Command parameter.
    /// </summary>
    /// <value>The parameter.</value>
    public CodeGenCommandParams Param { get; }

    /// <summary>
    /// Gets the Command result.
    /// </summary>
    /// <value>The result.</value>
    public CodeGenCommandResult Result { get; }

    /// <summary>
    /// Gets the CQRS segregates.
    /// </summary>
    /// <value>The segregates.</value>
    public IEnumerable<ICodeGenCqrsSegregate> Segregates
    {
        get
        {
            yield return this.Param;
            yield return this.Result;
            yield return this.Handler;
        }
    }

    /// <summary>
    /// Creates a new instance of CodeGenCommand.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="cqrsNameSpace">The name space.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="param">The parameter.</param>
    /// <param name="result">The result.</param>
    /// <param name="dtos">The data transfer objects.</param>
    /// <returns></returns>
    public static CodeGenCommandModel New(string name,
        string? cqrsNameSpace,
        string? dtoNameSpace,
        CodeGenCommandHandler handler,
        CodeGenCommandParams param,
        CodeGenCommandResult result,
        params CodeGenDto[] dtos)
    {
        _ = name.ArgumentNotNull(nameof(name));

        if (!name.EndsWith("Command"))
        {
            name = $"{name}Command";
        }

        return new CodeGenCommandModel(name, cqrsNameSpace, dtoNameSpace, handler, param, result, dtos);
    }

    public bool Equals(CodeGenCommandModel other)
        => this.Name == other.Name && this.CqrsNameSpace == other.CqrsNameSpace;

    public override int GetHashCode()
        => HashCode.Combine(this.Name, this.CqrsNameSpace);
}