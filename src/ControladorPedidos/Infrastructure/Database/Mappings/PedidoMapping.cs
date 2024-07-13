using ControladorPedidos.Application.Pedidos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControladorPedidos.Infrastructure.Database.Mappings;

public class PedidoMapping : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedido");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ClienteId);
        builder.HasOne(x => x.Cliente).WithMany(x => x.Pedidos).HasForeignKey(x => x.ClienteId);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.ValorTotal).IsRequired();
        builder.HasMany(x => x.Produtos).WithMany(x => x.Pedidos);
        builder.Property(x => x.CriadoEm).IsRequired();
        builder.Property(x => x.AtualizadoEm).IsRequired(false);
        builder.Property(x => x.Pago).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.Excluido).IsRequired().HasDefaultValue(false);
    }
}
