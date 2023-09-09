using Contracts.ViewModels;

using Library.Interfaces;

namespace Contracts.Services;

public interface ISecurityService : IBusinessService, IAsyncCrud<ClaimViewModel>
{
}