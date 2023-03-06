using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Security.DataSources
{
    public partial class InfraSecDbContext
    {
        //public InfraSecDbContext()
        //{
        //}

        //public InfraSecDbContext(DbContextOptions<InfraSecDbContext> options)
        //    : base(options)
        public InfraSecDbContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.\\SqlExpress;Database=MesInfra;Integrated Security=True");
            }
        }
    }
}
