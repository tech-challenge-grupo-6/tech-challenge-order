﻿using ControladorPedidos.Application.Produtos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControladorPedidos.Infrastructure.Database.Mappings;

public class ProdutoMapping : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produto");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Nome).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Preco).IsRequired();
        builder.Property(x => x.Descricao).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Imagem).IsRequired().HasMaxLength(500);
        builder.Property(x => x.CriadoEm).IsRequired();
        builder.Property(x => x.AtualizadoEm).IsRequired(false);
        builder.HasOne(x => x.Categoria).WithMany(x => x.Produtos).HasForeignKey(x => x.CategoriaId);
    }
}
