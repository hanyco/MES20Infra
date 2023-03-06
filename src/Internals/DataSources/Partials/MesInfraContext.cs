using HanyCo.Infra.Data;
using HanyCo.Infra.Interfaces.Markers;
using HanyCo.Infra.Threading;
using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    [WriteDbContext]
    public partial class InfraWriteDbContext : MesDbContextBase
    {
        public InfraWriteDbContext(string connectionString) : base(connectionString)
        {
        }
    }

    [ReadDbContext]
    public class InfraReadDbContext : InfraWriteDbContext
    {
        public AsyncLock AsyncLock { get; private set; }

        public InfraReadDbContext()
            => this.InitializeInstance();
        public InfraReadDbContext(string connectionString) : base(connectionString)
            => this.InitializeInstance();
        public InfraReadDbContext(DbContextOptions<InfraWriteDbContext> options) : base(options)
            => this.InitializeInstance();

        private void InitializeInstance()
        {
            this.AsyncLock = new();
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
    }

    public enum PropertyType
    {
        None,
        String = 1,
        Interger = 2,
        Long = 3,
        Short = 4,
        Float = 5,
        Byte = 6,
        Boolean = 7,
        DateTime = 8,
        Guid = 9,
        Dto = 20,
        Custom = 30,
    }
    public enum CqrsSegregateType
    {
        Query,
        Command
    }
    public enum CqrsSegregateCategory
    {
        Create,
        Read,
        Update,
        Delete,
    }
    public enum ControlType
    {
        None = 0,
        RadioButton = 10,
        CheckBox = 20,
        TextBox = 30,
        DropDown = 40,
        DateTimePicker = 50,
        NumericTextBox = 60,
        LookupBox = 70,
        ImageUpload = 80,
        FileUpload = 90,
        DataGrid = 100,
        CurrencyBox = 110,
        ExternalViewBox = 120,
        Component = 130,
    }
    public enum TriggerType
    {
        Load,
        Submit
    }

    public partial class Property : IMesEntity
    {

    }

    public partial class Dto : IMesEntity
    {

    }

    public partial class Module : IMesEntity
    {

    }

    public partial class CqrsSegregate : IMesEntity
    {

    }
}
