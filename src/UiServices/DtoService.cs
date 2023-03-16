using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using Contracts.Services;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Actors;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components;
using HanyCo.Infra.CodeGeneration.Helpers;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Data.Linq;
using Library.Exceptions.Validations;
using Library.Helpers;
using Library.Helpers.CodeGen;
using Library.Interfaces;
using Library.Mapping;
using Library.Results;
using Library.Validations;
using Library.Windows;

using Microsoft.EntityFrameworkCore;

using DtoEntity = HanyCo.Infra.Internals.Data.DataSources.Dto;

namespace Services;

internal sealed class DtoService : IDtoService, IDtoCodeService,
    IAsyncValidator<DtoViewModel>, IAsyncSaveService, IResetChanges
{
    private readonly IEntityViewModelConverter _converter;
    private readonly IPropertyService _propertyService;
    private readonly InfraReadDbContext _readDbContext;
    private readonly ISecurityDescriptorService _securityDescriptor;
    private readonly InfraWriteDbContext _writeDbContext;

    public DtoService(InfraReadDbContext readDbContext,
                      InfraWriteDbContext writeDbContext,
                      IEntityViewModelConverter converter,
                      ISecurityDescriptorService securityDescriptor,
                      IPropertyService propertyService)
    {
        this._readDbContext = readDbContext;
        this._writeDbContext = writeDbContext;
        this._converter = converter;
        this._securityDescriptor = securityDescriptor;
        this._propertyService = propertyService;
    }

    public Task<DtoViewModel> CreateAsync() 
        => Task.FromResult(new DtoViewModel());

    public DtoViewModel CreateByDbTable(in DbTableViewModel table, in IEnumerable<DbColumnViewModel> columns)
    {
        var result = new DtoViewModel
        {
            DbObject = table,
            Name = $"Get{table.Name}ParamsDto",
        };
        if (columns?.Any() is true)
        {
            _ = columns.Compact().Select(x => new PropertyViewModel
            {
                DbObject = x,
                Name = x.Name,
                Type = PropertyTypeHelper.FromDbType(x.DbType),
                IsNullable = x.IsNullable,
                Id = x.ObjectId * -1,
            }).ForEach(result.Properties.Add).Build();
        }

        return result;
    }

    public async Task<Result> DeleteAsync(DtoViewModel model, bool persist)
    {
        Check.IfArgumentNotNull(model);

        try
        {
            await deleteProperties(model);
            await deleteDto(model);
            return await this.SubmitChangesAsync(persist: persist).ThrowOnFailAsync();
        }
        catch (DbUpdateException ex) when (ex.GetBaseException().Message.Contains("FK_CqrsSegregate_Dto"))
        {
            var message = new NotificationMessage("This action can not be completed because this DTO has been referenced a CQRS Segregate.",
                                                  "Can not delete DTO.",
                                                  "DTO In Use",
                                                  "In order to delete this DTO, delete the CQRS Segregate and try again.");
            return Result.CreateFail(message);
        }
        catch (DbUpdateException ex) when (ex.GetBaseException().Message.Contains("FK_UiComponent_Property"))
        {
            var message = new NotificationMessage("This action can not be completed because this DTO has been referenced a UI Component property.",
                                                  "Can not delete DTO.",
                                                  "DTO In Use",
                                                  "In order to delete this DTO, delete the UI Component property and try again.");
            return Result.CreateFail(message);
        }

        async Task deleteDto(DtoViewModel dto)
        {
            _ = this._writeDbContext.RemoveById<DtoEntity>(dto.Id!.Value);
            await Task.CompletedTask;
        }

        async Task deleteProperties(DtoViewModel dto)
            => await this._propertyService.DeleteByParentIdAsync(dto.Id!.Value, false);
    }

    public Codes GenerateCodes(in DtoViewModel viewModel, GenerateCodesParameters? arguments = null)
    {
        Check.IfArgumentNotNull(viewModel);

        var result = new Codes();
        var codeGen = convertViewModelToCodeGen(viewModel);
        var code = codeGen.GenerateCode(viewModel.NameSpace);

        return result.Add(code);

        static CodeGenDto convertViewModelToCodeGen(DtoViewModel resultViewModel)
        {
            var result = CodeGenDto.New(TypeMemberNameHelper.GetFullName(resultViewModel.NameSpace, resultViewModel.Name!));
            foreach (var prop in resultViewModel.Properties)
            {
                _ = result.AddProp(CodeGenType.New(prop.TypeFullName), prop.Name!, prop.IsList ?? false, prop.IsNullable ?? false, comment: prop.Comment);
            }
            return result;
        }
    }

    public async Task<IReadOnlyList<DtoViewModel>> GetAllAsync()
    {
        var query = from dto in this._readDbContext.Dtos
                    select dto;

        var dbResult = await query.ToListLockAsync(this._readDbContext.AsyncLock);
        var result = this._converter.FillByDbEntity(dbResult).ToList();
        return result;
    }

    public async Task<IReadOnlySet<DtoViewModel>> GetAllByCategoryAsync(bool paramsDtos, bool resultDtos, bool viewModels)
    {
        var rawQuery = from dto in this._readDbContext.Dtos
                       select dto;
        var whereClause = generateWhereClause(paramsDtos, resultDtos, viewModels);
        var query = rawQuery.Where(whereClause).Select(dto => dto);

        var dbResult = await query.ToListLockAsync(this._readDbContext.AsyncLock);
        var result = this._converter.FillByDbEntity(dbResult).ToReadOnlySet();
        return result;

        static Expression<Func<DtoEntity, bool>> generateWhereClause(bool paramsDtos, bool resultDtos, bool viewModels)
        {
            var whereClause = PredicateBuilder.False<DtoEntity>();
            if (paramsDtos)
            {
                whereClause = whereClause.Or(dto => dto.IsParamsDto);
            }
            if (resultDtos)
            {
                whereClause = whereClause.Or(dto => dto.IsResultDto);
            }
            if (viewModels)
            {
                whereClause = whereClause.Or(dto => dto.IsViewModel);
            }

            return whereClause;
        }
    }

    public async Task<DtoViewModel?> GetByIdAsync(long id)
    {
        var dbResult = await getDto(id);
        if (dbResult is null)
        {
            return null;
        }
        var dtoSecs = await getDtoSecurityDescriptor(dbResult);
        var properties = await getProperties(dbResult);
        var result = this._converter.ToViewModel(dbResult, dtoSecs)!.ForMember(x => x.Properties.AddRange(properties));

        return result;

        async Task<DtoEntity?> getDto(long id)
        {
            var query = from x in this._readDbContext.Dtos.Include(x => x.Module)
                        where x.Id == id
                        select x;
            var dbResult = await query.FirstOrDefaultLockAsync(this._readDbContext.AsyncLock);
            return dbResult;
        }

        Task<IReadOnlyList<PropertyViewModel>> getProperties(DtoEntity dbResult)
            => this._propertyService.GetByParentIdAsync(dbResult.Id);

        async Task<IEnumerable<SecurityDescriptorViewModel>> getDtoSecurityDescriptor(DtoEntity dbResult)
            => dbResult.Guid.IsNullOrEmpty()
                    ? new List<SecurityDescriptorViewModel>()
                    : await this._securityDescriptor.GetByEntityIdAsync(dbResult.Guid);
    }

    public async Task<IReadOnlyList<DtoViewModel>> GetByModuleId(long id)
    {
        var query = from dto in this._readDbContext.Dtos
                    where dto.ModuleId == id
                    select dto;
        var dbResult = await query.ToListLockAsync(this._readDbContext.AsyncLock);
        var result = this._converter.FillByDbEntity(dbResult).ToList();
        return result;
    }

    public Task<IReadOnlyList<PropertyViewModel>> GetPropertiesByDtoIdAsync(long dtoId)
        => this._propertyService.GetByParentIdAsync(dtoId);

    public async Task<Result<DtoViewModel>> InsertAsync(DtoViewModel viewModel, bool persist = true)
    {
        _ = await this.CheckValidatorAsync(viewModel);
        _ = InitializeViewModel(viewModel);
        var entity = this.ToDbEntity(viewModel);

        await using var transaction = await this._writeDbContext.Database.BeginTransactionAsync();
        await insertDto(viewModel, entity.Dto);
        await insertProperties(entity.PropertyViewModels, entity.Dto.Id);
        var result = await this.SubmitChangesAsync(persist, transaction).With(_ => viewModel.Id = entity.Dto.Id);
        return Result<DtoViewModel>.From(result, viewModel);

        async Task insertDto(DtoViewModel viewModel, DtoEntity dto)
        {
            _ = await this._writeDbContext.ReAttach(dto.Module!).DbContext
                                          .Dtos.Add(dto)
                                          .SaveChangesAsync()
                                          .With(_ => viewModel.Guid = dto.Guid);
            await this._securityDescriptor.SetSecurityDescriptorsAsync(viewModel, false);
        }
        async Task insertProperties(IEnumerable<PropertyViewModel> properties, long parentEntityId)
        {
            foreach (var property in properties)
            {
                property.ParentEntityId = parentEntityId;
                _ = await this._propertyService.InsertAsync(property, false);
                if (property.SecurityDescriptors?.Any() is true)
                {
                    await this._securityDescriptor.SetSecurityDescriptorsAsync(property, false);
                }
            }
        }
    }

    public void ResetChanges()
        => this._writeDbContext.ChangeTracker.Clear();

    public Task<Result<int>> SaveChangesAsync()
        => this._writeDbContext.SaveChangesResultAsync();

    public async Task<Result<DtoViewModel>> UpdateAsync(long id, DtoViewModel viewModel, bool persist = true)
    {
        _ = await this.CheckValidatorAsync(viewModel);
        _ = InitializeViewModel(viewModel);
        var entity = this.ToDbEntity(viewModel);
        this.ResetChanges();

        await using var transaction = await this._writeDbContext.Database.BeginTransactionAsync();
        await removeDeletedProperties(viewModel.DeletedProperties);
        await updateDto(viewModel, entity.Dto);
        await updateProperties(entity.PropertyViewModels, entity.Dto);
        var result = await this.SubmitChangesAsync(persist, transaction).With(_ => viewModel.Id = entity.Dto.Id);
        return Result<DtoViewModel>.From(result, viewModel);

        async Task updateDto(DtoViewModel viewModel, DtoEntity dto)
        {
            _ = this._writeDbContext.Attach(dto)
                                    .SetModified(x => x.Name)
                                    .SetModified(x => x.NameSpace)
                                    .SetModified(x => x.Comment)
                                    .SetModified(x => x.ModuleId)
                                    .SetModified(x => x.IsParamsDto)
                                    .SetModified(x => x.IsResultDto)
                                    .SetModified(x => x.IsViewModel);

            await this._securityDescriptor.SetSecurityDescriptorsAsync(viewModel, false);
        }
        async Task updateProperties(IEnumerable<PropertyViewModel> properties, DtoEntity dto)
        {
            foreach (var prop in properties)
            {
                var property = this._converter.ToDbEntity(prop, dto.Id)!;
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
                await this._securityDescriptor.SetSecurityDescriptorsAsync(prop, false);
            }
        }
        async Task removeDeletedProperties(IEnumerable<PropertyViewModel>? deletedProperties)
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

    public async Task<Result<DtoViewModel>> ValidateAsync([DisallowNull] DtoViewModel viewModel)
    {
        Check.IfArgumentNotNull(viewModel);

        var result = viewModel.Check()
            .NotNullOrEmpty(x => x.Name, () => "DTO name cannot be null.")
            .RuleFor(x => x.Module?.Id is not null or 0, () => "Module name cannot be null.")
            .BuildAll();
        if (!result.IsSucceed)
        {
            return result;
        }

        var query = from dto in this._readDbContext.Dtos
                    where dto.Name == viewModel!.Name && dto.Id != viewModel.Id
                    select dto.Id;
        _ = result.Check(await query.AnyAsync(), "DTO name already exists.", ObjectDuplicateValidationException.ErrorCode);
        var duplicates = viewModel!.Properties.GroupBy(x => x.Name).Where(g => g.Count() > 1).Select(y => y.Key).Compact().ToList();
        _ = result.Check(duplicates.Count > 0, $"{duplicates.Merge(",")} property name(s are) is duplicated.", ObjectDuplicateValidationException.ErrorCode);
        return result;
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
        var propsVm = viewModel.Properties.ToEnumerable().ToList();
        viewModel.Properties.Clear();
        var dto = this._converter.ToDbEntity(viewModel)!;
        var props = propsVm.ConvertAll(x => this._converter.ToDbEntity(x, dto.Id));
        return (dto, props!, propsVm);
    }
}