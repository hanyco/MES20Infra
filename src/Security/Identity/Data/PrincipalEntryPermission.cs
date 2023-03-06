namespace HanyCo.Infra.Security.DataSources
{
    /// <summary>
    /// مجوزهای هر شخص بر اساس موجودیتهای سیستم
    /// </summary>
    public partial class PrincipalEntryPermission
    {
        public long Id { get; set; }
        public long SystemEntryId { get; set; }
        public long PrincipalId { get; set; }
        public short PermissionType { get; set; }
        public short? ApplyTo { get; set; }
        public short PermissionId { get; set; }
        public string? MoreInfoJson { get; set; }

        public virtual Principal Principal { get; set; }
        public virtual SystemEntry SystemEntry { get; set; }
    }
}
