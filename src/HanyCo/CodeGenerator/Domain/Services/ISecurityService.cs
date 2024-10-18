using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface ISecurityService : IBusinessService, IAsyncCrud<ClaimViewModel, Guid>
{
    Task<Result<IEnumerable<ClaimViewModel>>> GetEntityClaims(Guid entity, CancellationToken token = default);

    Task<Result> RemoveEntityClaims(Guid value, bool persist, CancellationToken token = default);

    Task<Result> SetEntityClaims(Guid entity, IEnumerable<ClaimViewModel> claims, bool persist, CancellationToken token = default);
}