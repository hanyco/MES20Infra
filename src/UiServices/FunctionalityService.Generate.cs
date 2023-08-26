using System.Diagnostics.CodeAnalysis;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.CodeGeneration.Models;
using Library.Exceptions;
using Library.Exceptions.Validations;
using Library.Results;
using Library.Threading;
using Library.Threading.MultistepProgress;
using Library.Validations;

namespace Services;

internal sealed partial class FunctionalityService
{
    /// <summary>
    /// This method generates codes asynchronously based on the provided FunctionalityViewModel and optional arguments.
    /// It is closely related to the FunctionalityViewModel and its associated codes.
    /// </summary>
    /// <param name="viewModel">The FunctionalityViewModel instance containing components and view models.</param>
    /// <param name="args">Optional arguments for code generation.</param>
    /// <param name="token">Cancellation token to cancel the code generation process.</param>
    /// <returns>A Result containing the generated codes or a failure message if no codes were generated.</returns>
    public async Task<Result<Codes>> GenerateCodesAsync(FunctionalityViewModel viewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs? args = null, CancellationToken token = default)
    {
        Check.MustBeArgumentNotNull(viewModel);

        // Determine whether to update existing codes or generate new ones.
        var codeResult = (args?.UpdateModelView ?? false) ? viewModel.Codes : new();

        // Generate codes asynchronously and combine the results.
        var results = await GenerateCodes(viewModel, codeResult, token);

        // Combine generated results if available, or return a failure message.
        return results.Any()
            ? Result<Codes>.Combine(results, Codes.Combine)
            : Result<Codes>.CreateFailure("No codes generated. ViewModel has no parameter to generate any codes.", Codes.Empty)!;

        // Internal method to generate codes for various components and view models.
        async Task<IEnumerable<Result<Codes>>> GenerateCodes(FunctionalityViewModel viewModel, FunctionalityViewModelCodes results, CancellationToken token)
        {
            var result = new List<Result<Codes>>();

            // Generate codes for GetAllQueryViewModel if available.
            if (viewModel.GetAllQueryViewModel != null)
            {
                results.GetAllQueryCodes = AddToList(await this._cqrsCodeService.GenerateCodeAsync(viewModel.GetAllQueryViewModel, token: token));
            }

            // Generate codes for GetByIdQueryViewModel if available.
            if (viewModel.GetByIdQueryViewModel != null)
            {
                results.GetByIdQueryCodes = AddToList(await this._cqrsCodeService.GenerateCodeAsync(viewModel.GetByIdQueryViewModel, token: token));
            }

            // Generate codes for InsertCommandViewModel if available.
            if (viewModel.InsertCommandViewModel != null)
            {
                results.InsertCommandCodes = AddToList(await this._cqrsCodeService.GenerateCodeAsync(viewModel.InsertCommandViewModel, token: token));
            }

            // Generate codes for UpdateCommandViewModel if available.
            if (viewModel.UpdateCommandViewModel != null)
            {
                results.UpdateCommandCodes = AddToList(await this._cqrsCodeService.GenerateCodeAsync(viewModel.UpdateCommandViewModel, token: token));
            }

            // Generate codes for DeleteCommandViewModel if available.
            if (viewModel.DeleteCommandViewModel != null)
            {
                results.DeleteCommandCodes = AddToList(await this._cqrsCodeService.GenerateCodeAsync(viewModel.DeleteCommandViewModel, token: token));
            }

            // Generate codes for BlazorListComponentViewModel if available.
            if (viewModel.BlazorListComponentViewModel != null)
            {
                results.BlazorListCodes = AddToList(this._blazorCodingService.GenerateCodes(viewModel.BlazorListComponentViewModel));
            }

            // Generate codes for BlazorDetailsComponentViewModel if available.
            if (viewModel.BlazorDetailsComponentViewModel != null)
            {
                results.BlazorDetailsComponentViewModel = AddToList(this._blazorCodingService.GenerateCodes(viewModel.BlazorDetailsComponentViewModel));
            }

            return result;

            // Internal method to add a code result to the result list.
            Result<Codes> AddToList(Result<Codes> codeResult)
            {
                result.Add(codeResult);
                return codeResult;
            }
        }
    }

    public async Task<Result<FunctionalityViewModel?>> GenerateViewModelAsync(FunctionalityViewModel viewModel, CancellationToken token = default)
    {
        if (!validate(viewModel, token).TryParse(out var validationResult))
        {
            return validationResult!;
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
                .RuleFor(x => x.SourceDto.Module?.Id != 0, () => new ValidationException("Module is not selected or has not Id."));

        // Initialize the viewModel with the connection string
        static Result<(CreationData Data, CancellationTokenSource TokenSource)> initialize(FunctionalityViewModel viewModel, CancellationToken token)
        {
            // Create a linked tokenSource from the token
            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            // Create a new CreationData with the dataResult, dbTable, and cancellationTokenSource
            var result = new CreationData(viewModel, (viewModel.Name ?? viewModel.SourceDto.Name).NotNull(), tokenSource);
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

                //! The bellow lines are commented by now to focus on above lines. They will be un-commented later.
                //.AddStep(this.CreateBlazorListComponent, getTitle($"Creating Blazor `{data.ViewModel.Name}ListComponent`…"))
                //.AddStep(this.CreateBlazorDetailsComponent, getTitle($"Creating Blazor `{data.ViewModel.Name}DetailsComponent`…"))
                //.AddStep(this.CreateBlazorPage, getTitle($"Creating {data.ViewModel.Name} Blazor Page…"))

                //! The following line generates codes. The codes must be generated on demanded.
                //x .AddStep(this.GenerateCodes, getTitle($"Generating {data.ViewModel.Name} Codes…"))
                ;

        // Finalize the process
        static string getResultMessage(in CreationData result, CancellationToken token)
        {
            if (!result.Result.Message.IsNullOrEmpty())
            {
                return result.Result.Message;
            }
            if (result.Result.IsFailure)
            {
                return "An error occurred while creating functionality view model";
            }
            if (token.IsCancellationRequested)
            {
                return "Generating process is cancelled.";
            };
            return "Functionality view model is created.";
        }
    }

    private static DtoViewModel RawDto(CreationData data, bool addTableColumns = false)
    {
        // Create an initial DTO based on the input data
        var dto = Create(data);

        // If the addTableColumns parameter is true, add table columns to the DTO
        return addTableColumns ? AddColumns(data, dto) : dto;

        // Internal method for creating an initial DTO
        static DtoViewModel Create(CreationData data) =>
            new DtoViewModel(data.ViewModel.SourceDto.Id, data.ViewModel.Name!)
            {
                Comment = data.COMMENT, // Set DTO comment
                NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos"), // Set DTO namespace
                Functionality = data.ViewModel // Set DTO functionality
            }
            .With(x => x.Module.Id = data.ViewModel.SourceDto.Module.Id); // Set DTO module ID

        // Internal method for adding table columns to DTO
        static DtoViewModel AddColumns(CreationData data, DtoViewModel dto)
        {
            _ = dto.Properties.ClearAndAddRange(data.ViewModel.SourceDto.Properties.Select(x => new PropertyViewModel(x))); // Add columns to DTO
            return dto;
        }
    }

    private Task CreateBlazorDetailsComponent(CreationData data, CancellationToken token)
    {
        createViewModel(data);
        return Task.CompletedTask;

        void createViewModel(CreationData data) => data.ViewModel.BlazorDetailsViewModel = RawDto(data, true)
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
            dto.Name = $"Delete{data.ViewModel.DeleteCommandViewModel.Name}Params";
            dto.IsParamsDto = true;
            dto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
            data.ViewModel.DeleteCommandViewModel.ParamDto = dto;
        }

        Task createValidator(CancellationToken token) =>
            Task.CompletedTask;

        async Task createHandler(CancellationToken token)
        {
            data.ViewModel.DeleteCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.DeleteCommandViewModel.Name = $"Delete{data.SourceDtoName.TrimEnd("Dto")}Command";
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
            dto.Name = $"Delete{data.ViewModel.DeleteCommandViewModel.Name}Result";
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
            data.ViewModel.GetAllQueryViewModel = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetAllQueryViewModel.Name = $"GetAll{StringHelper.Pluralize(data.SourceDtoName.TrimEnd("Dto"))}Query";
            data.ViewModel.GetAllQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetAllQueryViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries");
            data.ViewModel.GetAllQueryViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetAllQueryViewModel.FriendlyName = data.ViewModel.GetAllQueryViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.GetAllQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetAllQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ParamDto = RawDto(data, false);
            data.ViewModel.GetAllQueryViewModel.ParamDto.Name = $"{data.ViewModel.GetAllQueryViewModel.Name}Params";
            data.ViewModel.GetAllQueryViewModel.ParamDto.IsParamsDto = true;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ResultDto = RawDto(data, true);
            data.ViewModel.GetAllQueryViewModel.ResultDto.Name = $"{data.ViewModel.GetAllQueryViewModel.Name}Result";
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
            data.ViewModel.GetByIdQueryViewModel = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetByIdQueryViewModel.Name = $"GetById{data.SourceDtoName.TrimEnd("Dto")}Query";
            data.ViewModel.GetByIdQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetByIdQueryViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries");
            data.ViewModel.GetByIdQueryViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetByIdQueryViewModel.FriendlyName = data.ViewModel.GetByIdQueryViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.GetByIdQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetByIdQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParams()
        {
            data.ViewModel.GetByIdQueryViewModel.ParamDto = RawDto(data, false);
            data.ViewModel.GetByIdQueryViewModel.ParamDto.Name = $"{data.ViewModel.GetByIdQueryViewModel.Name}Params";
            data.ViewModel.GetByIdQueryViewModel.ParamDto.IsParamsDto = true;
            data.ViewModel.GetByIdQueryViewModel.ParamDto.Properties.Add(new() { Comment = data.COMMENT, Name = "Id", Type = PropertyType.Long });
        }

        void createResult()
        {
            data.ViewModel.GetByIdQueryViewModel.ResultDto = RawDto(data, true);
            data.ViewModel.GetByIdQueryViewModel.ResultDto.Name = $"{data.ViewModel.GetByIdQueryViewModel.Name}Result";
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
            data.ViewModel.InsertCommandViewModel.Name = $"Insert{data.SourceDtoName.TrimEnd("Dto")}Command";
            data.ViewModel.InsertCommandViewModel.Category = CqrsSegregateCategory.Create;
            data.ViewModel.InsertCommandViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Commands");
            data.ViewModel.InsertCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.InsertCommandViewModel.FriendlyName = data.ViewModel.InsertCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.InsertCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.InsertCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.InsertCommandViewModel.ParamDto = RawDto(data, true);
            data.ViewModel.InsertCommandViewModel.ParamDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.InsertCommandViewModel.ParamDto.Name = $"{data.ViewModel.InsertCommandViewModel.Name}Params";
            data.ViewModel.InsertCommandViewModel.ParamDto.IsParamsDto = true;
            var idProp = data.ViewModel.InsertCommandViewModel.ParamDto.Properties.FirstOrDefault(y => y.Name?.EqualsTo("id") is true);
            if (idProp != null)
            {
                _ = data.ViewModel.InsertCommandViewModel.ParamDto.Properties.Remove(idProp);
            }
        }

        void createResult(CreationData data)
        {
            data.ViewModel.InsertCommandViewModel.ResultDto = RawDto(data, false);
            data.ViewModel.InsertCommandViewModel.ResultDto.Name = $"{data.ViewModel.InsertCommandViewModel.Name}Result";
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
            data.ViewModel.UpdateCommandViewModel.Name = $"Update{data.SourceDtoName.TrimEnd("Dto")}Command";
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
            dto.Name = $"{data.ViewModel.UpdateCommandViewModel.Name}Params";
            dto.IsParamsDto = true;
            data.ViewModel.UpdateCommandViewModel.ParamDto = dto;
        }

        void createResult(CreationData data)
        {
            var dto = RawDto(data, false);
            dto.Name = $"{data.ViewModel.UpdateCommandViewModel.Name}Result";
            dto.IsResultDto = true;
            dto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
            data.ViewModel.UpdateCommandViewModel.ResultDto = dto;
        }
    }

    private sealed class CreationData(FunctionalityViewModel result, string sourceDtoName, CancellationTokenSource tokenSource)
    {
        internal readonly string COMMENT = "Auto-generated by Functionality Service.";
        private Result<FunctionalityViewModel>? _result;

        [NotNull]
        internal CancellationTokenSource CancellationTokenSource { get; } = tokenSource;

        [NotNull]
        internal Result<FunctionalityViewModel> Result => this._result ??= new(this.ViewModel);

        internal string? SourceDtoName { get; } = sourceDtoName;

        [NotNull]
        internal FunctionalityViewModel ViewModel { get; } = result.ArgumentNotNull();
    }
}