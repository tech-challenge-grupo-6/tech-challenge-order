using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Gateways;
using ControladorPedidos.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ControladorPedido.Tests.Gateways;

public class ProdutoRepositoryTests
{
    private static DatabaseContext CreateUniqContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new DatabaseContext(options);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllProducts()
    {
        using var context = CreateUniqContext();
        var repository = new ProdutoRepository(context);

        Guid categoriaId = Guid.NewGuid();
        await repository.Add(new Categoria { Id = categoriaId, Nome = "Test" });
        await repository.Add(new Produto { Id = Guid.NewGuid(), CategoriaId = categoriaId, Descricao = "Test", Imagem = "Test", Nome = "Test", Preco = 1 });

        var result = await repository.GetAll();

        result.Should().HaveCount(context.Produtos.Count());
    }

    [Fact]
    public async Task GetIdsByCategoria_ShouldReturnCorrectIds()
    {
        using var context = CreateUniqContext();
        var repository = new ProdutoRepository(context);
        Guid categoriaId = Guid.NewGuid();
        await repository.Add(new Categoria { Id = categoriaId, Nome = "Test" });
        await repository.Add(new Produto { Id = Guid.NewGuid(), CategoriaId = categoriaId, Descricao = "Test", Imagem = "Test", Nome = "Test", Preco = 1 });
        var result = await repository.GetIdsByCategoria(categoriaId);

        result.Should().BeEquivalentTo(context.Produtos.Where(p => p.CategoriaId == categoriaId).Select(p => p.Id));
    }

    [Fact]
    public async Task GetById_ShouldReturnCorrectProduct()
    {
        using var context = CreateUniqContext();
        var repository = new ProdutoRepository(context);

        Guid categoriaId = Guid.NewGuid();
        var id = Guid.NewGuid();
        await repository.Add(new Categoria { Id = categoriaId, Nome = "Test" });
        await repository.Add(new Produto { Id = id, CategoriaId = categoriaId, Descricao = "Test", Imagem = "Test", Nome = "Test", Preco = 1 });

        var result = await repository.GetById(id);

        result.Should().BeEquivalentTo(context.Produtos.Find(id));
    }

    [Fact]
    public async Task Add_ShouldAddProduct()
    {
        using var context = CreateUniqContext();
        var repository = new ProdutoRepository(context);

        Guid categoriaId = Guid.NewGuid();
        await repository.Add(new Categoria { Id = categoriaId, Nome = "Test" });
        Produto produto = new() { Id = Guid.NewGuid(), CategoriaId = categoriaId, Descricao = "Test", Imagem = "Test", Nome = "Test", Preco = 1 };
        await repository.Add(produto);

        context.Produtos.Should().Contain(produto);
    }

    [Fact]
    public async Task Update_ShouldUpdateProduct()
    {
        using var context = CreateUniqContext();
        var repository = new ProdutoRepository(context);

        Guid categoriaId = Guid.NewGuid();
        await repository.Add(new Categoria { Id = categoriaId, Nome = "Test" });
        Produto produto = new() { Id = Guid.NewGuid(), CategoriaId = categoriaId, Descricao = "Test", Imagem = "Test", Nome = "Test", Preco = 1 };
        await repository.Add(produto);
        produto.Descricao = "Updated";
        await repository.Update(produto);

        Produto updated = context.Produtos.Find(produto.Id)!;
        updated.Descricao.Should().Be("Updated");
    }

    [Fact]
    public async Task Remove_ShouldRemoveProduct()
    {
        using var context = CreateUniqContext();
        var repository = new ProdutoRepository(context);

        Guid categoriaId = Guid.NewGuid();
        await repository.Add(new Categoria { Id = categoriaId, Nome = "Test" });
        Produto produto = new() { Id = Guid.NewGuid(), CategoriaId = categoriaId, Descricao = "Test", Imagem = "Test", Nome = "Test", Preco = 1 };
        await repository.Add(produto);
        await repository.Remove(produto);

        context.Produtos.Should().NotContain(produto);
    }

    [Fact]
    public async Task GetAllCategorias_ShouldReturnAllCategories()
    {
        using var context = CreateUniqContext();
        var repository = new ProdutoRepository(context);

        await repository.Add(new Categoria { Id = Guid.NewGuid(), Nome = "Test1" });
        await repository.Add(new Categoria { Id = Guid.NewGuid(), Nome = "Test2" });

        var result = await repository.GetAllCategorias();

        result.Should().HaveCount(context.CategoriaProdutos.Count());
    }

    [Fact]
    public async Task GetCategoriaById_ShouldReturnCorrectCategory()
    {
        using var context = CreateUniqContext();
        var repository = new ProdutoRepository(context);

        Guid categoriaId = Guid.NewGuid();
        await repository.Add(new Categoria { Id = categoriaId, Nome = "Test" });
        var result = await repository.GetCategoriaById(categoriaId);

        result.Should().BeEquivalentTo(context.CategoriaProdutos.Find(categoriaId));
    }
}
