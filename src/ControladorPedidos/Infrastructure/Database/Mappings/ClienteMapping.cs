using ControladorPedidos.Application.Clientes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControladorPedidos.Infrastructure.Database.Mappings;

public class ClienteMapping : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Cliente");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Nome).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Cpf).IsRequired().HasMaxLength(11);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CriadoEm).IsRequired();
        builder.Property(x => x.AtualizadoEm).IsRequired(false);
        builder.HasMany(x => x.Pedidos).WithOne(x => x.Cliente).HasForeignKey(x => x.ClienteId);
        builder.Property(x => x.Excluido).IsRequired().HasDefaultValue(false);
    }
}
