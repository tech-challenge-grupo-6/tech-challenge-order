using ControladorPedidos.Application.Produtos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControladorPedidos.Infrastructure.Database.Mappings;

public class CategoriaProdutoMapping : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("CategoriaProduto");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Nome).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CriadoEm).IsRequired();
        builder.Property(x => x.AtualizadoEm).IsRequired(false);
        builder.HasMany(x => x.Produtos).WithOne(x => x.Categoria).HasForeignKey(x => x.CategoriaId);
    }
}
