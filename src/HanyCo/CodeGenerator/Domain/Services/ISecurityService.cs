using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface ISecurityService : IBusinessService, IAsyncCrud<ClaimViewModel, Guid>
{
    Task<Result<IEnumerable<ClaimViewModel>>> GetEntityClaimsAsync(Guid entity, CancellationToken token = default);

    Task<Result> RemoveEntityClaimsAsync(Guid value, bool persist, CancellationToken token);

    Task<Result> SetEntityClaimsAsync(Guid entity, IEnumerable<ClaimViewModel> claims, bool persist, CancellationToken token = default);
}