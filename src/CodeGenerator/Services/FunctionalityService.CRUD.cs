using System.Collections.ObjectModel;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.BusinessServices;
using Library.Exceptions;
using Library.Exceptions.Validations;
using Library.Results;
using Library.Threading;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal partial class FunctionalityService
{
    public Task<bool> AnyByNameAsync(string name)
    {
        var query = from dto in this._readDbContext.Functionalities
                    where dto.Name == name
                    select dto.Id;
        return query.AnyAsync();
    }

    public async Task<Result> DeleteAsync(FunctionalityViewModel model, bool persist = true, CancellationToken token = default)
    {
        CheckPersistence(persist);
        if (!validate(model).TryParse(out var vr))
        {
            return vr;
        }

        var functionality = await getFunctionality(model.Id!.Value, token);
        if (functionality == null)
        {
            return Result.CreateFailure<ObjectNotFoundException>();
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
        return result;

        static Result<FunctionalityViewModel> validate(FunctionalityViewModel model)
            => model.Check().ArgumentNotNull().NotNull(x => x.Id);
        Task<Functionality?> getFunctionality(long modelId, CancellationToken token)
            => this.GetByIdQueryAsync(modelId).FirstOrDefaultLockAsync(this._readDbContext.AsyncLock, token);
        Task<Result> removeQuery(CqrsSegregate query, CancellationToken token)
            => removeSegregate(query, this._queryService.DeleteByIdAsync, token);
        Task<Result> removeCommand(CqrsSegregate command, CancellationToken token)
            => removeSegregate(command, this._commandService.DeleteByIdAsync, token);
        Task<Result> removeDto(Dto dto, CancellationToken token)
            => dto == null ? Task.FromResult(Result.Success) : this._dtoService.DeleteByIdAsync(dto.Id, true, token);

        async Task<Result> removeFunctionality(Functionality functionality, CancellationToken token)
        {
            _ = this._writeDbContext.RemoveById<Functionality>(functionality.Id);
            var i = await this.SaveChangesAsync(token);
            return i > 0 ? Result.Success : Result.Failure;
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

    public Task<IReadOnlyList<FunctionalityViewModel>> GetAllAsync(CancellationToken cancellationToken = default) =>
        ServiceHelper.GetAllAsync<FunctionalityViewModel, Functionality>(this, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public async Task<FunctionalityViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var dbQuery = this.GetByIdQueryAsync(id);
        var dbResult = await dbQuery.FirstOrDefaultLockAsync(this._readDbContext.AsyncLock, cancellationToken);
        var result = this._converter.ToViewModel(dbResult);
        return result;
    }

    private IQueryable<Functionality> GetByIdQuery(long id)
    {
        var result = this._readDbContext.Functionalities
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
            .Where(f => f.Id == id)
            .Select(f => new Functionality
            {
                Id = f.Id,
                Name = f.Name,
                ModuleId = f.ModuleId,
                Guid = f.Guid,
                Comment = f.Comment,
                DeleteCommand = new CqrsSegregate
                {
                    Id = f.DeleteCommand.Id,
                    ModuleId = f.DeleteCommand.ModuleId,
                    ParamDto = new Dto
                    {
                        Id = f.DeleteCommand.ParamDto.Id,
                        Name = f.DeleteCommand.ParamDto.Name,
                        Properties = f.DeleteCommand.ParamDto.Properties
                    },
                    ResultDto = new Dto
                    {
                        Id = f.DeleteCommand.ResultDto.Id,
                        Name = f.DeleteCommand.ResultDto.Name,
                        Properties = f.DeleteCommand.ResultDto.Properties
                    }
                },
                GetAllQuery = new CqrsSegregate
                {
                    Id = f.GetAllQuery.Id,
                    ModuleId = f.GetAllQuery.ModuleId,
                    ParamDto = new Dto
                    {
                        Id = f.GetAllQuery.ParamDto.Id,
                        Name = f.GetAllQuery.ParamDto.Name,
                        Properties = f.GetAllQuery.ParamDto.Properties
                    },
                    ResultDto = new Dto
                    {
                        Id = f.GetAllQuery.ResultDto.Id,
                        Name = f.GetAllQuery.ResultDto.Name,
                        Properties = f.GetAllQuery.ResultDto.Properties
                    }
                },
                GetByIdQuery = new CqrsSegregate
                {
                    Id = f.GetByIdQuery.Id,
                    ModuleId = f.GetByIdQuery.ModuleId,
                    ParamDto = new Dto
                    {
                        Id = f.GetByIdQuery.ParamDto.Id,
                        Name = f.GetByIdQuery.ParamDto.Name,
                        Properties = f.GetByIdQuery.ParamDto.Properties
                    },
                    ResultDto = new Dto
                    {
                        Id = f.GetByIdQuery.ResultDto.Id,
                        Name = f.GetByIdQuery.ResultDto.Name,
                        Properties = f.GetByIdQuery.ResultDto.Properties
                    }
                },
                InsertCommand = new CqrsSegregate
                {
                    Id = f.InsertCommand.Id,
                    ModuleId = f.InsertCommand.ModuleId,
                    ParamDto = new Dto
                    {
                        Id = f.InsertCommand.ParamDto.Id,
                        Name = f.InsertCommand.ParamDto.Name,
                        Properties = f.InsertCommand.ParamDto.Properties
                    },
                    ResultDto = new Dto
                    {
                        Id = f.InsertCommand.ResultDto.Id,
                        Name = f.InsertCommand.ResultDto.Name,
                        Properties = f.InsertCommand.ResultDto.Properties
                    }
                },
                UpdateCommand = new CqrsSegregate
                {
                    Id = f.UpdateCommand.Id,
                    ModuleId = f.UpdateCommand.ModuleId,
                    ParamDto = new Dto
                    {
                        Id = f.UpdateCommand.ParamDto.Id,
                        Name = f.UpdateCommand.ParamDto.Name,
                        Properties = f.UpdateCommand.ParamDto.Properties
                    },
                    ResultDto = new Dto
                    {
                        Id = f.UpdateCommand.ResultDto.Id,
                        Name = f.UpdateCommand.ResultDto.Name,
                        Properties = f.UpdateCommand.ResultDto.Properties
                    }
                },
                Module = new Module
                {
                    Id = f.Module.Id,
                    Name = f.Module.Name,
                    //   اضافه   کردن   سایر   فیلدهای   مورد   نیاز
                },
                SourceDto = new Dto
                {
                    Id = f.SourceDto.Id,
                    Name = f.SourceDto.Name,
                    Properties = f.SourceDto.Properties
                }
            });
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
                (entity.GetAllQuery.Id, entity.GetByIdQuery.Id, entity.InsertCommand.Id, entity.UpdateCommand.Id, entity.DeleteCommand.Id, entity.Module.Id, entity.SourceDto.Id);
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
                return Result.CreateFailure("Functionality already exists.")!;
            }
            exists = await this._dtoService.AnyByNameAsync(model.SourceDto.Name!);
            if (exists)
            {
                return Result.CreateFailure("Functionality already exists.")!;
            }
            exists = await this._queryService.AnyByNameAsync(model.GetAllQueryViewModel.Name!);
            if (exists)
            {
                return Result.CreateFailure("GetAllQuery already exists.")!;
            }
            exists = await this._queryService.AnyByNameAsync(model.GetByIdQueryViewModel.Name!);
            if (exists)
            {
                return Result.CreateFailure("GetByIdQuery already exists.")!;
            }
            exists = await this._commandService.AnyByNameAsync(model.InsertCommandViewModel.Name!);
            if (exists)
            {
                return Result.CreateFailure("InsertCommand already exists.")!;
            }
            exists = await this._commandService.AnyByNameAsync(model.UpdateCommandViewModel.Name!);
            if (exists)
            {
                return Result.CreateFailure("UpdateCommand already exists.")!;
            }
            exists = await this._commandService.AnyByNameAsync(model.DeleteCommandViewModel.Name!);
            if (exists)
            {
                return Result.CreateFailure("DeleteCommand already exists.")!;
            };
            return Result.Success;
        }
    }

    public async Task<Result<FunctionalityViewModel>> UpdateAsync(long id, FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        if (!this.Validate(model).TryParse(out var validationCheck))
        {
            return validationCheck!;
        }
        if (model.SourceDto is null or { Id: null })
        {
            return Result<FunctionalityViewModel>.CreateFailure(model, () => new NullValueValidationException(nameof(model.SourceDto)))!;
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
        var result = await ServiceHelper.UpdateAsync(this, this._readDbContext, model, this._converter.ToDbEntity, false, logger: this.Logger, cancellationToken: cancellationToken).ModelResult();
        if (persist)
        {
            _ = await this.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    private static void CheckPersistence(bool persist)
    {
        if (!persist)
        {
            throw new NotSupportedException("non-persistent operation is not supported.");
        }
    }
}