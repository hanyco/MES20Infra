using System.IO;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Actors;
using HanyCo.Infra.CodeGeneration.CodeGenerator.AggregatedModels;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Commands;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components.Queries;
using HanyCo.Infra.CodeGeneration.Helpers;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Cqrs.Models.Commands;
using Library.Cqrs.Models.Queries;
using Library.Helpers.CodeGen;
using Library.Validations;

namespace HanyCo.Infra.UI.Services.Imp;

[Service]
internal sealed class CqrsCodeGeneratorService : ICqrsCodeGeneratorService
{
    private readonly ICqrsCommandService _CommandService;
    private readonly IPropertyService _PropertyService;
    private readonly ICqrsQueryService _QueryService;
    private readonly InfraReadDbContext _ReadDbContext;

    public CqrsCodeGeneratorService(
        ICqrsCommandService commandService,
        ICqrsQueryService queryService,
        IPropertyService propertyService,
        InfraReadDbContext readDbContext)
    {
        this._CommandService = commandService;
        this._QueryService = queryService;
        this._PropertyService = propertyService;
        this._ReadDbContext = readDbContext;
    }

    public IEnumerable<GenerateAllCqrsCodesResultItem> GenerateAllCodes(CqrsCqrsGenerateCodesParams parametes, CqrsCodeGenerateCodesConfig? config)
    {
        var (entityName, _, _, _) = parametes.ArgumentNotNull();
        config ??= new();
        if (config.ShouldGenerateGetAll)
        {
            yield return new($"GetAll{StringHelper.Pluralize(entityName)}", this.GenerateGetAllCode(parametes));
        }
        if (config.ShouldGenerateGetById)
        {
            yield return new($"Get{entityName}ById", this.GenerateGetByIdCode(parametes));
        }
        if (config.ShouldGenerateCreate)
        {
            yield return new($"Create{entityName}", this.GenerateCreateCode(parametes));
        }
        if (config.ShouldGenerateUpdate)
        {
            yield return new($"Update{entityName}", this.GenerateUpdateCode(parametes));
        }
        if (config.ShouldGenerateDelete)
        {
            yield return new($"Delete{entityName}", this.GenerateDeleteCode(parametes));
        }
    }

    public async Task<Codes> GenerateCodeAsync(CqrsViewModelBase viewModel, CqrsCodeGenerateCodesConfig? config = null)
            => viewModel.ArgumentNotNull() switch
            {
                CqrsQueryViewModel queryViewModel => await GenerateQueryAsync(queryViewModel),
                CqrsCommandViewModel commandViewModel => await GenerateCommandAsync(commandViewModel),
                _ => throw new NotSupportedException()
            };

    public Codes GenerateCreateCode(in CqrsCodeGenerateCrudParams parametes)
    {
        (var table, var cqrsNameSpace, var dtoNameSpace) = parametes.ArgumentNotNull();
        var tableName = table.Value.Name!.Trim();
        var paramDto = CodeGenDto.New($"{tableName}ParamDto");
        foreach (var child in table.Children.First().Children)
        {
            var column = child.Value.As<DbColumnViewModel>()!;
            var type = new CodeGenType(PropertyTypeHelper.FromDbType(column.DbType).ToFullTypeName());
            _ = paramDto.AddProp(type, column.Name!, isNullable: column.IsNullable);
        }
        var param = CodeGenCommandParameter.New().AddProp(paramDto, "Dto");
        var result = CodeGenCommandResult.New().AddProp(typeof(Guid), "Id");
        var handler = CodeGenCommandHandler.New(param, result);
        var query = CodeGenCommandModel.New(
            $"Create{tableName}",
            cqrsNameSpace,
            dtoNameSpace,
            handler,
            param,
            result,
            paramDto);
        return query.GenerateCode();
    }

    public Codes GenerateDeleteCode(in CqrsCodeGenerateCrudParams parametes)
    {
        (var table, var cqrsNameSpace, var dtoNameSpace) = parametes.ArgumentNotNull();
        var tableName = table.Value.Name!.Trim();
        var paramDto = CodeGenDto.New($"{tableName}ParamDto").AddProp(typeof(Guid), "Id");
        var param = CodeGenCommandParameter.New().AddProp(paramDto, "Dto");
        var result = CodeGenCommandResult.New();
        var handler = CodeGenCommandHandler.New(param, result);
        var query = CodeGenCommandModel.New(
            $"Delete{tableName}",
            cqrsNameSpace,
            dtoNameSpace,
            handler,
            param,
            result,
            paramDto);
        return query.GenerateCode();
    }

    public Codes GenerateGetAllCode(in CqrsCodeGenerateCrudParams parametes)
    {
        (var table, var cqrsNameSpace, var dtoNameSpace) = parametes.ArgumentNotNull();
        var tableName = table.Value.Name.NotNull().Trim();
        var resultDto = CodeGenDto.New($"{tableName}ResultDto").AddProp(typeof(Guid), "Id");
        foreach (var child in table.Children.First().Children)
        {
            var column = child.Value.As<DbColumnViewModel>()!;
            var type = new CodeGenType(PropertyTypeHelper.FromDbType(column.DbType).ToFullTypeName());
            _ = resultDto.AddProp(type, column.Name!, isNullable: column.IsNullable);
        }
        var param = CodeGenQueryParam.New();
        var result = CodeGenQueryResult.New().AddProp(resultDto, "Result", isList: true);
        var handler = CodeGenQueryHandler.New(param, result);
        var query = CodeGenQueryModel.New(
            $"GetAll{StringHelper.Pluralize(tableName)}",
            cqrsNameSpace,
            dtoNameSpace,
            handler,
            param,
            result,
            resultDto);
        return query.GenerateCode();
    }

    public Codes GenerateGetByIdCode(in CqrsCodeGenerateCrudParams parametes)
    {
        (var table, var cqrsNameSpace, var dtoNameSpace) = parametes.ArgumentNotNull();
        var tableName = table.Value.Name!.Trim();
        var resultDto = CodeGenDto.New($"{tableName}ResultDto").AddProp(typeof(Guid), "Id");
        foreach (var child in table.Children.First().Children)
        {
            var column = child.Value.As<DbColumnViewModel>()!;
            var type = new CodeGenType(PropertyTypeHelper.FromDbType(column.DbType).ToFullTypeName());
            _ = resultDto.AddProp(type, column.Name!, isNullable: column.IsNullable);
        }
        var param = CodeGenQueryParam.New().AddProp(typeof(Guid), "Id");
        var result = CodeGenQueryResult.New().AddProp(resultDto, "Result", isNullable: true);
        var handler = CodeGenQueryHandler.New(param, result);
        var query = CodeGenQueryModel.New(
            $"Get{tableName}",
            cqrsNameSpace,
            dtoNameSpace,
            handler,
            param,
            result,
            resultDto);
        return query.GenerateCode();
    }

    public Codes GenerateUpdateCode(in CqrsCodeGenerateCrudParams parametes)
    {
        (var table, var cqrsNameSpace, var dtoNameSpace) = parametes.ArgumentNotNull();
        var tableName = table.Value.Name!.Trim();
        var paramDto = CodeGenDto.New($"{tableName}ParamDto").AddProp(typeof(Guid), "Id");
        foreach (var child in table.Children.First().Children)
        {
            var column = child.Value.As<DbColumnViewModel>()!;
            var type = new CodeGenType(PropertyTypeHelper.FromDbType(column.DbType).ToFullTypeName());
            _ = paramDto.AddProp(type, column.Name!, isNullable: column.IsNullable);
        }
        var param = CodeGenCommandParameter.New().AddProp(typeof(Guid), "Id").AddProp(paramDto, "Dto");
        var result = CodeGenCommandResult.New();
        var handler = CodeGenCommandHandler.New(param, result);
        var query = CodeGenCommandModel.New(
            $"Update{tableName}",
            cqrsNameSpace,
            dtoNameSpace,
            handler,
            param,
            result,
            paramDto);
        return query.GenerateCode();
    }

    //UNDONE CQRS Code Generator Service SaveToDatabaseAsync.
    public Task SaveToDatabaseAsync(CqrsCqrsGenerateCodesParams parametes, CqrsCodeGenerateCodesConfig config)
        => throw new NotImplementedException();

    public async Task SaveToDiskAsync(CqrsViewModelBase viewModel, string path, CqrsCodeGenerateCodesConfig? config = null)
    {
        var codes = await this.GenerateCodeAsync(viewModel, config);
        if (codes?.Any() is not true)
        {
            return;
        }

        foreach (var code in codes)
        {
            await File.WriteAllTextAsync(Path.Combine(path, code.FileName), code.Statement);
        }
    }

    private static CodeGenDto ConvertViewModelToCodeGen(DtoViewModel resultViewModel)
    {
        var result = CodeGenDto.New(TypeMemberNameHelper.GetFullName(resultViewModel.NameSpace, resultViewModel.Name));
        foreach (var prop in resultViewModel.Properties)
        {
            _ = result.AddProp(CodeGenType.New(prop.TypeFullName), prop.Name!, prop.IsList ?? false, prop.IsNullable ?? false);
        }
        return result;
    }

    private static CodeGenDto ExtractParamsDto(in CqrsViewModelBase viewModel)
        => ConvertViewModelToCodeGen(viewModel.ParamDto);

    private static CodeGenDto ExtractResultDto(in CqrsViewModelBase viewModel)
        => ConvertViewModelToCodeGen(viewModel.ResultDto);

    private static async Task<Codes> GenerateCommandAsync(CqrsCommandViewModel commandViewModel)
    {
        Check.IfArgumentNotNull(commandViewModel?.Name);

        var paramsDto = ExtractParamsDto(commandViewModel);
        var resultDto = ExtractResultDto(commandViewModel);
        var commandParam = CodeGenCommandParameter.New().AddProp(paramsDto, "dto");
        var commandResult = CodeGenCommandResult.New().AddProp(resultDto, "Result");
        var commandHandler = CodeGenCommandHandler.New(commandParam, commandResult);

        var query = CodeGenCommandModel.New(commandViewModel.Name,
                                            commandViewModel.CqrsNameSpace,
                                            commandViewModel.DtoNameSpace,
                                            commandHandler,
                                            commandParam,
                                            commandResult);
        return await Task.FromResult(query.GenerateCode());
    }

    private static async Task<Codes> GenerateQueryAsync(CqrsQueryViewModel queryViewModel)
    {
        Check.IfArgumentNotNull(queryViewModel?.Name);

        var paramsDto = ExtractParamsDto(queryViewModel);
        var resultDto = ExtractResultDto(queryViewModel);
        var queryParam = CodeGenQueryParam.New().AddProp(paramsDto, "dto");
        var queryResult = CodeGenQueryResult.New().AddProp(resultDto, "Result");
        var queryHandler = CodeGenQueryHandler.New(queryParam,
                                                   queryResult,
                                                   (typeof(ICommandProcessor), "CommandProcessor"),
                                                   (typeof(IQueryProcessor), "QueryProcessor"));

        var query = CodeGenQueryModel.New(queryViewModel.Name,
                                          queryViewModel.CqrsNameSpace,
                                          queryViewModel.DtoNameSpace,
                                          queryHandler,
                                          queryParam,
                                          queryResult);
        return await Task.FromResult(query.GenerateCode());
    }
}