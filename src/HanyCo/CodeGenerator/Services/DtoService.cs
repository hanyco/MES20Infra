using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.Helpers;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Data.EntityFrameworkCore;
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

namespace Services.CodeGen;

internal sealed class DtoService(
    InfraReadDbContext readDbContext,
    InfraWriteDbContext writeDbContext,
    IEntityViewModelConverter converter,
    ISecurityService securityDescriptor,
    IPropertyService propertyService,
    ICodeGeneratorEngine codeGeneratorEngine)
    : IDtoService, IDtoCodeService, IAsyncValidator<DtoViewModel>, IAsyncSaveChanges, IResetChanges
{
    private readonly ICodeGeneratorEngine _codeGeneratorEngine = codeGeneratorEngine;
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly IPropertyService _propertyService = propertyService;
    private readonly InfraReadDbContext _readDbContext = readDbContext;
    private readonly InfraWriteDbContext _writeDbContext = writeDbContext;

    public Task<bool> AnyByNameAsync(string name)
    {
        var query = from dto in this._readDbContext.Dtos
                    where dto.Name == name
                    select dto.Id;
        return query.AnyAsync();
    }

    public Task<DtoViewModel> CreateAsync(CancellationToken token = default)
        => Task.FromResult(new DtoViewModel());

    [return: NotNull]
    public DtoViewModel CreateByDbTable(in DbTableViewModel table, in IEnumerable<DbColumnViewModel> columns)
    {
        var result = new DtoViewModel
        {
            DbObject = table,
            Name = $"{StringHelper.Singularize(table.Name)}Dto",
        };
        if (columns?.Any() is true)
        {
            _ = result.Properties!.AddRange(columns.Compact().Select(this._converter.ToPropertyViewModel));
        }

        return result;
    }

    public async Task<Result<int>> DeleteAsync(DtoViewModel model, bool persist, CancellationToken token = default)
    {
        if (!validate(model, token).TryParse(out var validationResult))
        {
            return validationResult.WithValue<int>(-1);
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
            return Result.Fail<int>();
        }
        catch (DbUpdateException ex) when (ex.GetBaseException().Message.Contains("FK_UiComponent_Property"))
        {
            var message = new NotificationMessage("This action can not be completed because this DTO has been referenced a UI Component property.",
                                                  "Can not delete DTO.",
                                                  "DTO In Use",
                                                  "In order to delete this DTO, delete the UI Component property and try again.");
            return Result.Fail<int>();
        }

        static Result<DtoViewModel> validate(DtoViewModel? model, CancellationToken token = default)
            => model.Check()
                    .ArgumentNotNull()
                    .NotNull(x => x!.Id)
                    .Build()!;
    }

    public async Task<Result> DeleteById(long dtoId, bool persist = true, CancellationToken token = default)
    {
        var dto = await this.GetByIdAsync(dtoId, token);
        if (!Check.IfIsNull(dto).TryParse(out var ncr))
        {
            return ncr;
        };

        return await this.DeleteAsync(dto!, persist, token);
    }

    public Result<Codes?> GenerateCodes(DtoViewModel viewModel, DtoCodeServiceAsyncCodeGeneratorArgs? arguments = null)
    {
        if (!validate(viewModel).TryParse(out var validationResult))
        {
            return validationResult.WithValue(Codes.Empty);
        }

        var properties = viewModel.Properties.Distinct().Select(toProperty);
        var type = new Class(arguments?.TypeName ?? viewModel.Name!)
        {
            InheritanceModifier = InheritanceModifier.Sealed,
            AccessModifier = AccessModifier.Public
        }.AddMember(properties);
        if (viewModel.BaseType is { } baseType)
        {
            _ = type.AddBaseType(baseType);
        }
        addSecurityClaims(viewModel, type);

        var nameSpace = INamespace.New(viewModel.NameSpace).AddType(type);

        var statement = this._codeGeneratorEngine.Generate(nameSpace);
        var code = new Code(type.Name, Languages.CSharp, statement.Value).With(x => x.SetCategory(CodeCategory.Dto));

        var result = Result.From<Codes?>(statement, code.ToCodes());
        return result;

        static CodeGenProperty toProperty(PropertyViewModel pvm)
        {
            var typeFullName = pvm.TypeFullName.NotNull();

            var type = (pvm.IsList ?? false
                ? TypePath.New(typeof(List<>).FullName!, [typeFullName])
                : TypePath.New(typeFullName))
                .WithNullable(pvm.IsNullable ?? false);
            var result = new CodeGenProperty(pvm.Name!, type);
            //if (pvm.Name != "Id" && !(pvm.IsNullable ?? false))
            //{
            //    result.AddAttribute<RequiredAttribute>();
            //}
            foreach (var claim in pvm.SecurityClaims)
            {
                result.AddAttribute<SecurityAttribute>([("Key", claim.Key.NotNull()), ("Value", claim.Value?.ToString() ?? "null")]);
            }
            return result;
        }

        static Result<DtoViewModel> validate(in DtoViewModel? viewModel, in CancellationToken token = default)
            => viewModel.ArgumentNotNull().Check()
                .NotNull(x => x.Module)
                .NotNullOrEmpty(x => x.Name)
                .NotNullOrEmpty(x => x.NameSpace)
                .Build();

        static void addSecurityClaims(DtoViewModel viewModel, Class type)
        {
            foreach (var claim in viewModel.SecurityClaims)
            {
                _ = type.AddAttribute<SecurityAttribute>(("Key", claim.Key.NotNull()), ("Value", claim.Value?.ToString() ?? "null"));
            }
        }
    }

    public async Task<IReadOnlyList<DtoViewModel>> GetAllAsync(CancellationToken token = default)
    {
        var query = from dto in this._readDbContext.Dtos
                    select dto;

        var dbResult = await query.ToListLockAsync(this._readDbContext.AsyncLock);
        var result = this._converter.FillByDbEntity(dbResult).ToList();
        return result;
    }

    public async Task<IReadOnlySet<DtoViewModel>> GetAllByCategoryAsync(bool? paramsDtos, bool? resultDtos, bool? viewModels, CancellationToken token = default)
    {
        var rawQuery = from dto in this._readDbContext.Dtos.Include(x => x.Module)
                       select dto;
        var whereClause = generateWhereClause(paramsDtos, resultDtos, viewModels, token);
        var query = rawQuery.Where(whereClause).Select(dto => dto);

        var dbResult = await query.ToListLockAsync(this._readDbContext.AsyncLock);
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
            var query = from x in this._readDbContext.Dtos.Include(x => x.Module)
                        where x.Id == id
                        select x;
            var dbResult = await query.FirstOrDefaultLockAsync(this._readDbContext.AsyncLock, cancellationToken: token);

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
        var query = from dto in this._readDbContext.Dtos
                    where dto.ModuleId == id
                    select dto;
        var dbResult = await query.ToListLockAsync(this._readDbContext.AsyncLock);
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

        _ = this.InitializeViewModel(viewModel);
        var entity = this.ToDbEntity(viewModel);

        await using var transaction = await this._writeDbContext.Database.BeginTransactionAsync(token);
        await insertDto(viewModel, entity.Dto, persist, token);
        await insertProperties(viewModel, persist, entity, token);
        var result = await this.SubmitChangesAsync(persist, transaction, token: token).With((_) => viewModel.Id = entity.Dto.Id);
        return Result.From(result, viewModel);

        async Task insertDto(DtoViewModel viewModel, DtoEntity dto, bool persist, CancellationToken token = default)
        {
            dto.Module = null;
            _ = this._writeDbContext.Dtos.Add(dto).With(_ => viewModel.Guid = dto.Guid);
            if (persist)
            {
                _ = await this.SaveChangesAsync(token);
            }
            //x await this._securityDescriptor.SetSecurityDescriptorsAsync(viewModel, false, token);
        }

        Task insertProperties(DtoViewModel viewModel, bool persist, (DtoEntity Dto, IEnumerable<Property> Properties, IEnumerable<PropertyViewModel> PropertyViewModels) entity, CancellationToken token)
        {
            _ = this.PrepareProperties(viewModel);
            return this._propertyService.InsertProperties(entity.PropertyViewModels, entity.Dto.Id, false, token);
        }
    }

    public void ResetChanges()
        => this._writeDbContext.ChangeTracker.Clear();

    public Task<Result<int>> SaveChangesAsync(CancellationToken token = default)
        => this._writeDbContext.SaveChangesResultAsync(cancellationToken: token);

    public async Task<Result<DtoViewModel>> UpdateAsync(long id, DtoViewModel viewModel, bool persist = true, CancellationToken token = default)
    {
        var vr = await this.ValidateAsync(viewModel, token);
        if (vr.IsFailure)
        {
            return vr!;
        }
        this.ResetChanges();

        var entity = this.InitializeViewModel(viewModel)
            .PrepareProperties(viewModel)
            .ToDbEntity(viewModel);
        removeDeletedProperties(viewModel.DeletedProperties);
        updateDto(viewModel, entity.Dto, token);
        updateProperties(entity.PropertyViewModels, entity.Dto, token);

        var result = await this.SubmitChangesAsync(persist, token: token).With(_ => viewModel.Id = entity.Dto.Id);
        return Result.From(result, viewModel);

        void updateDto(DtoViewModel viewModel, DtoEntity dto, CancellationToken token = default)
            => this._writeDbContext.Attach(dto)
                    .SetModified(x => x.Name)
                    .SetModified(x => x.NameSpace)
                    .SetModified(x => x.Comment)
                    .SetModified(x => x.ModuleId)
                    .SetModified(x => x.IsParamsDto)
                    .SetModified(x => x.IsResultDto)
                    .SetModified(x => x.IsViewModel)
                    .SetEntryModified();
        void updateProperties(IEnumerable<PropertyViewModel> properties, DtoEntity dto, CancellationToken token = default)
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
        void removeDeletedProperties(IEnumerable<PropertyViewModel>? deletedProperties)
        {
            if (deletedProperties?.Any() is not true)
            {
                return;
            }
            foreach (var prop in deletedProperties)
            {
                if (prop?.Id is { } id)
                {
                    _ = this._writeDbContext.RemoveById<Property>(id);
                }
            }
        }
    }

    public async Task<Result<DtoViewModel?>> ValidateAsync(DtoViewModel? viewModel, CancellationToken token = default)
    {
        var validation = viewModel.Check()
            .ArgumentNotNull()
            .NotNullOrEmpty(x => x!.Name, () => "DTO name cannot be null.")
            .RuleFor(x => x!.Module?.Id is not null and not 0, () => "Module name cannot be null or zero.")
            .Build();
        if (!validation.IsSucceed)
        {
            return validation;
        }

        var query = from dto in this._readDbContext.Dtos
                    where dto.Name == viewModel!.Name && dto.Id != viewModel.Id
                    select dto.Id;
        if (await query.AnyAsync(cancellationToken: token))
        {
            return Result.Fail<DtoViewModel>(new ObjectDuplicateValidationException("DTO"));
        }

        var duplicates = viewModel!.Properties.FindDuplicates().Select(x => x?.Name).Compact().ToList();
        if (duplicates.Any())
        {
            return Result.Fail<DtoViewModel>(new Library.Exceptions.Validations.ValidationException($"{duplicates.Merge(",")} property name(s) are|is duplicated."));
        };
        return Result.Success(viewModel)!;
    }

    private DtoService InitializeViewModel(in DtoViewModel viewModel)
    {
        if (viewModel.Guid.IsNullOrEmpty())
        {
            viewModel.Guid = Guid.NewGuid();
        }
        return this;
    }

    private DtoService PrepareProperties(in DtoViewModel viewModel)
    {
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
        return this;
    }

    private (DtoEntity Dto, IEnumerable<Property> Properties, IEnumerable<PropertyViewModel> PropertyViewModels) ToDbEntity(in DtoViewModel viewModel)
    {
        var propsVm = viewModel.Properties.Copy();
        viewModel.Properties.Clear();
        var dto = this._converter.ToDbEntity(viewModel)!;
        var props = propsVm.Select(x => this._converter.ToDbEntity(x)).AsReadOnly();
        return (dto, props!, propsVm);
    }
}