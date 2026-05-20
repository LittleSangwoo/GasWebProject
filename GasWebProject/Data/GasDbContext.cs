using Microsoft.EntityFrameworkCore;

namespace GasWebProject.Models
{
    public class GasDbContext : DbContext
    {
        public GasDbContext(DbContextOptions<GasDbContext> options) : base(options)
        {
        }

        public DbSet<GasComponent> Components { get; set; }

        // можно сразу засидить БД начальными данными для тестов
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GasComponent>().HasData(
                new GasComponent { Name = "N2", M = 28.01348, Z = 0.99976, Nominal = null },
                new GasComponent { Name = "CH4", M = 16.04276, Z = 0.99814, Nominal = 0.0005 },
                new GasComponent { Name = "C3H8", M = 44.09652, Z = 0.98306, Nominal = 0.00001 },
                new GasComponent { Name = "C2H4", M = 28.05376, Z = 0.99394, Nominal = 0.00004 },
                new GasComponent { Name = "C2H2", M = 26.0372, Z = 0.99270, Nominal = 0.000007 },
                new GasComponent { Name = "C2H6", M = 30.06964, Z = 0.99197, Nominal = 0.00015 }
            );
        }
    }
}
