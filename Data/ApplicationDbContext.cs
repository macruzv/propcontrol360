using Microsoft.EntityFrameworkCore;
using propcontrol360.Models;

namespace propcontrol360.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Agent> Agents => Set<Agent>();
        public DbSet<Contract> Contracts => Set<Contract>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Agent)
                .WithMany(a => a.Properties)
                .HasForeignKey(p => p.AgentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Property)
                .WithMany(p => p.Contracts)
                .HasForeignKey(c => c.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Contracts)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Agent)
                .WithMany(a => a.Contracts)
                .HasForeignKey(c => c.AgentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
