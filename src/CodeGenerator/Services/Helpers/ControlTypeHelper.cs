
using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Validations;

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