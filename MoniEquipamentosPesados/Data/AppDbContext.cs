using ApiMoniEquipamentosPesados.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiMoniEquipamentosPesados.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Equipamento> Equipamentos => Set<Equipamento>();
    public DbSet<Manutencao> Manutencoes => Set<Manutencao>(); 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Equipamento>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Codigo).IsUnique();

            entity.Property(e => e.Modelo).IsRequired();

            entity.ToTable(t => t.HasCheckConstraint("CK_Equipamento_Horimetro", "\"Horimetro\" >= 0"));
        });
    }
}