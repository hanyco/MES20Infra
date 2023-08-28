﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.Markers;

using Library.Data.Markers;
using Library.Helpers;
using Library.Threading;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

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

public enum CqrsSegregateCategory
{
    Create,
    Read,
    Update,
    Delete,
}

public enum CqrsSegregateType
{
    Query,
    Command
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
    ByteArray = 10,
    Dto = 20,
    Custom = 30,
}

public enum SecurityDescriptorStrategy
{
    None = 0,
    Claim = 1,
}

public enum TriggerType
{
    Load,
    Button,
    Submit,
    Reset
}

public partial class CqrsSegregate : IMesEntity
{
}

public partial class CrudCode : IMesEntity
{
}

public partial class Dto : IMesEntity
{
}

public partial class EntitySecurity : IIdenticalEntity<Guid>
{
}

public partial class Functionality : IMesEntity
{
}

[ReadDbContext]
public class InfraReadDbContext : InfraWriteDbContext
{
    public InfraReadDbContext()
        => this.InitializeInstance();

    public InfraReadDbContext(DbContextOptions<InfraWriteDbContext> options)
        : base(options)
        => this.InitializeInstance();

    public AsyncLock AsyncLock { get; private set; }

    [MemberNotNull(nameof(AsyncLock))]
    private void InitializeInstance()
    {
        this.AsyncLock = new();
        this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
}

[WriteDbContext]
public partial class InfraWriteDbContext : DbContext
{
#warning این جالبه

    [DbFunction(Name = "SOUNDEX", IsBuiltIn = true)]
    public static string SoundEx(string query) =>
        throw new NotImplementedException();
}

public partial class Module : IMesEntity
{
}

public partial class Property : IMesEntity
{
}

public partial class SecurityClaim : IIdenticalEntity<Guid>
{
}

public partial class SecurityDescriptor : IIdenticalEntity<Guid>
{
    [NotMapped]
    public SecurityDescriptorStrategy SecurityDescriptorStrategy
    {
        get => EnumHelper.ToEnum<SecurityDescriptorStrategy>(this.Strategy);
        set => this.Strategy = Convert.ToByte(value);
    }
}

public partial class SystemMenu : IMesEntity
{
}

public partial class UiBootstrapPosition : IMesEntity
{
}

public partial class UiComponent : IMesEntity
{
}

public partial class UiComponentAction : IMesEntity
{
}

public partial class UiComponentProperty : IMesEntity
{
}

public partial class UiPage : IMesEntity
{
}

public partial class UiPageComponent : IMesEntity
{
}

public partial class Translation : IMesEntity
{
}