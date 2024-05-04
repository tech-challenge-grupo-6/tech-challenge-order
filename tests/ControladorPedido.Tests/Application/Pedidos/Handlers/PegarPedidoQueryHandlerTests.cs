using System.Text;
using System.Text.Json;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Pedidos.Handlers;
using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Pedidos.Queries;
using ControladorPedidos.Application.Pedidos.Repositories;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute.ExceptionExtensions;

namespace ControladorPedido.Tests.Application.Pedidos.Handlers;

public class PegarPedidoQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingPedidoInCache_ReturnsPedido()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var repository = Substitute.For<IPedidoRepository>();
        var cacheConfiguration = new CacheConfiguration("Cliente", "Produto", "Categoria", "Pedido", "localhost:6379");
        var cache = Substitute.For<IDistributedCache>();

        var handler = new PegarPedidoQueryHandler(mediator, repository, cacheConfiguration, cache);
        var pedidoId = Guid.NewGuid();
        var query = new PegarPedidoQuery(pedidoId);
        var cachedPedido = new Pedido { Id = pedidoId, ClienteId = Guid.NewGuid(), Produtos = [new Produto { Id = Guid.NewGuid() }], Status = Status.Criado, ValorTotal = 100 };
        var cachedPedidoJson = JsonSerializer.Serialize(cachedPedido);

        cache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Encoding.UTF8.GetBytes(cachedPedidoJson));

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        await repository.DidNotReceive().GetById(Arg.Any<Guid>());
    }

    [Fact]
    public async Task Handle_NonExistingPedidoInCache_ReturnsPedidoFromRepository()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var repository = Substitute.For<IPedidoRepository>();
        var cacheConfiguration = new CacheConfiguration("Cliente", "Produto", "Categoria", "Pedido", "localhost:6379");
        var cache = Substitute.For<IDistributedCache>();

        var handler = new PegarPedidoQueryHandler(mediator, repository, cacheConfiguration, cache);

        var pedidoId = Guid.NewGuid();
        var query = new PegarPedidoQuery(pedidoId);
        var repositoryPedido = new Pedido { Id = pedidoId, ClienteId = Guid.NewGuid(), Produtos = [new Produto { Id = Guid.NewGuid() }], Status = Status.Criado, ValorTotal = 100 };

        repository.GetById(query.Id).Returns(repositoryPedido);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        await cache.ReceivedWithAnyArgs(1).GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExceptionThrown_PublishesExceptionNotificationAndThrows()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var repository = Substitute.For<IPedidoRepository>();
        var cacheConfiguration = new CacheConfiguration("Cliente", "Produto", "Categoria", "Pedido", "localhost:6379");
        var cache = Substitute.For<IDistributedCache>();

        var handler = new PegarPedidoQueryHandler(mediator, repository, cacheConfiguration, cache);

        var pedidoId = Guid.NewGuid();
        var query = new PegarPedidoQuery(pedidoId);
        var cancellationToken = CancellationToken.None;

        repository.GetById(query.Id).Throws(new ArgumentException("Pedido não encontrado"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(query, cancellationToken));
        await mediator.Received(1).Publish(Arg.Any<ExceptionNotification>(), cancellationToken);
    }
}
