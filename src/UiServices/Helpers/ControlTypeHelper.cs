using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Helpers;

using Library.Validations;

using static HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements.HtmlButton;

namespace Services.Helpers;

public static class ControlTypeHelper
{
    public static ControlType ByDtoViewModel(DtoViewModel dto)
    {
        Check.IfArgumentNotNull(dto);
        return dto.Properties.Count switch
        {
            0 => ControlType.None,
            1 => dto.Properties.First().Type.ToControlType(null, null, dto).Control,
            > 1 => ControlType.DataGrid,
            _ => throw new NotImplementedException()
        };
    }

    public static ControlType FromControlTypeId(int controltypeId)
        => EnumHelper.ToEnum<ControlType>(controltypeId);
}

//public sealed class ControlTypeProperties : Expando
//{
//}

public static class TriggerTypeHelper
{
    public static TriggerKind GetKind(this TriggerType triggerType) => triggerType switch
    {
        TriggerType.Button => TriggerKind.Button,
        TriggerType.Submit => TriggerKind.Button,
        TriggerType.Reset => TriggerKind.Button,
        TriggerType.Load => TriggerKind.Load,
        _ => throw new NotImplementedException()
    };
    public static ButtonType ToButtonType(this TriggerType triggerType) => triggerType switch
    {
        TriggerType.Button => ButtonType.Button,
        TriggerType.Submit => ButtonType.Submit,
        TriggerType.Reset => ButtonType.Reset,
        _ => throw new NotImplementedException(),
    };
}
public enum TriggerKind
{
    Button,
    Load
}