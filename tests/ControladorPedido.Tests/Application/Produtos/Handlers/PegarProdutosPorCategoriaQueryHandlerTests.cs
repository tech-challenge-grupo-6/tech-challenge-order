using System.Text;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Produtos.Handlers;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Queries;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute.ExceptionExtensions;

namespace ControladorPedido.Tests.Application.Produtos.Handlers;

public class PegarProdutosPorCategoriaQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithValidRequest_ReturnsListOfProducts()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var repository = Substitute.For<IProdutoRepository>();
        var cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        var cache = Substitute.For<IDistributedCache>();

        var handler = new PegarProdutosPorCategoriaQueryHandler(mediator, repository, cacheConfiguration, cache);

        var request = new PegarProdutosPorCategoriaQuery(Guid.NewGuid());
        var cancellationToken = CancellationToken.None;

        var idsByCategoria = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        repository.GetIdsByCategoria(request.CategoriaId).Returns(idsByCategoria);

        var produto1 = new Produto { Id = idsByCategoria[0], Nome = "Produto 1" };
        var produto2 = new Produto { Id = idsByCategoria[1], Nome = "Produto 2" };
        repository.GetById(idsByCategoria[0]).Returns(produto1);
        repository.GetById(idsByCategoria[1]).Returns(produto2);

        cache.GetAsync(Arg.Any<string>(), cancellationToken).Returns(Encoding.UTF8.GetBytes(string.Empty));

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        await mediator.Received().Publish(Arg.Any<CacheNotification>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_WithException_ThrowsExceptionAndPublishesNotification()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var repository = Substitute.For<IProdutoRepository>();
        var cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        var cache = Substitute.For<IDistributedCache>();

        var handler = new PegarProdutosPorCategoriaQueryHandler(mediator, repository, cacheConfiguration, cache);

        var request = new PegarProdutosPorCategoriaQuery(Guid.NewGuid());
        var cancellationToken = CancellationToken.None;

        repository.GetIdsByCategoria(request.CategoriaId).Throws(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(request, cancellationToken));

        await mediator.Received().Publish(Arg.Any<ExceptionNotification>(), cancellationToken);
    }
}
