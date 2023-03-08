using HanyCo.Infra.UI.Services;

namespace InfraTestProject;

public class DtoServiceFixture : ServicesFixture
{
    private IDtoService? _service;

    public IDtoService Service => this._service ??= this.GetService<IDtoService>();
}