using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Produtos.Commands;
using ControladorPedidos.Application.Produtos.Handlers;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;

namespace ControladorPedido.Tests.Application.Produtos.Handlers;

public class CadastrarProdutoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsProductId()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var produtoRepository = Substitute.For<IProdutoRepository>();
        var cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        var handler = new CadastrarProdutoCommandHandler(mediator, produtoRepository, cacheConfiguration);
        var command = new CadastrarProdutoCommand("Teste", Guid.NewGuid(), 20, "Teste", "test.png");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().BeOfType<string>();
    }

    [Fact]
    public async Task Handle_InvalidCommand_ThrowsException()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var produtoRepository = Substitute.For<IProdutoRepository>();
        var cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        var handler = new CadastrarProdutoCommandHandler(mediator, produtoRepository, cacheConfiguration);
        var command = new CadastrarProdutoCommand(null!, Guid.Empty, 20, "Teste", "test.png");
        var cancellationToken = CancellationToken.None;

        // Act
        Func<Task> act = async () => await handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<Exception>();
        await mediator.Received().Publish(Arg.Any<ExceptionNotification>(), cancellationToken);
    }
}
