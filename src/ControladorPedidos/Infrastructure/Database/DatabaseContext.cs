using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ControladorPedidos.Infrastructure.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        ChangeTracker.StateChanged += UpdateBaseEntity;
        ChangeTracker.Tracked += UpdateBaseEntity;
    }

    public DbSet<Categoria> CategoriaProdutos { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Pedido> Pedidos { get; set; } = null!;
    public DbSet<Produto> Produtos { get; set; } = null!;

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
    }

    private void UpdateBaseEntity(object? sender, EntityEntryEventArgs e)
    {
        if (e.Entry.Entity is EntityBase baseEntity)
        {
            if (e.Entry.State == EntityState.Added)
            {
                baseEntity.CriadoEm = DateTime.UtcNow;
                baseEntity.AtualizadoEm ??= null!;
            }
            else if (e.Entry.State == EntityState.Modified)
            {
                baseEntity.AtualizadoEm = DateTime.UtcNow;
            }
            else
            {
                //Não fazer nada
            }
        }
    }
}