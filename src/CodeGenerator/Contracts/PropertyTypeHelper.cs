using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.Validations;

namespace HanyCo.Infra.UI.Helpers;

public static class PropertyTypeHelper
{
    public static PropertyType FromDbType(string dbType)
        => dbType switch
        {
            "nvarchar" or "varchar" or "nchar" => PropertyType.String,
            "int" => PropertyType.Integer,
            "bigint" => PropertyType.Long,
            "smallint" => PropertyType.Short,
            "float" => PropertyType.Float,
            "byte" => PropertyType.Byte,
            "bit" => PropertyType.Boolean,
            "datetime" or "datetime2" or "datetimeoffset" or "date" => PropertyType.DateTime,
            "uniqueidentifier" => PropertyType.Guid,
            "varbinary" => PropertyType.ByteArray,
            _ => throw new NotSupportedException("Not supported Db Type"),
        };

    public static PropertyType FromPropertyTypeId(int propertyId)
        => EnumHelper.ToEnum<PropertyType>(propertyId);

    public static (ControlType Control, dynamic? ExtraInfo) ToControlType(this PropertyType propertyType, bool? isList, bool? isNullable, DtoViewModel? dto)
        => (propertyType, isList) switch
        {
            (PropertyType.String, false or null) => (ControlType.TextBox, new { IsNullable = isNullable }),
            (PropertyType.Boolean, false or null) => (ControlType.CheckBox, new { IsNullable = isNullable }),
            (PropertyType.DateTime, false or null) => (ControlType.DateTimePicker, new { IsNullable = isNullable }),
            (PropertyType.Guid, false or null) => (ControlType.TextBox, new { IsNullable = isNullable }),
            (PropertyType.Integer, false or null) => (ControlType.NumericTextBox, (Numeric: (32, 0), IsNullable: isNullable)),
            (PropertyType.Long, false or null) => (ControlType.NumericTextBox, (Numeric: (64, 0), IsNullable: isNullable)),
            (PropertyType.Short, false or null) => (ControlType.NumericTextBox, (Numeric: (2, 0), IsNullable: isNullable)),
            (PropertyType.Float, false or null) => (ControlType.NumericTextBox, (Numeric: (32, 10), IsNullable: isNullable)),
            (PropertyType.Byte, false or null) => (ControlType.NumericTextBox, (Numeric: (1, 0), IsNullable: isNullable)),
            (PropertyType.ByteArray, false or null) => (ControlType.ImageUpload, (Numeric: (1, 0), IsNullable: isNullable)),
            (PropertyType.None, false or null) => throw new NotSupportedException(),
            (PropertyType.Dto, _) => (ControlType.Component, new { IsList = isList, IsNullable = isNullable, Dto = dto }),
            (PropertyType.Custom, _) => throw new NotSupportedException(),
            (_, true) => (ControlType.DropDown, new { IsNullable = isNullable }),
            _ => throw new NotSupportedException(),
        };

    public static string ToFullTypeName(this PropertyType propertyType, in string? dtoFullName = null)
        => propertyType switch
        {
            PropertyType.None => string.Empty,
            PropertyType.String => typeof(string).FullName!,
            PropertyType.Integer => typeof(int).FullName!,
            PropertyType.Long => typeof(long).FullName!,
            PropertyType.Short => typeof(short).FullName!,
            PropertyType.Float => typeof(float).FullName!,
            PropertyType.Byte => typeof(byte).FullName!,
            PropertyType.Boolean => typeof(bool).FullName!,
            PropertyType.DateTime => typeof(DateTime).FullName!,
            PropertyType.Guid => typeof(Guid).FullName!,
            PropertyType.ByteArray => typeof(byte[]).FullName!,
            PropertyType.Dto => dtoFullName.NotNull(),
            PropertyType.Custom => throw new NotSupportedException(),
            _ => throw new NotSupportedException(),
        };
}