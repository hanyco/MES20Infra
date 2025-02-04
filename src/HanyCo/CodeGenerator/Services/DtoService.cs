using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.Helpers;
using HanyCo.Infra.Internals.Data.DataSources;

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
using Microsoft.IdentityModel.Tokens;

using DtoEntity = HanyCo.Infra.Internals.Data.DataSources.Dto;

namespace Services;

internal sealed class DtoService(
    InfraReadDbContext readDbContext,
    InfraWriteDbContext writeDbContext,
    IEntityViewModelConverter converter,
    IPropertyService propertyService,
    ICodeGeneratorEngine codeGeneratorEngine)
    : IDtoService, IDtoCodeService, IAsyncValidator<DtoViewModel>, IAsyncSaveChanges, IResetChanges
{
    private readonly ICodeGeneratorEngine _codeGeneratorEngine = codeGeneratorEngine;
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly IPropertyService _propertyService = propertyService;
    private readonly InfraReadDbContext _readDbContext = readDbContext;
    private readonly InfraWriteDbContext _writeDbContext = writeDbContext;

    public Task<bool> AnyByName(string name)
    {
        var query = from dto in this._readDbContext.Dtos
                    where dto.Name == name
                    select dto.Id;
        return query.AnyLockAsync(this._readDbContext.AsyncLock);
    }

    public Task<DtoViewModel> CreateAsync(CancellationToken token = default) =>
        Task.FromResult(new DtoViewModel());

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
            return validationResult.WithValue(-1);
        }

        try
        {
            _ = await this._propertyService.DeleteByParentIdAsync(model.Id!.Value, false, token);
            _ = this._writeDbContext.RemoveById<DtoEntity>(model.Id!.Value);
            return await this.SubmitChanges(persist: persist, token: token);
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

        static Result<DtoViewModel> validate(DtoViewModel? model, CancellationToken token = default) =>
            model.Check()
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
        }
        ;

        return await this.DeleteAsync(dto!, persist, token);
    }

    public Result<Codes?> GenerateCodes(DtoViewModel viewModel, DtoCodeServiceAsyncCodeGeneratorArgs? arguments = null)
    {
        if (!validate(viewModel).TryParse(out var validationResult))
        {
            return validationResult.WithValue<Codes?>(default);
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
                //result.AddAttribute<SecurityAttribute>([("Key", claim.Key.NotNull()), ("Value", claim.Value?.ToString() ?? "null")]);
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
                //_ = type.AddAttribute<SecurityAttribute>(("Key", claim.Key.NotNull()), ("Value", claim.Value?.ToString() ?? "null"));
            }
        }
    }

    public async Task<IReadOnlyList<DtoViewModel>> GetAllAsync(CancellationToken token = default)
    {
        var query = from dto in this._readDbContext.Dtos
                    select dto;

        var dbResult = await query.AsNoTracking().ToListLockAsync(this._readDbContext.AsyncLock, cancellationToken: token);
        var result = this._converter.ToViewModel(dbResult).Compact().ToList();
        return result;
    }

    public async Task<IReadOnlySet<DtoViewModel>> GetAllByCategoryAsync(bool? paramsDtos, bool? resultDtos, bool? viewModels, CancellationToken token = default)
    {
        var rawQuery = from dto in this._readDbContext.Dtos.Include(x => x.Module)
                       select dto;
        var whereClause = generateWhereClause(paramsDtos, resultDtos, viewModels, token);
        var query = rawQuery.Where(whereClause).Select(dto => dto);

        var dbResult = await query.AsNoTracking().ToListLockAsync(this._readDbContext.AsyncLock, cancellationToken: token);
        var result = this._converter.ToViewModel(dbResult).Compact().ToReadOnlySet();
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
            var dbResult = await query.AsNoTracking().FirstOrDefaultLockAsync(this._readDbContext.AsyncLock, cancellationToken: token);

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
        var dbResult = await query.AsNoTracking().ToListLockAsync(this._readDbContext.AsyncLock, cancellationToken: token);
        var result = this._converter.ToViewModel(dbResult).Compact().ToList();
        return result;
    }

    public Task<IReadOnlyList<PropertyViewModel>> GetPropertiesByDtoIdAsync(long dtoId, CancellationToken token = default)
        => this._propertyService.GetByParentIdAsync(dtoId, token);

    public Task<Result<DtoViewModel>> Insert(DtoViewModel viewModel, bool persist = true, CancellationToken token = default) => CatchResultAsync(async () =>
    {
        await this.ValidateAsync(viewModel, token).ThrowOnFailAsync(token).End();

        var transaction = this._writeDbContext.Database.CurrentTransaction is null
            ? await this._writeDbContext.Database.BeginTransactionAsync(token)
            : null;
        try
        {
            // Initialize view model.
            _ = this.InitializeViewModel(viewModel);

            // Insert DTO
            var dto = await insertDto(viewModel, persist, token).ThrowIfCancellationRequested(token);

            // Insert DTO properties
            await insertProperties(viewModel, dto.Id, persist, token).ThrowIfCancellationRequested(token);

            if (persist)
            {
                await this.SaveChangesAsync(token).ThrowOnFailAsync(token).End();
                if (transaction is not null)
                {
                    await transaction.CommitAsync(token);
                }
                viewModel.Id = dto.Id;
            }

            return viewModel;
        }
        catch
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync(token);
            }
            throw;
        }
        finally
        {
            if (transaction != null)
            {
                await transaction.DisposeAsync();
            }
        }

        async Task<DtoEntity> insertDto(DtoViewModel viewModel, bool persist, CancellationToken token)
        {
            var (dto, _) = this.ToDbEntity(viewModel);
            dto.Module = null;
            var entry = await this._writeDbContext.Dtos.AddAsync(dto, token);
            if (persist)
            {
                _ = await this.SubmitChanges(true, token: token).ThrowOnFailAsync(cancellationToken: token);
            }
            return entry.Entity;
        }

        Task insertProperties(DtoViewModel viewModel, long dtoId, bool persist, CancellationToken token)
        {
            _ = this.PrepareProperties(viewModel);
            return this._propertyService.InsertProperties(viewModel.Properties, dtoId, persist, token).ThrowOnFailAsync(cancellationToken: token);
        }
    });

    public void ResetChanges()
        => this._writeDbContext.ChangeTracker.Clear();

    public Task<Result<int>> SaveChangesAsync(CancellationToken token = default)
        => this._writeDbContext.SaveChangesResultAsync(cancellationToken: token);

    public Task<Result<DtoViewModel>> Update(long id, DtoViewModel viewModel, bool persist = true, CancellationToken token = default) => CatchResultAsync(async () =>
    {
        _ = await this.ValidateAsync(viewModel, token).ThrowOnFailAsync();
        this.ResetChanges();

        var entity = this.InitializeViewModel(viewModel)
            .PrepareProperties(viewModel)
            .ToDbEntity(viewModel);
        removeDeletedProperties(viewModel.DeletedProperties);
        updateDto(viewModel, entity.Dto, token);
        updateProperties(viewModel.Properties, entity.Dto, token);

        var result = await this.SubmitChanges(persist, token: token).With(_ => viewModel.Id = entity.Dto.Id).ThrowOnFailAsync();
        return viewModel;

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
    });

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
            return Result.Fail(default(DtoViewModel?));
        }

        var duplicates = viewModel!.Properties.FindDuplicates().Select(x => x?.Name).Compact().ToList();
        if (duplicates.Any())
        {
            return Result.Fail<DtoViewModel?>(new ValidationException($"{duplicates.Merge(",")} property name(s) are|is duplicated."));
        }
        ;
        return Result.Success(viewModel)!;
    }

    private DtoService InitializeViewModel(in DtoViewModel viewModel)
    {
        // if viewModel.Guid is null or empty
        if (viewModel.Guid.IsNullOrEmpty())
        {
            // Initialize it
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

    private (DtoEntity Dto, IEnumerable<Property> Properties) ToDbEntity(in DtoViewModel viewModel)
    {
        var dto = this._converter.ToDbEntity(viewModel)!;
        var props = viewModel.Properties.Select(this._converter.ToDbEntity).Compact().AsReadOnly();
        return (dto, props);
    }
}