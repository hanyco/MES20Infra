namespace HanyCo.Infra.Security.Identity.Data
{
    public partial class SystemMenu
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Caption { get; set; }
        public string Uri { get; set; }
    }
}
