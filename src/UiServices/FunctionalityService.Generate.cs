using System.Diagnostics.CodeAnalysis;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.CodeGeneration.Models;
using Library.Results;
using Library.Threading;
using Library.Threading.MultistepProgress;
using Library.Validations;

namespace Services;

internal sealed partial class FunctionalityService
{
    /// <summary>
    /// Generates codes asynchronously based on the provided FunctionalityViewModel and optional
    /// arguments. It is closely related to the FunctionalityViewModel and its associated codes.
    /// </summary>
    /// <param name="viewModel">
    /// The FunctionalityViewModel instance containing components and view models.
    /// </param>
    /// <param name="args">Optional arguments for code generation.</param>
    /// <param name="token">Cancellation token to cancel the code generation process.</param>
    /// <returns>
    /// A Result containing the generated codes or a failure message if no codes were generated.
    /// </returns>
    public async Task<Result<Codes>> GenerateCodesAsync(FunctionalityViewModel viewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs? args = null, CancellationToken token = default)
    {
        Check.MustBeArgumentNotNull(viewModel);

        // Determine whether to update existing codes or generate new ones.
        var codeResult = (args?.UpdateModelView ?? false) ? viewModel.Codes : new();

        // Generate codes asynchronously and combine the results.
        var results = await generateCodes(viewModel, codeResult, token);

        // Combine generated results if available, or return a failure message.
        return results.Any()
            ? Result<Codes>.Combine(results, Codes.Combine)
            : Result<Codes>.CreateFailure("No codes generated. ViewModel has no parameter to generate any codes.", Codes.Empty)!;

        // Internal method to generate codes for various components and view models.
        async Task<IEnumerable<Result<Codes>>> generateCodes(FunctionalityViewModel viewModel, FunctionalityViewModelCodes codes, CancellationToken token)
        {
            var result = new List<Result<Codes>>();

            // Generate codes for GetAllQueryViewModel if available.
            if (viewModel.GetAllQueryViewModel != null)
            {
                codes.GetAllQueryCodes = await gatherAllCodesAsync(viewModel.GetAllQueryViewModel, token: token);
            }

            // Generate codes for GetByIdQueryViewModel if available.
            if (viewModel.GetByIdQueryViewModel != null)
            {
                codes.GetByIdQueryCodes = await gatherAllCodesAsync(viewModel.GetByIdQueryViewModel, token: token);
            }

            // Generate codes for InsertCommandViewModel if available.
            if (viewModel.InsertCommandViewModel != null)
            {
                codes.InsertCommandCodes = await gatherAllCodesAsync(viewModel.InsertCommandViewModel, token: token);
            }

            // Generate codes for UpdateCommandViewModel if available.
            if (viewModel.UpdateCommandViewModel != null)
            {
                codes.UpdateCommandCodes = await gatherAllCodesAsync(viewModel.UpdateCommandViewModel, token: token);
            }

            // Generate codes for DeleteCommandViewModel if available.
            if (viewModel.DeleteCommandViewModel != null)
            {
                codes.DeleteCommandCodes = await gatherAllCodesAsync(viewModel.DeleteCommandViewModel, token: token);
            }

            // Generate codes for BlazorListComponentViewModel if available.
            if (viewModel.BlazorListComponentViewModel != null)
            {
                codes.BlazorListCodes = addToList(this._blazorComponentCodingService.GenerateCodes(viewModel.BlazorListComponentViewModel));
            }

            // Generate codes for BlazorDetailsComponentViewModel if available.
            if (viewModel.BlazorDetailsComponentViewModel != null)
            {
                codes.BlazorDetailsComponentCodes = addToList(this._blazorComponentCodingService.GenerateCodes(viewModel.BlazorDetailsComponentViewModel));
            }

            // Generate codes for BlazorPageViewModel if available.
            if (viewModel.BlazorPageViewModel != null)
            {
                codes.BlazorDetailsComponentCodes = addToList(this._blazorPageCodingService.GenerateCodes(viewModel.BlazorPageViewModel));
            }

            return result;

            // Internal method to add a code result to the result list.
            Result<Codes> addToList(Result<Codes> codeResult)
            {
                result.Add(codeResult);
                return codeResult;
            }

            async Task<Codes> gatherAllCodesAsync(CqrsViewModelBase cqrsViewModel, CancellationToken token)
            {
                var getAllCodes = new List<Result<Codes>>
                {
                    addToList(this._dtoCodeService.GenerateCodes(cqrsViewModel.ParamsDto)),
                    addToList(this._dtoCodeService.GenerateCodes(cqrsViewModel.ResultDto)),
                    addToList(await this._cqrsCodeService.GenerateCodeAsync(cqrsViewModel, token: token))
                };
                return new Codes(getAllCodes.Select(x => x.Value));
            }
        }
    }

    public async Task<Result<FunctionalityViewModel?>> GenerateViewModelAsync(FunctionalityViewModel viewModel, CancellationToken token = default)
    {
        var validationResult = await this.ValidateAsync(viewModel, token);
        if (!validationResult.IsSucceed)
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

                .AddStep(this.CreateBlazorListComponent, getTitle($"Creating Blazor `{data.ViewModel.Name}ListComponent`…"))
                .AddStep(this.CreateBlazorDetailsComponent, getTitle($"Creating Blazor `{data.ViewModel.Name}DetailsComponent`…"))
                .AddStep(this.CreateBlazorPage, getTitle($"Creating {data.ViewModel.Name} Blazor Page…"));

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
        var dto = new DtoViewModel(data.ViewModel.SourceDto.Id, data.ViewModel.Name!)
        {
            Comment = data.COMMENT, // Set DTO comment
            NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos"), // Set DTO namespace
            Functionality = data.ViewModel // Set DTO functionality
        }
        .With(x => x.Module.Id = data.ViewModel.SourceDto.Module.Id); // Set DTO module ID

        // If the addTableColumns parameter is true, add table columns to the DTO
        return addTableColumns ? AddColumns(data, dto) : dto;

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

        void createViewModel(CreationData data)
        {
            data.ViewModel.BlazorDetailsComponentViewModel = this._blazorComponentCodingService.CreateNewComponentByDto(data.ViewModel.SourceDto);
            data.ViewModel.BlazorDetailsComponentViewModel.Name = $"{data.ViewModel.SourceDto.Name}DetailsComponent";
            data.ViewModel.BlazorDetailsComponentViewModel.ClassName = $"{data.ViewModel.SourceDto.Name}DetailsComponent";
            data.ViewModel.BlazorDetailsComponentViewModel.IsGrid = true;
        }
    }

    private Task CreateBlazorListComponent(CreationData data, CancellationToken token)
    {
        createViewModel(data);
        return Task.CompletedTask;

        void createViewModel(CreationData data)
        {
            data.ViewModel.BlazorListComponentViewModel = this._blazorComponentCodingService.CreateNewComponentByDto(data.ViewModel.SourceDto);
            data.ViewModel.BlazorListComponentViewModel.Name = $"{data.ViewModel.SourceDto.Name}ListComponent";
            data.ViewModel.BlazorListComponentViewModel.ClassName = $"{data.ViewModel.SourceDto.Name}ListComponent";

        }
    }

    private Task CreateBlazorPage(CreationData data, CancellationToken token)
    {
        createPageViewModel(data);
        return Task.CompletedTask;

        void createPageViewModel(CreationData data)
        {
            data.ViewModel.BlazorPageViewModel = this._blazorPageService.CreateViewModel(data.ViewModel.SourceDto);
            data.ViewModel.BlazorPageViewModel.Module = data.ViewModel.SourceDto.Module;
            data.ViewModel.BlazorPageViewModel.Components.Add(data.ViewModel.BlazorDetailsComponentViewModel);
            //data.ViewModel.BlazorDetailsComponentViewModel.PageDataContext
            data.ViewModel.BlazorPageViewModel.Components.Add(data.ViewModel.BlazorListComponentViewModel);
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

        void createParams(CreationData data)
        {
            data.ViewModel.DeleteCommandViewModel.ParamsDto = RawDto(data, false);
            data.ViewModel.DeleteCommandViewModel.ParamsDto.Name = $"Delete{data.ViewModel.DeleteCommandViewModel.Name.TrimEnd("Command")}Params";
            data.ViewModel.DeleteCommandViewModel.ParamsDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.DeleteCommandViewModel.ParamsDto.IsParamsDto = true;
            data.ViewModel.DeleteCommandViewModel.ParamsDto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
        }

        void createResult(CreationData data)
        {
            data.ViewModel.DeleteCommandViewModel.ResultDto = RawDto(data, false);
            data.ViewModel.DeleteCommandViewModel.ResultDto.Name = $"Delete{data.ViewModel.DeleteCommandViewModel.Name.TrimEnd("Command")}Result";
            data.ViewModel.DeleteCommandViewModel.ResultDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.DeleteCommandViewModel.ResultDto.IsResultDto = true;
        }
        Task createValidator(CancellationToken token) =>
            Task.CompletedTask;
    }

    private async Task CreateGetAllQuery(CreationData data, CancellationToken token)
    {
        var result = await TaskRunner.StartWith(data)
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
            data.ViewModel.GetAllQueryViewModel.ParamsDto = RawDto(data, false);
            data.ViewModel.GetAllQueryViewModel.ParamsDto.Name = $"{data.ViewModel.GetAllQueryViewModel.Name.TrimEnd("Query")}Params";
            data.ViewModel.GetAllQueryViewModel.ParamsDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.GetAllQueryViewModel.ParamsDto.IsParamsDto = true;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ResultDto = RawDto(data, true);
            data.ViewModel.GetAllQueryViewModel.ResultDto.Name = $"{data.ViewModel.GetAllQueryViewModel.Name.TrimEnd("Query")}Result";
            data.ViewModel.GetAllQueryViewModel.ResultDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.GetAllQueryViewModel.ResultDto.IsResultDto = true;
            data.ViewModel.GetAllQueryViewModel.ResultDto.IsList = true;
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
            data.ViewModel.GetByIdQueryViewModel.ParamsDto = RawDto(data, false);
            data.ViewModel.GetByIdQueryViewModel.ParamsDto.Name = $"{data.ViewModel.GetByIdQueryViewModel.Name.TrimEnd("Query")}Params";
            data.ViewModel.GetByIdQueryViewModel.ParamsDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.GetByIdQueryViewModel.ParamsDto.IsParamsDto = true;
            data.ViewModel.GetByIdQueryViewModel.ParamsDto.Properties.Add(new() { Comment = data.COMMENT, Name = "Id", Type = PropertyType.Long });
        }

        void createResult()
        {
            data.ViewModel.GetByIdQueryViewModel.ResultDto = RawDto(data, true);
            data.ViewModel.GetByIdQueryViewModel.ResultDto.Name = $"{data.ViewModel.GetByIdQueryViewModel.Name.TrimEnd("Query")}Result";
            data.ViewModel.GetByIdQueryViewModel.ResultDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
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
            data.ViewModel.InsertCommandViewModel.ParamsDto = RawDto(data, true);
            data.ViewModel.InsertCommandViewModel.ParamsDto.Name = $"{data.ViewModel.InsertCommandViewModel.Name.TrimEnd("Command")}Params";
            data.ViewModel.InsertCommandViewModel.ParamsDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.InsertCommandViewModel.ParamsDto.IsParamsDto = true;
            var idProp = data.ViewModel.InsertCommandViewModel.ParamsDto.Properties.FirstOrDefault(y => y.Name?.EqualsTo("id") is true);
            if (idProp != null)
            {
                _ = data.ViewModel.InsertCommandViewModel.ParamsDto.Properties.Remove(idProp);
            }
        }

        void createResult(CreationData data)
        {
            data.ViewModel.InsertCommandViewModel.ResultDto = RawDto(data, false);
            data.ViewModel.InsertCommandViewModel.ResultDto.Name = $"{data.ViewModel.InsertCommandViewModel.Name.TrimEnd("Command")}Result";
            data.ViewModel.InsertCommandViewModel.ResultDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
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

        static Task createValidator(CreationData data, CancellationToken token) =>
            Task.CompletedTask;

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
            data.ViewModel.UpdateCommandViewModel.ParamsDto = RawDto(data, true);
            data.ViewModel.UpdateCommandViewModel.ParamsDto.Name = $"{data.ViewModel.UpdateCommandViewModel.Name.TrimEnd("Command")}Params";
            data.ViewModel.UpdateCommandViewModel.ParamsDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.UpdateCommandViewModel.ParamsDto.IsParamsDto = true;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.UpdateCommandViewModel.ResultDto = RawDto(data, false);
            data.ViewModel.UpdateCommandViewModel.ResultDto.Name = $"{data.ViewModel.UpdateCommandViewModel.Name.TrimEnd("Command")}Result";
            data.ViewModel.UpdateCommandViewModel.ResultDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.UpdateCommandViewModel.ResultDto.IsResultDto = true;
            data.ViewModel.UpdateCommandViewModel.ResultDto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
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