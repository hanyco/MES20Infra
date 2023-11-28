namespace Contracts.ViewModels;

public sealed class SecurityDescriptorViewModel : InfraViewModelBase
{
    public SecurityDescriptorViewModel()
    {
    }

    public SecurityDescriptorViewModel(long? id, string? name) : base(id, name)
    {
    }
}