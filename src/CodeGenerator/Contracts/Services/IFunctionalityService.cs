using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.ViewModels;

using Library.Interfaces;
using Library.Results;
using Library.Wpf.Windows;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface IFunctionalityService : IBusinessService, IAsyncCrud<FunctionalityViewModel>, IAsyncCreator<FunctionalityViewModel>
{
    
}