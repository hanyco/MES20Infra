using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Actors;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.Helpers;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.BusinessServices;
using Library.CodeGeneration.Models;
using Library.Data.Linq;
using Library.Exceptions.Validations;
using Library.Helpers.CodeGen;
using Library.Interfaces;
using Library.Mapping;
using Library.Results;
using Library.Validations;
using Library.Windows;

using Microsoft.EntityFrameworkCore;

using DtoEntity = HanyCo.Infra.Internals.Data.DataSources.Dto;

namespace Services;

internal sealed class DtoService(
    InfraReadDbContext readDbContext,
    InfraWriteDbContext writeDbContext,
    IEntityViewModelConverter converter,
    ISecurityService securityDescriptor,
    IPropertyService propertyService)
    : IDtoService, IDtoCodeService, IAsyncValidator<DtoViewModel>, IAsyncSaveChanges, IResetChanges
{
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly InfraReadDbContext _db = readDbContext;
    private readonly IPropertyService _propertyService = propertyService;
    private readonly ISecurityService _securityDescriptor = securityDescriptor;
    private readonly InfraWriteDbContext _writeDbContext = writeDbContext;

    public Task<DtoViewModel> CreateAsync(CancellationToken token = default) =>
        Task.FromResult(new DtoViewModel());

    [return: NotNull]
    public DtoViewModel CreateByDbTable(in DbTableViewModel table, in IEnumerable<DbColumnViewModel> columns)
    {
        var result = new DtoViewModel
        {
            DbObject = table,
            Name = $"{table.Name}Dto",
        };
        if (columns?.Any() is true)
        {
            _ = result.Properties!.AddRange(columns.Compact().Select(this._converter.ToPropertyViewModel));
        }

        return result;
    }

    public async Task<Result> DeleteAsync(DtoViewModel model, bool persist, CancellationToken token = default)
    {
        if (!validate(model, token).TryParse(out var validationResult))
        {
            return validationResult;
        }

        try
        {
            _ = await this._propertyService.DeleteByParentIdAsync(model.Id!.Value, false, token);
            _ = this._writeDbContext.RemoveById<DtoEntity>(model.Id!.Value);
            return await this.SubmitChangesAsync(persist: persist, token: token);
        }
        catch (DbUpdateException ex) when (ex.GetBaseException().Message.Contains("FK_CqrsSegregate_Dto"))
        {
            var message = new NotificationMessage("This action can not be completed because this DTO has been referenced a CQRS Segregate.",
                                                  "Can not delete DTO.",
                                                  "DTO In Use",
                                                  "In order to delete this DTO, delete the CQRS Segregate and try again.");
            return Result.CreateFailure(message);
        }
        catch (DbUpdateException ex) when (ex.GetBaseException().Message.Contains("FK_UiComponent_Property"))
        {
            var message = new NotificationMessage("This action can not be completed because this DTO has been referenced a UI Component property.",
                                                  "Can not delete DTO.",
                                                  "DTO In Use",
                                                  "In order to delete this DTO, delete the UI Component property and try again.");
            return Result.CreateFailure(message);
        }

        static Result<DtoViewModel> validate(DtoViewModel? model, CancellationToken token = default)
            => model.Check()
                    .ArgumentNotNull()
                    .NotNull(x => x!.Id)
                    .Build()!;
    }

    public Result<Codes> GenerateCodes(DtoViewModel viewModel, GenerateCodesParameters? arguments = null)
    {
        Check.MustBeArgumentNotNull(viewModel);
        var result = new Codes().With(x => x.props().Category = CodeCategory.Dto);
        if (!validate(viewModel).TryParse(out var validationResult))
        {
            return Result<Codes>.From(validationResult, result);
        }

        var codeGen = convertViewModelToCodeGen(viewModel);
        var code = codeGen.GenerateCode(viewModel.NameSpace).With(x => x.props().Category = CodeCategory.Dto);

        return Result<Codes>.New(result.Add(code));

        static CodeGenDto convertViewModelToCodeGen(DtoViewModel resultViewModel, CancellationToken token = default)
        {
            var result = CodeGenDto.New(TypeMemberNameHelper.GetFullName(resultViewModel.NameSpace, resultViewModel.Name!));
            foreach (var prop in resultViewModel.Properties)
            {
                _ = result.AddProp(CodeGenType.New(prop.TypeFullName), prop.Name!, prop.IsList ?? false, prop.IsNullable ?? false, comment: prop.Comment);
            }
            return result;
        }

        static Result<DtoViewModel> validate(DtoViewModel? viewModel, CancellationToken token = default)
            => viewModel.Check()
                    .NotNull(x => x)
                    .NotNull(x => x.Module)
                    .NotNullOrEmpty(x => x.Name)
                    .NotNullOrEmpty(x => x.NameSpace)
                    .Build()!;
    }

    public async Task<IReadOnlyList<DtoViewModel>> GetAllAsync(CancellationToken token = default)
    {
        var query = from dto in this._db.Dtos
                    select dto;

        var dbResult = await query.ToListLockAsync(this._db.AsyncLock);
        var result = this._converter.FillByDbEntity(dbResult).ToList();
        return result;
    }

    public async Task<IReadOnlySet<DtoViewModel>> GetAllByCategoryAsync(bool? paramsDtos, bool? resultDtos, bool? viewModels, CancellationToken token = default)
    {
        var rawQuery = from dto in this._db.Dtos.Include(x => x.Module)
                       select dto;
        var whereClause = generateWhereClause(paramsDtos, resultDtos, viewModels, token);
        var query = rawQuery.Where(whereClause).Select(dto => dto);

        var dbResult = await query.ToListLockAsync(this._db.AsyncLock);
        var result = this._converter.FillByDbEntity(dbResult).ToReadOnlySet();
        return result;

        static Expression<Func<DtoEntity, bool>> generateWhereClause(bool? paramsDtos, bool? resultDtos, bool? viewModels, CancellationToken token = default)
        {
            if (paramsDtos == null && resultDtos == null && viewModels == null)
            {
                return PredicateBuilder.True<DtoEntity>();
            }

            var whereClause = PredicateBuilder.False<DtoEntity>();
            if (paramsDtos ?? false)
            {
                whereClause = whereClause.Or(dto => dto.IsParamsDto);
            }
            if (resultDtos ?? false)
            {
                whereClause = whereClause.Or(dto => dto.IsResultDto);
            }
            if (viewModels ?? false)
            {
                whereClause = whereClause.Or(dto => dto.IsViewModel);
            }

            return whereClause;
        }
    }

    public async Task<DtoViewModel?> GetByIdAsync(long id, CancellationToken token = default)
    {
        var dbResult = await getDto(id, token);
        if (dbResult is null)
        {
            return null;
        }
        var properties = await getProperties(dbResult, token);
        var result = this._converter.ToViewModel(dbResult)!.ForMember(x => x.Properties.AddRange(properties));

        return result;

        async Task<DtoEntity?> getDto(long id, CancellationToken token = default)
        {
            var query = from x in this._db.Dtos.Include(x => x.Module)
                        where x.Id == id
                        select x;
            var dbResult = await query.FirstOrDefaultLockAsync(this._db.AsyncLock);

            //! MOHAMMAD: 💀 Sample code. Don't remove the following lines 💀
            //var q1 = EF.CompileAsyncQuery((InfraReadDbContext db, long id) => db.Dtos.FirstOrDefault(x => x.Id == id));
            //var a = await q1(this.db, id);

            //var q2 = this.db.CompileAsyncQuery((InfraReadDbContext db, long id) => db.Dtos.FirstOrDefault(x => x.Id == id));
            //var b = await q2(id);

            //var q3 = this.db.CompileAsyncQuery(db => db.Dtos.FirstOrDefault(x => x.Id == id));
            //var c = await q3();

            //string name = "Ali";
            //var q4 = EF.CompileAsyncQuery((InfraReadDbContext db, string name) => db.Dtos.Where(x => x.Name == name));
            //var e = await q4(this.db, name).ToListAsync();

            //var q5 = db.CompileAsyncQuery((InfraReadDbContext db, string name) => db.Dtos.Where(x => x.Name == name));
            //var f = await q5(name).ToListAsync();

            //var q6 = db.CompileAsyncQuery(db => db.Dtos.Where(x => x.Name == name));
            //var g = await q6().ToListAsync();

            return dbResult;
        }

        Task<IReadOnlyList<PropertyViewModel>> getProperties(DtoEntity dbResult, CancellationToken token = default)
            => this._propertyService.GetByParentIdAsync(dbResult.Id, token);

    }

    public async Task<IReadOnlyList<DtoViewModel>> GetByModuleId(long id, CancellationToken token = default)
    {
        var query = from dto in this._db.Dtos
                    where dto.ModuleId == id
                    select dto;
        var dbResult = await query.ToListLockAsync(this._db.AsyncLock);
        var result = this._converter.FillByDbEntity(dbResult).ToList();
        return result;
    }

    public Task<IReadOnlyList<PropertyViewModel>> GetPropertiesByDtoIdAsync(long dtoId, CancellationToken token = default)
        => this._propertyService.GetByParentIdAsync(dtoId, token);

    public async Task<Result<DtoViewModel>> InsertAsync(DtoViewModel viewModel, bool persist = true, CancellationToken token = default)
    {
        var validationCheck = await this.ValidateAsync(viewModel, token);
        if (!validationCheck.IsSucceed)
        {
            return validationCheck!;
        }

        _ = InitializeViewModel(viewModel);
        var entity = this.ToDbEntity(viewModel);

        await using var transaction = await this._writeDbContext.Database.BeginTransactionAsync(token);
        await insertDto(viewModel, entity.Dto, token);
        if (persist)
        {
            await insertProperties(entity.PropertyViewModels, entity.Dto.Id, token);
        }

        var result = await this.SubmitChangesAsync(persist, transaction).With((Task<Result<int>> _) => viewModel.Id = entity.Dto.Id);
        return Result<DtoViewModel>.From(result, viewModel);

        async Task insertDto(DtoViewModel viewModel, DtoEntity dto, CancellationToken token = default)
        {
            _ = this._writeDbContext.ReAttach(dto.Module!).DbContext.Dtos.Add(dto).With(_ => viewModel.Guid = dto.Guid);
            if (persist)
            {
                _ = await this.SaveChangesAsync(token);
            }
            //xawait this._securityDescriptor.SetSecurityDescriptorsAsync(viewModel, false, token);
        }
        async Task insertProperties(IEnumerable<PropertyViewModel> properties, long parentEntityId, CancellationToken token = default) =>
            await this._propertyService.InsertProperties(properties, parentEntityId, persist, token);
    }

    public void ResetChanges()
        => this._writeDbContext.ChangeTracker.Clear();

    public Task<Result<int>> SaveChangesAsync(CancellationToken token = default)
        => this._writeDbContext.SaveChangesResultAsync(cancellationToken: token);

    public async Task<Result<DtoViewModel>> UpdateAsync(long id, DtoViewModel viewModel, bool persist = true, CancellationToken token = default)
    {
        _ = await this.CheckValidatorAsync(viewModel);
        _ = InitializeViewModel(viewModel);
        var entity = this.ToDbEntity(viewModel);
        this.ResetChanges();

        await using var transaction = await this._writeDbContext.BeginTransactionAsync(cancellationToken: token);
        await removeDeletedProperties(viewModel.DeletedProperties, token);
        await updateDto(viewModel, entity.Dto, token);
        await updateProperties(entity.PropertyViewModels, entity.Dto, token);
        var result = await this.SubmitChangesAsync(persist, transaction, token: token).With(_ => viewModel.Id = entity.Dto.Id);
        return Result<DtoViewModel>.From(result, viewModel);

        async Task updateDto(DtoViewModel viewModel, DtoEntity dto, CancellationToken token = default)
        {
            _ = this._writeDbContext.Attach(dto)
                                    .SetModified(x => x.Name)
                                    .SetModified(x => x.NameSpace)
                                    .SetModified(x => x.Comment)
                                    .SetModified(x => x.ModuleId)
                                    .SetModified(x => x.IsParamsDto)
                                    .SetModified(x => x.IsResultDto)
                                    .SetModified(x => x.IsViewModel);

        }
        async Task updateProperties(IEnumerable<PropertyViewModel> properties, DtoEntity dto, CancellationToken token = default)
        {
            foreach (var prop in properties)
            {
                var property = this._converter.ToDbEntity(prop).With(x => x.Id = dto.Id);
                _ = Catch(() => this._writeDbContext.Detach(property));
                _ = Catch(() => this._writeDbContext.Detach(dto.Module!));
                _ = this._writeDbContext.Attach(property)
                                        .SetModified(x => x.Name)
                                        .SetModified(x => x.DbObjectId)
                                        .SetModified(x => x.HasGetter)
                                        .SetModified(x => x.HasSetter)
                                        .SetModified(x => x.IsList)
                                        .SetModified(x => x.IsNullable)
                                        .SetModified(x => x.PropertyType)
                                        .SetModified(x => x.Comment)
                                        .SetModified(x => x.TypeFullName)
                                        .SetModified(x => x.DtoId);
            }
        }
        async Task removeDeletedProperties(IEnumerable<PropertyViewModel>? deletedProperties, CancellationToken token = default)
        {
            if (deletedProperties?.Any() is true)
            {
                foreach (var prop in deletedProperties)
                {
                    if (prop?.Id is { } id)
                    {
                        _ = this._writeDbContext.RemoveById<Property>(id);
                    }
                }
            }
            await Task.CompletedTask;
        }
    }

    public async Task<Result<DtoViewModel?>> ValidateAsync(DtoViewModel? viewModel, CancellationToken token = default)
    {
        var validation = viewModel.Check()
            .ArgumentNotNull()
            .NotNullOrEmpty(x => x!.Name, () => "DTO name cannot be null.")
            .RuleFor(x => x!.Module?.Id is not null and not 0, () => "Module name cannot be null.")
            .Build();
        if (!validation.IsSucceed)
        {
            return validation;
        }

        var query = from dto in this._db.Dtos
                    where dto.Name == viewModel!.Name && dto.Id != viewModel.Id
                    select dto.Id;
        if (await query.AnyAsync(cancellationToken: token))
        {
            return Result<DtoViewModel>.CreateFailure(new ObjectDuplicateValidationException("DTO"));
        }

        var duplicates = viewModel!.Properties.FindDuplicates().Select(x => x?.Name).Compact().ToList();
        if (duplicates.Any())
        {
            return Result<DtoViewModel>.CreateFailure(new ValidationException($"{duplicates.Merge(",")} property name(s) are|is duplicated."));
        };
        return Result<DtoViewModel>.CreateSuccess(viewModel)!;
    }

    private static DtoViewModel InitializeViewModel(in DtoViewModel viewModel)
    {
        if (viewModel.Guid.IsNullOrEmpty())
        {
            viewModel.Guid = Guid.NewGuid();
        }
        foreach (var property in viewModel.Properties)
        {
            if (viewModel.Id is { } parentId && property.ParentEntityId != parentId)
            {
                property.ParentEntityId = parentId;
            }

            if (property.Guid.IsNullOrEmpty())
            {
                property.Guid = Guid.NewGuid();
            }
        }
        return viewModel;
    }

    private (DtoEntity Dto, IEnumerable<Property> Properties, IEnumerable<PropertyViewModel> PropertyViewModels) ToDbEntity(in DtoViewModel viewModel)
    {
        var propsVm = viewModel.Properties.Copy();
        viewModel.Properties.Clear();
        var dto = this._converter.ToDbEntity(viewModel)!;
        var props = propsVm.ConvertAll(x => this._converter.ToDbEntity(x).With(x => x.Id = dto.Id));
        return (dto, props!, propsVm);
    }
}