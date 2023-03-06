namespace HanyCo.Infra.Security.DataSources
{
    /// <summary>
    /// کاربر، سمت، یا گروه
    /// </summary>
    public partial class Principal
    {
        public Principal() => this.PrincipalEntryPermissions = new HashSet<PrincipalEntryPermission>();

        public long Id { get; set; }
        public long ExternalId { get; set; }
        public long? ExternalParentId { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateorUserId { get; set; }

        public virtual ICollection<PrincipalEntryPermission> PrincipalEntryPermissions { get; set; }
    }
}
