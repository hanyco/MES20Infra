#nullable disable

using HanyCo.Infra.CodeGen.Contracts.ViewModels;
using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration;

using System.Collections.ObjectModel;

namespace HanyCo.Infra.CodeGen.Domain.ViewModels;

public abstract class UiComponentViewModelBase : InfraViewModelBase, ICodeBase
{
    public ISet<(TypePath Type, string FieldName)> AdditionalInjects { get; } = new HashSet<(TypePath Type, string FieldName)>();
    public ISet<string> AdditionalUsings { get; } = new HashSet<string>();
    public ObservableCollection<(TypePath Type, string Name)> Parameters { get; } = [];
}