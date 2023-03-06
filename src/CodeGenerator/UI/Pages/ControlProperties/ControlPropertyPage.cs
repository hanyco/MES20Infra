namespace HanyCo.Infra.UI.Pages.ControlProperties;

public class ControlPropertyPage : System.Windows.Controls.Page
{
    public ControlPropertyPage()
    {
    }

    public void Ok() => this.OnOk();

    protected virtual void OnOk()
    { }
}