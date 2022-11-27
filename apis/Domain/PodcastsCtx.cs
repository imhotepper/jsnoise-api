using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
    
    
    public class PodcastsCtxFactory : IDesignTimeDbContextFactory<PodcastsCtx>
    {
        public PodcastsCtx CreateDbContext(string[] args)
        {
            var connStr = Environment.GetEnvironmentVariable("DATABASE_URL")??
                          "User ID=postgres;Password=;Server=localhost;Port=5432;Database=podcasts-db2;Integrated Security=true;Pooling=true;";
            var optionsBuilder = new DbContextOptionsBuilder<PodcastsCtx>();
            optionsBuilder.UseNpgsql(connStr);

            return new PodcastsCtx(optionsBuilder.Options);
        }
    }
}