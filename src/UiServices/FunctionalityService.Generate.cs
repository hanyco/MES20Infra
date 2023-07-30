using System.Diagnostics.CodeAnalysis;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.CodeGeneration.Models;
using Library.Exceptions;
using Library.Exceptions.Validations;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;

namespace Services;

internal sealed partial class FunctionalityService
{
    public async Task<Result<Codes>> GenerateCodesAsync(FunctionalityViewModel viewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs? args = null, CancellationToken token = default)
    {
        if (!validate(viewModel).TryParse(out var result))
        {
            return Result<Codes>.From(result, Codes.Empty);
        }
        var codeResult = (args?.UpdateModelView ?? false) ? viewModel.CodesResults : new();
        var results = await generateCodes(viewModel, codeResult, token);

        return results.Merge();

        static Result<FunctionalityViewModel> validate(FunctionalityViewModel viewModel) =>
            viewModel.Check()
                .ArgumentNotNull()
                .NotNull(x => x.GetAllQueryViewModel)
                .NotNull(x => x.GetByIdQueryViewModel)
                .NotNull(x => x.InsertCommandViewModel)
                .NotNull(x => x.UpdateCommandViewModel)
                .NotNull(x => x.DeleteCommandViewModel)
                .NotNull(x => x.BlazorListComponentViewModel)
                .NotNull(x => x.BlazorDetailsComponentViewModel);

        async Task<FunctionalityViewModelCodesResults> generateCodes(FunctionalityViewModel viewModel, FunctionalityViewModelCodesResults results, CancellationToken token)
        {
            results.GetAllQueryCodes = await this._cqrsCodeService.GenerateCodeAsync(viewModel.GetAllQueryViewModel, token: token);
            results.GetByIdQueryCodes = await this._cqrsCodeService.GenerateCodeAsync(viewModel.GetByIdQueryViewModel, token: token);
            results.InsertCommandCodes = await this._cqrsCodeService.GenerateCodeAsync(viewModel.InsertCommandViewModel, token: token);
            results.UpdateCommandCodes = await this._cqrsCodeService.GenerateCodeAsync(viewModel.UpdateCommandViewModel, token: token);
            results.DeleteCommandCodes = await this._cqrsCodeService.GenerateCodeAsync(viewModel.DeleteCommandViewModel, token: token);
            results.BlazorListCodes = this._blazorCodingService.GenerateCodes(viewModel.BlazorListComponentViewModel);
            results.BlazorDetailsComponentViewModel = this._blazorCodingService.GenerateCodes(viewModel.BlazorDetailsComponentViewModel);
            return results;
        }
    }

    public async Task<Result<FunctionalityViewModel?>> GenerateViewModelAsync(FunctionalityViewModel viewModel, CancellationToken token = default)
    {
        if (!validate(viewModel, token).TryParse(out var validationChecks))
        {
            return validationChecks!;
        }

        this._reporter.Report(description: getTitle("Initializing..."));
        // Initialize the result with the viewModel
        var initResult = initialize(viewModel, token);
        // If the initialization fails, return the result
        if (!initResult.IsSucceed)
        {
            return Result<FunctionalityViewModel>.From(initResult, viewModel)!;
        }
        // Get the data and tokenSource from the initialization result
        var (data, tokenSource) = initResult.GetValue();
        // Initialize the steps for the process
        var process = initSteps(data);

        this._reporter.Report(description: getTitle("Running..."));
        // Run the process with the tokenSource
        var processResult = await process.RunAsync(tokenSource.Token);

        var message = getResultMessage(processResult, tokenSource.Token);
        this._reporter.Report(description: getTitle(message));
        tokenSource.Dispose();
        // Get the result from the processResult
        var result = processResult.Value.Result;

        return result!;

        // Get the title for the description
        ProgressData getTitle(in string description) =>
            new(Description: description, Sender: nameof(FunctionalityService));

        // Validate the model
        static Result<FunctionalityViewModel> validate(in FunctionalityViewModel model, CancellationToken token) =>
            model.Check()
                .RuleFor(_ => !token.IsCancellationRequested, () => new OperationCancelException("Cancelled by parent"))
                .ArgumentNotNull()
                .NotNull(x => x.Name)
                .NotNull(x => x.NameSpace)
                .NotNull(x => x.SourceDto)
                .NotNull(x => x.Name)
                .RuleFor(x => x.SourceDto.Module?.Id != 0, () => new ValidationException("Module is not selected."))
                ;

        // Initialize the viewModel with the connection string
        static Result<(CreationData Data, CancellationTokenSource TokenSource)> initialize(FunctionalityViewModel viewModel, CancellationToken token)
        {
            // If the token is in cancel state, return a failure result
            if (token.IsCancellationRequested)
            {
                return Result<(CreationData, CancellationTokenSource)>.CreateFailure("Cancelled by parent", default);
            }

            // Create a linked tokenSource from the token
            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            // Create a new CreationData with the dataResult, dbTable, and cancellationTokenSource
            var result = new CreationData(viewModel, viewModel.SourceDto, tokenSource);
            // Return a success result with the result and cancellationTokenSource
            return Result<(CreationData, CancellationTokenSource)>.CreateSuccess((result, tokenSource));
        }

        // Initialize the steps for the process
        MultistepProcessRunner<CreationData> initSteps(in CreationData data) =>
            MultistepProcessRunner<CreationData>.New(data, this._reporter, owner: nameof(FunctionalityService))
                .AddStep(this.CreateGetAllQuery, getTitle($"Creating `GetAll{StringHelper.Pluralize(data.ViewModel.Name)}Query`…"))
                .AddStep(this.CreateGetByIdQuery, getTitle($"Creating `GetById{data.ViewModel.Name}Query`…"))

                .AddStep(this.CreateInsertCommand, getTitle($"Creating `Insert{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateUpdateCommand, getTitle($"Creating `Update{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateDeleteCommand, getTitle($"Creating `Delete{data.ViewModel.Name}Command`…"))

                .AddStep(this.CreateBlazorListComponent, getTitle($"Creating Blazor `{data.ViewModel.Name}ListComponent`…"))
                .AddStep(this.CreateBlazorDetailsComponent, getTitle($"Creating Blazor `{data.ViewModel.Name}DetailsComponent`…"))
                .AddStep(this.CreateBlazorPage, getTitle($"Creating {data.ViewModel.Name} Blazor Page…"))

                .AddStep(this.GenerateCodes, getTitle($"Generating {data.ViewModel.Name} Codes…"))
                ;

        // Finalize the process
        static string getResultMessage(in CreationData result, CancellationToken token)
        {
            if (!result.Result.Message.IsNullOrEmpty())
            {
                return result.Result.Message;
            }
            if (token.IsCancellationRequested)
            {
                return "Generating process is cancelled.";
            }
            if (result.Result.IsFailure)
            {
                return "An error occurred while creating functionality view model";
            };
            return "Functionality view model is created.";
        }
    }

    private static DtoViewModel RawDto(CreationData data, bool addTableColumns = false)
    {
        var dto = create(data);
        return addTableColumns ? addColumns(data, dto) : dto;

        static DtoViewModel create(CreationData data) =>
            new DtoViewModel(data.ViewModel.SourceDto.Id, data.ViewModel.Name!)
            {
                Comment = data.COMMENT,
                NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos"),
                Functionality = data.ViewModel
            }
            .With(x => x.Module.Id = data.ViewModel.SourceDto.Module.Id);

        static DtoViewModel addColumns(CreationData data, DtoViewModel dto)
        {
            _ = dto.Properties.ClearAndAddRange(data.ViewModel.SourceDto.Properties.Select(x => new PropertyViewModel(x)));
            return dto;
        }
    }

    private Task CreateBlazorDetailsComponent(CreationData data, CancellationToken token)
    {
        createViewModel(data);
        return Task.CompletedTask;

        void createViewModel(CreationData data) =>
            data.ViewModel.BlazorDetailsViewModel = RawDto(data, true)
                .With(x => x.Name = $"Get{data.ViewModel.Name}DetailsViewModel")
                .With(x => x.IsViewModel = true);
    }

    private Task CreateBlazorListComponent(CreationData data, CancellationToken token)
    {
        createViewModel(data);
        return Task.CompletedTask;

        static void createViewModel(CreationData data)
        {
            data.ViewModel.BlazorListViewModel = RawDto(data, true);
            data.ViewModel.BlazorListViewModel.Name = $"Get{data.ViewModel.Name}ListViewModel";
            data.ViewModel.BlazorListViewModel.IsViewModel = true;
        }
    }

    private Task CreateBlazorPage(CreationData data, CancellationToken token)
    {
        createPageViewModel(data);
        return Task.CompletedTask;

        static void createPageViewModel(CreationData data)
        {
            var pageViewModel = RawDto(data, true);
            pageViewModel.Name = $"Get{data.ViewModel.Name}ViewModel";
            pageViewModel.IsViewModel = true;
            pageViewModel.Properties.Add(new()
            {
                Comment = data.COMMENT,
                Dto = data.ViewModel.BlazorDetailsViewModel,
                Name = data.ViewModel.BlazorDetailsViewModel.Name,
                Type = PropertyType.Dto
            });
            pageViewModel.Properties.Add(new()
            {
                Comment = data.COMMENT,
                Dto = data.ViewModel.BlazorListViewModel,
                Name = data.ViewModel.BlazorListViewModel.Name,
                Type = PropertyType.Dto,
                IsList = true
            });
        }
    }

    private Task CreateDeleteCommand(CreationData data, CancellationToken token)
    {
        return TaskRunner.StartWith(data)
            .Then(createHandler)
            .Then(createParams)
            .Then(createValidator)
            .Then(createResult)
            .RunAsync(token);

        void createParams(CreationData data)
        {
            var dto = RawDto(data, false);
            dto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            dto.Name = $"Delete{data.DtoViewModel.Name}Params";
            dto.IsParamsDto = true;
            dto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
            data.ViewModel.DeleteCommandViewModel.ParamDto = dto;
        }

        Task createValidator(CancellationToken token) =>
            Task.CompletedTask;

        async Task createHandler(CancellationToken token)
        {
            data.ViewModel.DeleteCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.DeleteCommandViewModel.Name = $"Delete{data.DtoViewModel.Name}Command";
            data.ViewModel.DeleteCommandViewModel.Category = CqrsSegregateCategory.Delete;
            data.ViewModel.DeleteCommandViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Commands");
            data.ViewModel.DeleteCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.DeleteCommandViewModel.FriendlyName = data.ViewModel.DeleteCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.DeleteCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.DeleteCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token);
        }

        void createResult(CreationData data)
        {
            var dto = RawDto(data, false);
            dto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            dto.Name = $"Delete{data.DtoViewModel.Name}Result";
            dto.IsResultDto = true;
            data.ViewModel.DeleteCommandViewModel.ResultDto = dto;
        }
    }

    private Task CreateGetAllQuery(CreationData data, CancellationToken token)
    {
        return TaskRunner.StartWith(data)
            .Then(createViewModel)
            .Then(createParams)
            .Then(createResult)
            .RunAsync(token);

        async Task createViewModel(CreationData data, CancellationToken token)
        {
            data.GetAllQueryName = $"GetAll{StringHelper.Pluralize(data.DtoViewModel.Name)}Query";
            data.ViewModel.GetAllQueryViewModel = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetAllQueryViewModel.Name = $"{data.GetAllQueryName}ViewModel";
            data.ViewModel.GetAllQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetAllQueryViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries");
            data.ViewModel.GetAllQueryViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetAllQueryViewModel.FriendlyName = data.GetAllQueryName.SplitCamelCase().Merge(" ");
            data.ViewModel.GetAllQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetAllQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ParamDto = RawDto(data, false);
            data.ViewModel.GetAllQueryViewModel.ParamDto.Name = $"{data.GetAllQueryName}Params";
            data.ViewModel.GetAllQueryViewModel.ParamDto.IsParamsDto = true;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ResultDto = RawDto(data, true);
            data.ViewModel.GetAllQueryViewModel.ResultDto.Name = $"GetAll{data.GetAllQueryName}Result";
            data.ViewModel.GetAllQueryViewModel.ResultDto.IsResultDto = true;
        }
    }

    private Task CreateGetByIdQuery(CreationData data, CancellationToken token)
    {
        return TaskRunner.StartWith(data)
            .Then(createViewModel)
            .Then(createParams)
            .Then(createResult)
            .RunAsync(token);

        async Task createViewModel(CancellationToken token)
        {
            data.GetByIdQueryName = $"GetById{data.DtoViewModel.Name}Query";
            data.ViewModel.GetByIdQueryViewModel = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetByIdQueryViewModel.Name = $"{data.GetByIdQueryName}ViewModel";
            data.ViewModel.GetByIdQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetByIdQueryViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries");
            data.ViewModel.GetByIdQueryViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetByIdQueryViewModel.FriendlyName = data.GetByIdQueryName.SplitCamelCase().Merge(" ");
            data.ViewModel.GetByIdQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetByIdQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParams()
        {
            data.ViewModel.GetByIdQueryViewModel.ParamDto = RawDto(data, false);
            data.ViewModel.GetByIdQueryViewModel.ParamDto.Name = $"GetById{data.DtoViewModel.Name}Params";
            data.ViewModel.GetByIdQueryViewModel.ParamDto.IsParamsDto = true;
            data.ViewModel.GetByIdQueryViewModel.ParamDto.Properties.Add(new() { Comment = data.COMMENT, Name = "Id", Type = PropertyType.Long });
        }

        void createResult()
        {
            data.ViewModel.GetByIdQueryViewModel.ResultDto = RawDto(data, true);
            data.ViewModel.GetByIdQueryViewModel.ResultDto.Name = $"GetById{data.DtoViewModel.Name}Result";
            data.ViewModel.GetByIdQueryViewModel.ResultDto.IsResultDto = true;
        }
    }

    private Task CreateInsertCommand(CreationData data, CancellationToken token)
    {
        return TaskRunner.StartWith(data)
            .Then(createHandler)
            .Then(createParams)
            .Then(createValidator)
            .Then(createResult)
            .RunAsync(token);

        static Task createValidator(CreationData data, CancellationToken token) => Task.CompletedTask;

        async Task createHandler(CreationData data, CancellationToken token)
        {
            data.ViewModel.InsertCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.InsertCommandViewModel.Name = $"Insert{data.DtoViewModel.Name}Command";
            data.ViewModel.InsertCommandViewModel.Category = CqrsSegregateCategory.Create;
            data.ViewModel.InsertCommandViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Commands");
            data.ViewModel.InsertCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.InsertCommandViewModel.FriendlyName = data.ViewModel.InsertCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.InsertCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.InsertCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token);
        }

        void createParams(CreationData data)
        {
            var dto = RawDto(data, true);
            dto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            dto.Name = $"Insert{data.DtoViewModel.Name}Params";
            dto.IsParamsDto = true;
            var idProp = dto.Properties.FirstOrDefault(y => y.Name?.EqualsTo("id") is true);
            if (idProp != null)
            {
                _ = dto.Properties.Remove(idProp);
            }
            data.ViewModel.InsertCommandViewModel.ParamDto = dto;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.InsertCommandViewModel.ResultDto = RawDto(data, false);
            data.ViewModel.InsertCommandViewModel.ResultDto.Name = $"Insert{data.DtoViewModel.Name}Result";
            data.ViewModel.InsertCommandViewModel.ResultDto.IsResultDto = true;
            data.ViewModel.InsertCommandViewModel.ResultDto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
        }
    }

    private Task CreateUpdateCommand(CreationData data, CancellationToken token)
    {
        return TaskRunner.StartWith(data)
            .Then(createHandler)
            .Then(createParamsAsync)
            .Then(createResult)
            .Then(createValidator)
            .RunAsync(token);

        static Task createValidator(CreationData data, CancellationToken token) => Task.CompletedTask;

        async Task createHandler(CreationData data, CancellationToken token)
        {
            data.ViewModel.UpdateCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.UpdateCommandViewModel.Name = $"Update{data.DtoViewModel.Name}Command";
            data.ViewModel.UpdateCommandViewModel.Category = CqrsSegregateCategory.Update;
            data.ViewModel.UpdateCommandViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Commands");
            data.ViewModel.UpdateCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.UpdateCommandViewModel.FriendlyName = data.ViewModel.UpdateCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.UpdateCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.UpdateCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParamsAsync(CreationData data)
        {
            var dto = RawDto(data, true);
            dto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            dto.Name = $"Update{data.DtoViewModel.Name}Params";
            dto.IsParamsDto = true;
            data.ViewModel.UpdateCommandViewModel.ParamDto = dto;
        }

        void createResult(CreationData data)
        {
            var dto = RawDto(data, false);
            dto.Name = $"Update{data.DtoViewModel.Name}Result";
            dto.IsResultDto = true;
            dto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
            data.ViewModel.UpdateCommandViewModel.ResultDto = dto;
        }
    }

    private async Task GenerateCodes(CreationData data, CancellationToken token) =>
        await this.GenerateCodesAsync(data.ViewModel, new(true), token);

    private class CreationData(FunctionalityViewModel result, DtoViewModel dtoViewModel, CancellationTokenSource tokenSource)
    {
        internal readonly string COMMENT = "Auto-generated by Functionality Service.";
        private Result<FunctionalityViewModel>? _result;

        [NotNull]
        internal CancellationTokenSource CancellationTokenSource { get; } = tokenSource;

        internal DtoViewModel DtoViewModel { get; } = dtoViewModel;

        internal string? GetAllQueryName { get; set; }

        internal string? GetByIdQueryName { get; set; }

        [NotNull]
        internal Result<FunctionalityViewModel> Result => this._result ??= new(this.ViewModel);

        internal FunctionalityViewModel ViewModel { get; } = result;
    }
}