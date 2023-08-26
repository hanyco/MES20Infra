using System.Collections.ObjectModel;

namespace HanyCo.Infra.UI.ViewModels;

public interface IHasSecurityDescriptor
{
    public Guid? Guid { get; set; }

    ObservableCollection<SecurityDescriptorViewModel> SecurityDescriptors { get; }
}