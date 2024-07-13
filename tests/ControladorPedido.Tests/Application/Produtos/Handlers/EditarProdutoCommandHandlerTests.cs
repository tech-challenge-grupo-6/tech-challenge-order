using System.Text.Json;
using System.Text.Json.Serialization;
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

public class EditarProdutoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsUnit()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var produtoRepository = Substitute.For<IProdutoRepository>();
        var cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var handler = new EditarProdutoCommandHandler(mediator, produtoRepository, cacheConfiguration, jsonSerializerOptions);
        var command = new EditarProdutoCommand(Guid.NewGuid(), "Teste", Guid.NewGuid(), 20, "Teste", "test.png");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ThrowsException()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var produtoRepository = Substitute.For<IProdutoRepository>();
        var cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var handler = new EditarProdutoCommandHandler(mediator, produtoRepository, cacheConfiguration, jsonSerializerOptions);
        var command = new EditarProdutoCommand(Guid.NewGuid(), null!, Guid.NewGuid(), 20, "Teste", "test.png");

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Handle_ValidCommand_PublishesCacheNotification()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var produtoRepository = Substitute.For<IProdutoRepository>();
        var cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var handler = new EditarProdutoCommandHandler(mediator, produtoRepository, cacheConfiguration, jsonSerializerOptions);
        var command = new EditarProdutoCommand(Guid.NewGuid(), "Teste", Guid.NewGuid(), 20, "Teste", "test.png");
        var produto = (Produto)command;
        produtoRepository.Update(Arg.Any<Produto>()).Returns(Task.CompletedTask);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var expectedKey = $"{cacheConfiguration.ProdutoPrefix}:{produto.Id}";
        var expectedValue = JsonSerializer.Serialize(produto);
        await mediator.Received(1).Publish(Arg.Any<CacheNotification>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ValidCommand_PublishesProdutoEditadoNotification()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var produtoRepository = Substitute.For<IProdutoRepository>();
        var cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var handler = new EditarProdutoCommandHandler(mediator, produtoRepository, cacheConfiguration, jsonSerializerOptions);
        var command = new EditarProdutoCommand(Guid.NewGuid(), "Teste", Guid.NewGuid(), 20, "Teste", "test.png");
        var produto = (Produto)command;
        produtoRepository.Update(Arg.Any<Produto>()).Returns(Task.CompletedTask);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await mediator.Received(1).Publish(Arg.Any<ProdutoEditadoNotification>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_PublishesExceptionNotification()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var produtoRepository = Substitute.For<IProdutoRepository>();
        var cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var handler = new EditarProdutoCommandHandler(mediator, produtoRepository, cacheConfiguration, jsonSerializerOptions);
        var command = new EditarProdutoCommand(Guid.NewGuid(), "Teste", Guid.NewGuid(), 20, "Teste", "test.png");
        var exception = new Exception();
        produtoRepository.Update(Arg.Any<Produto>()).Throws(exception);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>();
        await mediator.Received(1).Publish(Arg.Any<ExceptionNotification>(), CancellationToken.None);
    }
}
