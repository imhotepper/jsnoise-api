using Microsoft.EntityFrameworkCore;

namespace CoreJsNoise.Domain
{
    public class PodcastsCtx: DbContext
    {
        public PodcastsCtx(DbContextOptions<PodcastsCtx> options) : base(options)
        {
        }
        
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Show> Shows { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producer>()
                .ToTable("Producers");
            modelBuilder.Entity<Show>()
                .ToTable("Shows")                
                .HasIndex(i => i.ProducerId);
        }
        
    }
}