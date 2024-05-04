using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Gateways;
using ControladorPedidos.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ControladorPedido.Tests.Gateways;

public class PedidoRepositoryTests
{
    private static DatabaseContext CreateUniqContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new DatabaseContext(options);
    }

    private static async Task<Cliente> AddCliente(DatabaseContext context)
    {
        var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "Test", Cpf = "123123123", Email = "test@test.com" };
        await context.Clientes.AddAsync(cliente);
        await context.SaveChangesAsync();
        return cliente;
    }

    private static async Task<Produto> AddProduto(DatabaseContext context)
    {
        var categoria = new Categoria { Id = Guid.NewGuid(), Nome = "Test" };
        var produto = new Produto { Id = Guid.NewGuid(), CategoriaId = categoria.Id, Descricao = "Test", Imagem = "Test", Nome = "Test", Preco = 1 };
        await context.CategoriaProdutos.AddAsync(categoria);
        await context.Produtos.AddAsync(produto);
        await context.SaveChangesAsync();
        return produto;
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllPedidos()
    {
        using var context = CreateUniqContext();
        var repository = new PedidoRepository(context);

        Cliente cliente = await AddCliente(context);
        Produto produto = await AddProduto(context);
        await repository.Add(new Pedido { ClienteId = cliente.Id, Produtos = [produto] });

        var result = await repository.GetAll();

        result.Should().HaveCount(context.Pedidos.Count());
    }

    [Fact]
    public async Task GetByStatus_ShouldReturnCorrectPedidos()
    {
        using var context = CreateUniqContext();
        var repository = new PedidoRepository(context);

        Cliente cliente = await AddCliente(context);
        Produto produto = await AddProduto(context);
        var status = Status.Criado;
        await repository.Add(new Pedido { ClienteId = cliente.Id, Produtos = [produto], Status = status });

        var result = await repository.GetByStatus(status);

        result.Should().BeEquivalentTo(context.Pedidos.Where(p => p.Status == status));
    }

    [Fact]
    public async Task GetByCliente_ShouldReturnCorrectPedidos()
    {
        using var context = CreateUniqContext();
        var repository = new PedidoRepository(context);

        Cliente cliente = await AddCliente(context);
        Produto produto = await AddProduto(context);
        await repository.Add(new Pedido { ClienteId = cliente.Id, Produtos = [produto] });

        var result = await repository.GetByCliente(cliente.Id);

        result.Should().BeEquivalentTo(context.Pedidos.Where(p => p.ClienteId == cliente.Id));
    }

    [Fact]
    public async Task GetById_ShouldReturnCorrectPedido()
    {
        using var context = CreateUniqContext();
        var repository = new PedidoRepository(context);

        var id = Guid.NewGuid();
        Cliente cliente = await AddCliente(context);
        Produto produto = await AddProduto(context);
        await repository.Add(new Pedido { ClienteId = cliente.Id, Produtos = [produto], Id = id });
        var result = await repository.GetById(id);

        result.Should().BeEquivalentTo(context.Pedidos.Find(id));
    }

    [Fact]
    public async Task Add_ShouldAddPedido()
    {
        using var context = CreateUniqContext();
        var repository = new PedidoRepository(context);

        Cliente cliente = await AddCliente(context);
        Produto produto = await AddProduto(context);
        Pedido pedido = new() { ClienteId = cliente.Id, Produtos = [produto], Id = Guid.NewGuid() };
        await repository.Add(pedido);

        context.Pedidos.Should().Contain(pedido);
    }

    [Fact]
    public async Task UpdateStatus_ShouldUpdatePedidoStatus()
    {
        using var context = CreateUniqContext();
        var repository = new PedidoRepository(context);

        Cliente cliente = await AddCliente(context);
        Produto produto = await AddProduto(context);
        Pedido pedido = new() { ClienteId = cliente.Id, Produtos = [produto], Id = Guid.NewGuid(), Status = Status.Criado };
        await repository.Add(pedido);

        pedido.Status = Status.EmProgresso;
        await repository.UpdateStatus(pedido);

        context.Pedidos.Find(pedido.Id)!.Status.Should().Be(Status.EmProgresso);
    }
}
