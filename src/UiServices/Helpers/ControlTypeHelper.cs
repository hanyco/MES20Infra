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
        Check.MustBeArgumentNotNull(dto);
        return dto.Properties.Count switch
        {
            0 => ControlType.None,
            1 => dto.Properties.First().Type.ToControlType(null, null, dto).Control,
            > 1 => ControlType.DataGrid,
            _ => throw new NotImplementedException()
        };
    }

    public static ControlType FromControlTypeId(int controlTypeId) =>
        EnumHelper.ToEnum<ControlType>(controlTypeId);
}

public static class TriggerTypeHelper
{
    public static ButtonType ToButtonType(this TriggerType triggerType) =>
        triggerType switch
        {
            TriggerType.FormButton => ButtonType.FormButton,
            TriggerType.RowButton => ButtonType.RowButton,
            _ => throw new NotImplementedException(),
        };
}

public enum TriggerKind
{
    Button,
    Load
}