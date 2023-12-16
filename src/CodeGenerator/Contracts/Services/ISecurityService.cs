using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;
using Library.Results;

namespace Contracts.Services;

public partial interface ISecurityCrudService : IBusinessService, IAsyncCrud<ClaimViewModel, Guid>, IAsyncSaveChanges, IResetChanges
{
    Task<Result<IEnumerable<ClaimViewModel>>> GetEntityClaimsAsync(Guid entity, CancellationToken token = default);

    Task<Result> RemoveEntityClaimsByEntityIdAsync(Guid value, bool persist, CancellationToken token);

    Task<Result> SetEntityClaimsAsync(Guid entity, IEnumerable<ClaimViewModel> claims, bool persist, CancellationToken token = default);
}

public readonly struct SecurityCodeGeneratorArgs
{
}

public interface ISecurityCodeGeneratorService : IBusinessService, ICodeGenerator<SecurityCodeGeneratorArgs>
{
}