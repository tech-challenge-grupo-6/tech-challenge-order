using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Gateways;
using ControladorPedidos.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ControladorPedido.Tests.Gateways;

public class ClienteRepositoryTests
{
    private static DatabaseContext CreateUniqContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new DatabaseContext(options);
    }

    [Fact]
    public async Task GetById_ShouldReturnClient_WhenClientExists()
    {
        using var context = CreateUniqContext();
        var clienteRepository = new ClienteRepository(context);
        var expectedClient = new Cliente { Id = Guid.NewGuid(), Cpf = "12345678901", Email = "test@test.com", Nome = "Test" };
        context.Clientes.Add(expectedClient);
        await context.SaveChangesAsync();

        var result = await clienteRepository.GetById(expectedClient.Id);

        result.Should().BeEquivalentTo(expectedClient);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenClientDoesNotExist()
    {
        using var context = CreateUniqContext();
        var clienteRepository = new ClienteRepository(context);
        var result = await clienteRepository.GetById(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCpf_ShouldReturnClient_WhenClientExists()
    {
        using var context = CreateUniqContext();
        var clienteRepository = new ClienteRepository(context);
        var expectedClient = new Cliente { Cpf = "123123123", Email = "test@test.com", Nome = "Test" };
        context.Clientes.Add(expectedClient);
        await context.SaveChangesAsync();

        var result = await clienteRepository.GetByCpf(expectedClient.Cpf);

        result!.Cpf.Should().Be(expectedClient.Cpf);
        result!.Email.Should().Be(expectedClient.Email);
        result!.Nome.Should().Be(expectedClient.Nome);
    }

    [Fact]
    public async Task GetByCpf_ShouldReturnNull_WhenClientDoesNotExist()
    {
        using var context = CreateUniqContext();
        var clienteRepository = new ClienteRepository(context);
        var result = await clienteRepository.GetByCpf("987654321");

        result.Should().BeNull();
    }

    [Fact]
    public async Task Add_ShouldAddClientToDatabase()
    {
        using var context = CreateUniqContext();
        var clienteRepository = new ClienteRepository(context);
        var newClient = new Cliente { Id = Guid.NewGuid(), Cpf = "12345678901", Email = "test@test.com", Nome = "Test" };

        await clienteRepository.Add(newClient);

        context.Clientes.Should().ContainEquivalentOf(newClient);
    }

    [Fact]
    public async Task Update_ShouldUpdateClientInDatabase()
    {
        using var context = CreateUniqContext();
        var clienteRepository = new ClienteRepository(context);
        var client = new Cliente { Id = Guid.NewGuid(), Cpf = "12345678901", Email = "test@test.com", Nome = "Test" };
        context.Clientes.Add(client);
        await context.SaveChangesAsync();

        client.Cpf = "09876543210";
        await clienteRepository.Update(client);

        var updatedClient = context.Clientes.Single(c => c.Id == client.Id);
        updatedClient.Cpf.Should().Be("09876543210");
    }
}
