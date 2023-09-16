using System.Windows;
using System.Windows.Controls;
using MyType = HanyCo.Infra.UI.UserControls.PartialDetailsForCommandHandlerUserControl;

namespace HanyCo.Infra.UI.UserControls;

/// <summary>
/// Interaction logic for PartialDetailsForCommandHandlerUserControl.xaml
/// </summary>
public partial class PartialDetailsForCommandHandlerUserControl : UserControl
{
    #region bool IsDefaultBody

    public static readonly DependencyProperty IsDefaultBodyProperty = ControlHelper.GetDependencyProperty<bool, MyType>(nameof(IsDefaultBody));

    public bool IsDefaultBody
    {
        get => (bool)this.GetValue(IsDefaultBodyProperty);
        set => this.SetValue(IsDefaultBodyProperty, value);
    }

    #endregion bool IsDefaultBody

    #region bool IsCodeSnippetBody

    public static readonly DependencyProperty IsCodeSnippetBodyProperty = ControlHelper.GetDependencyProperty<bool, MyType>(nameof(IsCodeSnippetBody));

    public bool IsCodeSnippetBody
    {
        get => (bool)this.GetValue(IsCodeSnippetBodyProperty);
        set => this.SetValue(IsCodeSnippetBodyProperty, value);
    }

    #endregion bool IsCodeSnippetBody

    #region bool IsCommandBody

    public static readonly DependencyProperty IsCommandBodyProperty = ControlHelper.GetDependencyProperty<bool, MyType>(nameof(IsCommandBody));

    public bool IsCommandBody
    {
        get => (bool)this.GetValue(IsCommandBodyProperty);
        set => this.SetValue(IsCommandBodyProperty, value);
    }

    #endregion bool IsCommandBody

    #region string CodeSnippetBodyText

    public static readonly DependencyProperty CodeSnippetBodyTextProperty = ControlHelper.GetDependencyProperty<string, MyType>(nameof(CodeSnippetBodyText), defaultValue: "throw new System.NotImplementedException();5");

    public string CodeSnippetBodyText
    {
        get => (string)this.GetValue(CodeSnippetBodyTextProperty);
        set => this.SetValue(CodeSnippetBodyTextProperty, value);
    }

    #endregion string CodeSnippetBodyText

    #region string DataContextFullName

    public static readonly DependencyProperty DataContextFullNameProperty = ControlHelper.GetDependencyProperty<string, MyType>(nameof(DataContextFullName), defaultValue: "throw new System.NotImplementedException();6");

    public string DataContextFullName
    {
        get => (string)this.GetValue(DataContextFullNameProperty);
        set => this.SetValue(DataContextFullNameProperty, value);
    }

    #endregion string DataContextFullName

    public PartialDetailsForCommandHandlerUserControl() => this.InitializeComponent();

    private void Me_Loaded(object sender, RoutedEventArgs e)
    {
        this.IsDefaultBody = true;
        this.CodeSnippetBodyText = "throw new System.NotImplementedException();2";
    }
}