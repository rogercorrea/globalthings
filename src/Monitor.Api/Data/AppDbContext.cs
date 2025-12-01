using Microsoft.EntityFrameworkCore;
using Monitor.Api.Models;

namespace Monitor.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    public DbSet<Measurement> Measurements => Set<Measurement>();
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<Equipment> Equipments => Set<Equipment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Measurement>()
            .HasIndex(m => new { m.Codigo, m.DataHoraMedicao })
            .IsUnique(false);

        modelBuilder.Entity<Sensor>()
            .HasIndex(s => s.Codigo)
            .IsUnique(true);
    }
}
