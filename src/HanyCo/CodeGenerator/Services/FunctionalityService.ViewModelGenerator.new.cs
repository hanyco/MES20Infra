using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Results;
using Library.Threading;

using MediatR;

using Services.CodeGen.Helpers;

namespace Services;

internal partial class FunctionalityService
{
    public async Task<Result<FunctionalityViewModel?>> GenerateViewModelAsync([DisallowNull] FunctionalityViewModel viewModel, CancellationToken token = default)
    {
        // Validate the model
        if (!this.Validate(viewModel).TryParse(out var validationResult))
        {
            return validationResult!;
        }

        var initResult = initialize(viewModel);
        if (!initResult.IsSucceed)
        {
            return Result.From(initResult, viewModel)!;
        }
        var data = initResult.Value;

        InitializeWorkspace(data);
        await this.CreateGetAllQuery(data, token).ConfigureAwait(false);

        return data.Result!;

        static Result<CreationData> initialize(FunctionalityViewModel viewModel)
        {
            var result = new CreationData(viewModel);
            if (result.ViewModel.SourceDto.NameSpace.IsNullOrEmpty())
            {
                result.ViewModel.SourceDto.NameSpace = TypePath.Combine(result.ViewModel.SourceDto.NameSpace, result.ViewModel.SourceDto.Module.Name!.Remove(" "), "Dtos");
            }

            // Return a success result with the result and cancellationTokenSource
            return Result.Success(result)!;
        }
    }

    private static void InitializeWorkspace(CreationData data)
    {
        data.ViewModel.MapperGeneratorViewModel.Arguments.Clear();
        data.ViewModel.Codes.Clear();
    }

    private async Task CreateGetAllQuery(CreationData data, CancellationToken token)
    {
        var name = $"GetAll{StringHelper.Pluralize(CommonHelpers.Purify(data.ViewModel.SourceDto.Name))}";
        var result = await TaskRunner.StartWith(data)
            .Then(createViewModel)
            .Then(createResult)
            .Then(createParams)
            .Then(createHandleMethodBody)
            .Then(setupSecurity)
            .RunAsync(token);

        async Task createViewModel(CreationData data, CancellationToken token)
        {
            data.ViewModel.GetAllQueryViewModel = await this._queryService.CreateAsync(cancellationToken: token);
            data.ViewModel.GetAllQueryViewModel.Name = $"{name}Query";
            data.ViewModel.GetAllQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetAllQueryViewModel.CqrsNameSpace = TypePath.Combine(GetRootNameSpace(data), "Queries");
            data.ViewModel.GetAllQueryViewModel.DtoNameSpace = TypePath.Combine(GetRootNameSpace(data), "Dtos");
            data.ViewModel.GetAllQueryViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetAllQueryViewModel.FriendlyName = data.ViewModel.GetAllQueryViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.GetAllQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetAllQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, cancellationToken: token);
            data.ViewModel.GetAllQueryViewModel.MapperNameSpace = GetMapperNameSpace(data);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ParamsDto = RawDto(data, false);
            data.ViewModel.GetAllQueryViewModel.ParamsDto.Name = $"{name}Query";
            data.ViewModel.GetAllQueryViewModel.ParamsDto.BaseType = TypePath.New<IRequest>([data.ViewModel.GetAllQueryViewModel.ResultDto.Name!]);
            data.ViewModel.GetAllQueryViewModel.ParamsDto.IsParamsDto = true;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ResultDto = RawDto(data, false);
            data.ViewModel.GetAllQueryViewModel.ResultDto.Name = $"{name}QueryResult";
            data.ViewModel.GetAllQueryViewModel.ResultDto.IsResultDto = true;
            data.ViewModel.GetAllQueryViewModel.ResultDto.Properties.Add(new(StringHelper.Pluralize(CommonHelpers.Purify(data.ViewModel.SourceDto.Name!)), PropertyType.Dto)
            {
                IsList = true,
                IsNullable = true,
                TypeFullName = data.ViewModel.SourceDto.FullName,
            });
        }
        
        static void createHandleMethodBody(CreationData data) =>
            data.ViewModel.GetAllQueryViewModel.HandleMethodBody = CodeSnippets.CreateGetAllQueryHandleMethodBody(data.ViewModel.GetAllQueryViewModel);

        void setupSecurity(CreationData data) =>
            AddClaimViewModel(data.ViewModel.GetAllQueryViewModel, data);
    }

    #region Private tool methods

    [DebuggerStepThrough]
    private static void AddClaimViewModel(InfraViewModelBase viewModel, string? key, object? value, ClaimViewModel? parent)
    => new ClaimViewModel(key ?? viewModel.Name, value, parent).With(claim => viewModel.SecurityClaims.Add(claim));

    [DebuggerStepThrough]
    private static void AddClaimViewModel(InfraViewModelBase viewModel, string? key, object? value, InfraViewModelBase? parent)
        => AddClaimViewModel(viewModel, key, value, parent?.SecurityClaims.FirstOrDefault());

    [DebuggerStepThrough]
    private static void AddClaimViewModel(InfraViewModelBase viewModel, CreationData data)
        => AddClaimViewModel(viewModel, viewModel.Name, null, data.ViewModel.SourceDto);

    [DebuggerStepThrough]
    private static string GetMapperNameSpace(CreationData data)
        => TypePath.Combine(GetRootNameSpace(data), "Mapper");

    [DebuggerStepThrough]
    private static string GetRootNameSpace(CreationData data)
        => data.ViewModel.SourceDto.NameSpace.TrimEnd(".Dtos").TrimEnd(".Dto");

    [DebuggerStepThrough]
    private static DtoViewModel RawDto(CreationData data, bool addTableColumns = false)
    {
        // Create an initial DTO based on the input data
        var dto = new DtoViewModel(data.ViewModel.SourceDto.Id, data.ViewModel.Name!)
        {
            Comment = data.COMMENT, // Set DTO comment
            NameSpace = TypePath.Combine(GetRootNameSpace(data), "Dtos"), // Set DTO namespace
            Functionality = data.ViewModel,
            DbObject = data.ViewModel.SourceDto.DbObject
        }
        .With(x => x.Module.Id = data.ViewModel.SourceDto.Module.Id); // Set DTO module ID

        // If the addTableColumns parameter is true, add table columns to the DTO
        return addTableColumns ? AddColumns(data, dto) : dto;

        // Internal method for adding table columns to DTO
        static DtoViewModel AddColumns(CreationData data, DtoViewModel dto)
        {
            // Add columns to DTO
            _ = dto.Properties.ClearAndAddRange(data.ViewModel.SourceDto.Properties.Select(x => new PropertyViewModel(x) { Comment = data.COMMENT }));
            return dto;
        }
    }

    #endregion Private tool methods
}