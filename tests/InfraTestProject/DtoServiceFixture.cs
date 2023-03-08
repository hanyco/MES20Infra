using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Services;

using Library.Helpers;

namespace InfraTestProject;

public sealed class DtoServiceFixture : ServicesFixture<IDtoService> {

    protected override void OnInitializingService(in IDtoService service)
    {
        this.WriteDbContext.Add(new Module { Id = 1, Name = "Human Resources", Guid = Guid.NewGuid() });
        this.WriteDbContext.SaveChanges();
    }
}