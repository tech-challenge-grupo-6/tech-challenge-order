﻿using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ControladorPedidos.Gateways;

public class ProdutoRepository(DatabaseContext dbContext) : IProdutoRepository
{
    public async Task<IEnumerable<Produto>> GetAll()
    {
        return await dbContext.Produtos.ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetIdsByCategoria(Guid categoriaId)
    {
        return await dbContext.Produtos
            .Where(p => p.CategoriaId == categoriaId)
            .Select(p => p.Id).ToListAsync();
    }

    public async Task<Produto?> GetById(Guid id)
    {
        return await dbContext.Produtos.FindAsync(id);
    }

    public async Task Add(Produto Produto)
    {
        dbContext.Produtos.Add(Produto);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(Produto Produto)
    {
        dbContext.Produtos.Update(Produto);
        await dbContext.SaveChangesAsync();
    }

    public async Task Add(Categoria Categoria)
    {
        dbContext.CategoriaProdutos.Add(Categoria);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(Categoria Categoria)
    {
        dbContext.CategoriaProdutos.Update(Categoria);
        await dbContext.SaveChangesAsync();
    }

    public async Task Remove(Produto produto)
    {
        dbContext.Produtos.Remove(produto);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Categoria>> GetAllCategorias()
    {
        return await dbContext.CategoriaProdutos.ToListAsync();
    }

    public async Task<Categoria?> GetCategoriaById(Guid id)
    {
        return await dbContext.CategoriaProdutos.FindAsync(id);
    }
}
