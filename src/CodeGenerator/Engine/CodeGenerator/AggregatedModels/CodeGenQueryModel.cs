using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Queries;

using Library.Validations;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.AggregatedModels;

[Fluent]
[Immutable]
public class CodeGenQueryModel : ICodeGenCqrsModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeGenQueryModel"/> class.
    /// </summary>
    /// <param name="name">The CQRS Query name.</param>
    /// <param name="cqrsNameSpace">The namespace.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="param">The parameter.</param>
    /// <param name="result">The result.</param>
    /// <param name="dtos">The dtos.</param>
    private CodeGenQueryModel(
        in string name,
        in string? cqrsNameSpace,
        in string? dtoNameSpace,
        in CodeGenQueryHandler handler,
        in CodeGenQueryParams param,
        in CodeGenQueryResult result,
        in IEnumerable<string> securityKeys,
        params CodeGenDto[] dtos)
    {
        this.Name = name;
        this.CqrsNameSpace = cqrsNameSpace;
        this.DtoNameSpace = dtoNameSpace;
        this.Handler = handler;
        this.Param = param;
        this.Result = result;
        this.SecurityKeys = securityKeys;
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
    /// Gets the query handler.
    /// </summary>
    /// <value>The handler.</value>
    public CodeGenQueryHandler Handler { get; }

    /// <summary>
    /// Gets the name of query.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }

    /// <summary>
    /// Gets the query parameter.
    /// </summary>
    /// <value>The parameter.</value>
    public CodeGenQueryParams Param { get; }

    /// <summary>
    /// Gets the query result.
    /// </summary>
    /// <value>The result.</value>
    public CodeGenQueryResult Result { get; }

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

    public IEnumerable<string> SecurityKeys { get; set; }

    /// <summary>
    /// Creates a new instance of CodeGenQuery.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="cqrsNameSpace">The name space.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="param">The parameter.</param>
    /// <param name="result">The result.</param>
    /// <param name="dtos">The data transfer objects.</param>
    /// <returns></returns>
    public static CodeGenQueryModel New(
        in string name,
        in string? cqrsNameSpace,
        in string? dtoNameSpace,
        in CodeGenQueryHandler handler,
        in CodeGenQueryParams param,
        in CodeGenQueryResult result,
        IEnumerable<string> securityKeys,
        params CodeGenDto[] dtos) =>
        new(name.ArgumentNotNull(), cqrsNameSpace, dtoNameSpace, handler, param, result, securityKeys, dtos);

    public bool Equals(CodeGenQueryModel other) =>
        this.Name == other.Name && this.CqrsNameSpace == other.CqrsNameSpace;

    public override int GetHashCode() =>
        HashCode.Combine(this.Name, this.CqrsNameSpace);
}