using System.ComponentModel;
using System.Windows;

namespace HanyCo.Infra.UI.UserControls;

/// <summary>
/// Interaction logic for FieldPanel.xaml
/// </summary>
[DefaultProperty(nameof(AdditionalContent))]
//[DefaultBindingProperty(nameof(AdditionalContent))]
[Bindable(true)]
public partial class FieldPanel
{
    public static readonly DependencyProperty AdditionalContentProperty =
        DependencyProperty.Register(nameof(AdditionalContent), typeof(object), typeof(FieldPanel), new PropertyMetadata(null));

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(FieldPanel), new PropertyMetadata(null));

    public FieldPanel() =>
        this.InitializeComponent();

    [Bindable(true)]
    public object AdditionalContent
    {
        get => this.GetValue(AdditionalContentProperty);
        set => this.SetValue(AdditionalContentProperty, value);
    }

    [Bindable(true)]
    public string Title
    {
        get => (string)this.GetValue(TitleProperty);
        set => this.SetValue(TitleProperty, value);
    }
}