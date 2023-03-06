namespace HanyCo.Infra.Security.Model;

[Flags]
public enum Permission
{
    None,
    Read = 2,
    Self = 4,
    ReadSelf = Read | Self,
    Add = 8,
    Modify = 16,
    ModifySelf = Modify | Self,
    FullAccess = Read | Add | Modify,
    Special = 64,
}
