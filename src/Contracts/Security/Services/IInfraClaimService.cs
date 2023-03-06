using HanyCo.Infra.Security.Model;

namespace HanyCo.Infra.Security.Services;

public interface IInfraClaimService
{
    Task AddClaim(IClaimModel claim);
}