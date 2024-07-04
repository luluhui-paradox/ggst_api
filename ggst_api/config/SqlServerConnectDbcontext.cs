using ggst_api.entity;
using Microsoft.EntityFrameworkCore;

namespace ggst_api.config
{
    public class SqlServerConnectDbcontext:DbContext
    {
        public virtual DbSet<PlayerInfoEntity> PlayerInfoEntities { get; set; }

        public SqlServerConnectDbcontext( DbContextOptions<SqlServerConnectDbcontext> options ):base(options) { 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerInfoEntity>()
                .HasKey(e => new { e.id, e.character });
        }
    }
}
