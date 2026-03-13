using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Shared.Context
{
    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions<BaseContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                throw new InvalidOperationException("O DbContextOptions deve ser configurado externamente.");
            }
        }

        //public DbSet<Cinema> Cinemas { get; set; }

        //public DbSet<Tenant> Tenants { get; set; }
        //public DbSet<Company> Companies { get; set; }
        //public DbSet<Unity> Unities { get; set; }
        //public DbSet<TestEntity> TestEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
