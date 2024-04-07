﻿using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Data.EntityFrameworkCore;
using Library.Exceptions;
using Library.Results;
using Library.Threading;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal partial class FunctionalityService
{
    public async Task<Result<int>> DeleteAsync(FunctionalityViewModel model, bool persist = true, CancellationToken token = default)
    {
        CheckPersistence(persist);
        if (!validate(model).TryParse(out var vr))
        {
            return vr.WithValue<int>(-1);
        }

        var functionality = await getFunctionality(model.Id!.Value, token);
        if (functionality == null)
        {
            return Result.Fail<int, ObjectNotFoundException>();
        }

        var tasks = new Collection<Func<Functionality, Task<Result>>>
        {
            x => removeFunctionality(x, token),
            x => removeDto(x.SourceDto, token),
            x => removeQuery(x.GetAllQuery, token),
            x => removeQuery(x.GetByIdQuery, token),
            x => removeCommand(x.InsertCommand, token),
            x => removeCommand(x.UpdateCommand, token),
            x => removeCommand(x.DeleteCommand, token)
        };

        var result = await tasks.RunAllAsync(functionality, token);
        return result.WithValue<int>(1);

        static Result<FunctionalityViewModel> validate(FunctionalityViewModel model)
            => model.Check().ArgumentNotNull().NotNull(x => x.Id);
        Task<Functionality?> getFunctionality(long modelId, CancellationToken token)
            => this.GetByIdFuncitonality(modelId, token);
        Task<Result> removeQuery(CqrsSegregate query, CancellationToken token)
            => removeSegregate(query, this._queryService.DeleteByIdAsync, token);
        Task<Result> removeCommand(CqrsSegregate command, CancellationToken token)
            => removeSegregate(command, this._commandService.DeleteByIdAsync, token);
        Task<Result> removeDto(Dto dto, CancellationToken token)
            => dto == null ? Task.FromResult(Result.Succeed) : this._dtoService.DeleteByIdAsync(dto.Id, true, token);

        async Task<Result> removeFunctionality(Functionality functionality, CancellationToken token)
        {
            _ = this._writeDbContext.RemoveById<Functionality>(functionality.Id);
            var i = await this.SaveChangesAsync(token);
            return i > 0 ? Result.Succeed : Result.Failed;
        }
        async Task<Result> removeSegregate(CqrsSegregate segregate, Func<long, bool, CancellationToken, Task<Result>> deleteByIdAsync, CancellationToken token)
        {
            var result = await deleteByIdAsync(segregate.Id, true, token);
            if (result.IsFailure)
            {
                return result;
            }
            result = await removeDto(segregate.ParamDto, token);
            if (result.IsFailure)
            {
                return result;
            }

            result = await removeDto(segregate.ResultDto, token);
            return result;
        }
    }

    public Task<IReadOnlyList<FunctionalityViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => this.GetAllAsync<FunctionalityViewModel, Functionality>(this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public async Task<FunctionalityViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var dbResult = await this.GetByIdFuncitonality(id, cancellationToken);
        var result = this._converter.ToViewModel(dbResult);
        return result;
    }

    public async Task<Result<FunctionalityViewModel>> InsertAsync(FunctionalityViewModel model, bool persist = true, CancellationToken token = default)
    {
        CheckPersistence(persist);
        if (!validateModel(model).TryParse(out var vr))
        {
            return vr;
        }

        var er = await checkExitance(model);
        if (er.IsFailure)
        {
            return er.WithValue(model);
        }
        var result = await TaskRunner.StartWith(model)
            .Then(x => saveQuery(x.GetAllQueryViewModel, token))
            .Then(x => saveQuery(x.GetByIdQueryViewModel, token))
            .Then(x => saveCommand(x.InsertCommandViewModel, token))
            .Then(x => saveCommand(x.UpdateCommandViewModel, token))
            .Then(x => saveCommand(x.DeleteCommandViewModel, token))
            .Then(x => saveDto(x.SourceDto, token))
            .Then(x => saveFunctionality(x, token))
            .RunAsync(token)
            .IfFailure(this._dtoService.ResetChanges)
            .IfSucceedAsync(async (x, token) =>
            {
                var saveResult = await this.SubmitChangesAsync(true, token: token);
                if (!saveResult.IsSucceed)
                {
                    this._dtoService.ResetChanges();
                }
                return saveResult.WithValue(x);
            }, token);

        return result.ToNotNullValue();

        static Result<FunctionalityViewModel> validateModel(FunctionalityViewModel model) =>
            BasicChecks(model)
            .NotNull(x => x!.SourceDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.GetAllQueryViewModel, () => "ViewModel is not initiated.")
            .NotNull(x => x!.GetAllQueryViewModel.ParamsDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.GetAllQueryViewModel.ResultDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.GetByIdQueryViewModel, () => "ViewModel is not initiated.")
            .NotNull(x => x!.GetByIdQueryViewModel.ParamsDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.GetByIdQueryViewModel.ResultDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.InsertCommandViewModel, () => "ViewModel is not initiated.")
            .NotNull(x => x!.InsertCommandViewModel.ParamsDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.InsertCommandViewModel.ResultDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.UpdateCommandViewModel, () => "ViewModel is not initiated.")
            .NotNull(x => x!.UpdateCommandViewModel.ParamsDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.UpdateCommandViewModel.ResultDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.DeleteCommandViewModel, () => "ViewModel is not initiated.")
            .NotNull(x => x!.DeleteCommandViewModel.ParamsDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.DeleteCommandViewModel.ResultDto, () => "ViewModel is not initiated.");
        async Task<Result> saveQuery(CqrsQueryViewModel model, CancellationToken token)
        {
            //model.ParamsDto.Functionality = model.ResultDto.Functionality = functionality;
            Result result = await this._dtoService.InsertAsync(model.ParamsDto, true, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }

            result = await this._dtoService.InsertAsync(model.ResultDto, true, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }

            result = await this._queryService.InsertAsync(model, true, token);
            if (!result.IsSucceed)
            {
                this._queryService.ResetChanges();
            }
            return result;
        }
        async Task<Result> saveCommand(CqrsCommandViewModel model, CancellationToken token)
        {
            //model.ParamsDto.Functionality = model.ResultDto.Functionality = functionality;
            Result result = await this._dtoService.InsertAsync(model.ParamsDto, true, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._dtoService.InsertAsync(model.ResultDto, true, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._commandService.InsertAsync(model, true, token);
            if (!result.IsSucceed)
            {
                this._queryService.ResetChanges();
            }

            return result;
        }
        Task<Result> saveDto(DtoViewModel model, CancellationToken token)
            => this._dtoService.InsertAsync(model, true, token).ToResultAsync();
        async Task<Result> saveFunctionality(FunctionalityViewModel model, CancellationToken token)
        {
            var entity = this._converter.ToDbEntity(model);
            var (getAllId, getById, insertId, updateId, deleteId, moduleId, sourceDtoId) =
                (entity.GetAllQuery.Id, entity.GetByIdQuery.Id, entity.InsertCommand.Id, entity.UpdateCommand.Id, entity.DeleteCommand.Id, entity.Module!.Id, entity.SourceDto.Id);
            entity.GetAllQuery
                = entity.GetByIdQuery
                = entity.InsertCommand
                = entity.UpdateCommand
                = entity.DeleteCommand
                = null!;
            entity.Module = null!;
            entity.SourceDto = null!;
            (entity.GetAllQueryId, entity.GetByIdQueryId, entity.InsertCommandId, entity.UpdateCommandId, entity.DeleteCommandId, entity.ModuleId, entity.SourceDtoId) =
                (getAllId, getById, insertId, updateId, deleteId, moduleId, sourceDtoId);

            _ = await this._writeDbContext.Functionalities.AddAsync(entity, token);
            entity.Module = null!;
            return await this.SaveChangesAsync(token);
        }

        async Task<Result> checkExitance(FunctionalityViewModel model)
        {
            var exists = await this.AnyByNameAsync(model.Name!);
            if (exists)
            {
                return Result.Fail()!;
            }
            exists = await this._dtoService.AnyByNameAsync(model.SourceDto.Name!);
            if (exists)
            {
                return Result.Fail()!;
            }
            exists = await this._queryService.AnyByNameAsync(model.GetAllQueryViewModel.Name!);
            if (exists)
            {
                return Result.Fail()!;
            }
            exists = await this._queryService.AnyByNameAsync(model.GetByIdQueryViewModel.Name!);
            if (exists)
            {
                return Result.Fail()!;
            }
            exists = await this._commandService.AnyByNameAsync(model.InsertCommandViewModel.Name!);
            if (exists)
            {
                return Result.Fail()!;
            }
            exists = await this._commandService.AnyByNameAsync(model.UpdateCommandViewModel.Name!);
            if (exists)
            {
                return Result.Fail()!;
            }
            exists = await this._commandService.AnyByNameAsync(model.DeleteCommandViewModel.Name!);
            if (exists)
            {
                return Result.Fail()!;
            };
            return Result.Succeed;
        }
    }

    //    return result.AsNoTracking().AsSplitQuery();
    //}
    public async Task<Result<FunctionalityViewModel>> UpdateAsync(long id, FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        CheckPersistence(persist);
        if (!this.Validate(model).TryParse(out var validationCheck))
        {
            return validationCheck!;
        }
        if (model.SourceDto is null or { Id: null })
        {
            return Result.Fail(model)!;
        }

        Result actionResult;
        actionResult = await this._dtoService.UpdateAsync(model.SourceDto.Id.Value, model.SourceDto, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._queryService.UpdateAsync(model.GetAllQueryViewModel.Id!.Value, model.GetAllQueryViewModel, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._queryService.UpdateAsync(model.GetByIdQueryViewModel.Id!.Value, model.GetByIdQueryViewModel, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._commandService.UpdateAsync(model.InsertCommandViewModel.Id!.Value, model.InsertCommandViewModel, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._commandService.UpdateAsync(model.UpdateCommandViewModel.Id!.Value, model.UpdateCommandViewModel, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._commandService.UpdateAsync(model.DeleteCommandViewModel.Id!.Value, model.DeleteCommandViewModel, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        var result = await DataServiceHelper.UpdateAsync(this, this._readDbContext, model, this._converter.ToDbEntity, false, logger: this.Logger, cancellationToken: cancellationToken).ModelResult();
        if (persist)
        {
            _ = await this.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    private static void CheckPersistence([DoesNotReturnIf(false)] bool persist)
    {
        if (!persist)
        {
            throw new NotSupportedException("non-persistent operation is not supported.");
        }
    }

    private Task<bool> AnyByNameAsync(string name)
    {
        var query = from dto in this._readDbContext.Functionalities
                    where dto.Name == name
                    select dto.Id;
        return query.AnyAsync();
    }

    private async Task<Functionality?> GetByIdFuncitonality(long id, CancellationToken cancellationToken)
    {
        var funcQuery = from func in this._readDbContext.Functionalities
                            .Include(f => f.DeleteCommand)
                                .ThenInclude(cs => cs.Module)
                            .Include(f => f.DeleteCommand)
                                .ThenInclude(cs => cs.ParamDto)
                            .Include(f => f.DeleteCommand)
                                .ThenInclude(cs => cs.ResultDto)
                            .Include(f => f.GetAllQuery)
                                .ThenInclude(cs => cs.Module)
                            .Include(f => f.GetAllQuery)
                                .ThenInclude(cs => cs.ParamDto)
                            .Include(f => f.GetAllQuery)
                                .ThenInclude(cs => cs.ResultDto)
                            .Include(f => f.GetByIdQuery)
                                .ThenInclude(cs => cs.Module)
                            .Include(f => f.GetByIdQuery)
                                .ThenInclude(cs => cs.ParamDto)
                            .Include(f => f.GetByIdQuery)
                                .ThenInclude(cs => cs.ResultDto)
                            .Include(f => f.InsertCommand)
                                .ThenInclude(cs => cs.Module)
                            .Include(f => f.InsertCommand)
                                .ThenInclude(cs => cs.ParamDto)
                            .Include(f => f.InsertCommand)
                                .ThenInclude(cs => cs.ResultDto)
                            .Include(f => f.UpdateCommand)
                                .ThenInclude(cs => cs.Module)
                            .Include(f => f.UpdateCommand)
                                .ThenInclude(cs => cs.ParamDto)
                            .Include(f => f.UpdateCommand)
                                .ThenInclude(cs => cs.ResultDto)
                            .Include(f => f.Module)
                            .Include(f => f.SourceDto)
                                .ThenInclude(f => f.Module)
                        where func.Id == id
                        select func;
        var functionality = await funcQuery.FirstOrDefaultAsync(cancellationToken);
        if (functionality == null)
        {
            return null;
        }

        await fillProps(functionality.GetAllQuery);
        await fillProps(functionality.GetByIdQuery);
        await fillProps(functionality.InsertCommand);
        await fillProps(functionality.UpdateCommand);
        await fillProps(functionality.DeleteCommand);
        await fillProperties(functionality.SourceDto.Properties, functionality.SourceDtoId);

        return functionality;

        async Task fillProps(CqrsSegregate cqrs)
        {
            await fillProperties(cqrs.ParamDto.Properties, cqrs.ParamDtoId);
            await fillProperties(cqrs.ResultDto.Properties, cqrs.ResultDtoId);
        }

        async Task fillProperties(ICollection<Property> properties, long paramDtoId)
            => properties.AddRange(await getProperties(paramDtoId));

        async Task<List<Property>> getProperties(long parentEntityId)
        {
            var propertiesQuery = from x in this._readDbContext.Properties
                                  where x.ParentEntityId == parentEntityId
                                  select x;
            var properties = await propertiesQuery.ToListAsync(cancellationToken: cancellationToken);
            return properties;
        }
    }
}