using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Produtos.Commands;
using ControladorPedidos.Application.Produtos.Handlers;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Notifications;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;
using NSubstitute.ExceptionExtensions;

namespace ControladorPedido.Tests.Application.Produtos.Handlers;

public class RemoverProdutoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingProduto_RemovesProdutoAndPublishesNotifications()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var produtoRepository = Substitute.For<IProdutoRepository>();
        var cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        var handler = new RemoverProdutoCommandHandler(mediator, produtoRepository, cacheConfiguration);

        var produtoId = Guid.NewGuid();
        var produto = new Produto() { Id = produtoId };

        produtoRepository.GetById(produtoId).Returns(produto);

        // Act
        var command = new RemoverProdutoCommand(produtoId);
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await produtoRepository.Received(1).Remove(produto);
        await mediator.Received(1).Publish(Arg.Any<CacheNotification>(), CancellationToken.None);
        await mediator.Received(1).Publish(Arg.Any<ProdutoRemovidoNotification>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_NonExistingProduto_ThrowsNotFoundException()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var produtoRepository = Substitute.For<IProdutoRepository>();
        var cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        var handler = new RemoverProdutoCommandHandler(mediator, produtoRepository, cacheConfiguration);

        var produtoId = Guid.NewGuid();

        produtoRepository.GetById(produtoId).Returns((Produto)null!);

        // Act
        var command = new RemoverProdutoCommand(produtoId);
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Produto não encontrado");
        await produtoRepository.DidNotReceive().Remove(Arg.Any<Produto>());
        await mediator.DidNotReceive().Publish(Arg.Any<CacheNotification>(), CancellationToken.None);
        await mediator.DidNotReceive().Publish(Arg.Any<ProdutoRemovidoNotification>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ExceptionOccurs_ThrowsAndPublishesExceptionNotification()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var produtoRepository = Substitute.For<IProdutoRepository>();
        var cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        var handler = new RemoverProdutoCommandHandler(mediator, produtoRepository, cacheConfiguration);

        var produtoId = Guid.NewGuid();
        var produto = new Produto { Id = produtoId };

        produtoRepository.GetById(produtoId).Returns(produto);
        produtoRepository.Remove(produto).Throws(new Exception("Some error"));

        // Act
        var command = new RemoverProdutoCommand(produtoId);
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Some error");
        await mediator.Received(1).Publish(Arg.Any<ExceptionNotification>(), CancellationToken.None);
    }
}
