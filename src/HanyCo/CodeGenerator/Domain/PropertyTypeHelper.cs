using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Validations;

namespace HanyCo.Infra.CodeGen.Domain;

public static class PropertyTypeHelper
{
    public static PropertyType FromPropertyTypeId(int propertyId) =>
        EnumHelper.ToEnum<PropertyType>(propertyId);

    public static (ControlType Control, dynamic? ExtraInfo) ToControlType(this PropertyType propertyType, bool? isList, bool? isNullable, DtoViewModel? dto) =>
        (propertyType, isList) switch
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

    public static string ToFullTypeName(this PropertyType propertyType, in string? dtoFullName = null) =>
        propertyType switch
        {
            PropertyType.None => string.Empty,
            PropertyType.String => typeof(string).FullName!,
            PropertyType.Integer => typeof(int).FullName!,
            PropertyType.Long => typeof(long).FullName!,
            PropertyType.Short => typeof(short).FullName!,
            PropertyType.Float => typeof(float).FullName!,
            PropertyType.Decimal => typeof(decimal).FullName!,
            PropertyType.Byte => typeof(byte).FullName!,
            PropertyType.Boolean => typeof(bool).FullName!,
            PropertyType.DateTime => typeof(DateTime).FullName!,
            PropertyType.Guid => typeof(Guid).FullName!,
            PropertyType.ByteArray => typeof(byte[]).FullName!,
            PropertyType.Dto => dtoFullName.NotNull(),
            PropertyType.Custom => throw new NotSupportedException(),
            _ => throw new NotSupportedException(),
        };

    public static PropertyType FromDbType(string dbType)
    {
        var value = Mapper().FirstOrDefault(x => x.DbTypes.Contains(dbType));
        return value == default ? PropertyType.Dto : value.propertyType;
    }

    internal static string? ToDbTypeName(this PropertyType propertyType) =>
        Mapper().FirstOrDefault(x => x.propertyType == propertyType).DbTypes?.FirstOrDefault();

    private static IEnumerable<(IEnumerable<string> DbTypes, Type netType, PropertyType propertyType)> Mapper()
    {
        yield return (EnumerableHelper.AsEnumerable(""), null, PropertyType.None);
        yield return (EnumerableHelper.AsEnumerable("uniqueidentifier"), typeof(Guid), PropertyType.Guid);
        yield return (EnumerableHelper.AsEnumerable("nvarchar", "varchar", "nchar"), typeof(string), PropertyType.String);
        yield return (EnumerableHelper.AsEnumerable("smallint"), typeof(short), PropertyType.Short);
        yield return (EnumerableHelper.AsEnumerable("int"), typeof(int), PropertyType.Integer);
        yield return (EnumerableHelper.AsEnumerable("bigint"), typeof(long), PropertyType.Long);
        yield return (EnumerableHelper.AsEnumerable("float"), typeof(float), PropertyType.Float);
        yield return (EnumerableHelper.AsEnumerable("decimal"), typeof(decimal), PropertyType.Decimal);
        yield return (EnumerableHelper.AsEnumerable("bit"), typeof(bool), PropertyType.Boolean);
        yield return (EnumerableHelper.AsEnumerable("byte"), typeof(byte), PropertyType.Byte);
        yield return (EnumerableHelper.AsEnumerable("varbinary"), typeof(byte[]), PropertyType.ByteArray);
        yield return (EnumerableHelper.AsEnumerable("datetime", "datetime2", "datetimeoffset", "date"), typeof(byte[]), PropertyType.DateTime);
    }
}