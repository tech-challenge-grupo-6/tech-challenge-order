using System.Text.Json;
using ControladorPedidos.Application.Clientes.Commands;
using ControladorPedidos.Application.Clientes.Handlers;
using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Application.Clientes.Notifications;
using ControladorPedidos.Application.Clientes.Repositories;
using ControladorPedidos.Application.Clientes.Validators;
using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;

namespace ControladorPedido.Tests.Application.Clientes.Handlers;

public class CastrarClienteCommandHandlerTests
{
    private readonly IMediator _mediator;
    private readonly IClienteRepository _repository;
    private readonly CacheConfiguration _cacheConfiguration;
    private readonly CastrarClienteCommandHandler _handler;

    public CastrarClienteCommandHandlerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _repository = Substitute.For<IClienteRepository>();
        _cacheConfiguration = new CacheConfiguration("Cliente", "Produto", "Categoria", "Pedido", "localhost:6379");
        _handler = new CastrarClienteCommandHandler(_mediator, _repository, _cacheConfiguration);
    }

    [Fact]
    public async Task Handle_WithValidCliente_ShouldAddClienteAndPublishNotifications()
    {
        // Arrange
        var command = new CadastroClienteCommand("Test", "32131171617", "test@test.com");
        var cliente = (Cliente)command;
        cliente.Id = Guid.NewGuid();
        var clienteCriadoNotification = (ClienteCriadoNotification)cliente;
        _repository.Add(Arg.Any<Cliente>()).ReturnsForAnyArgs(x =>
        {
            var c = x.Arg<Cliente>();
            c.Id = cliente.Id;
            return Task.CompletedTask;
        });
        _mediator.Publish(Arg.Any<ClienteCriadoNotification>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _mediator.Publish(Arg.Any<CacheNotification>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(cliente.Id.ToString());
        await _repository.Received(1).Add(Arg.Any<Cliente>());
        await _mediator.Received(1).Publish(Arg.Is(clienteCriadoNotification), Arg.Any<CancellationToken>());
        var cacheNotification = new CacheNotification($"{_cacheConfiguration.ClientePrefix}:{cliente.Cpf}", JsonSerializer.Serialize(cliente));
        await _mediator.Received(1).Publish(Arg.Is(cacheNotification), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidCliente_ShouldThrowArgumentExceptionNomeVazioAndPublishExceptionNotification()
    {
        // Arrange
        var command = new CadastroClienteCommand(null!, "32131171617", "test@test.com");
        _mediator.Publish(Arg.Any<ExceptionNotification>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Nome não pode ser vazio");
        await _repository.DidNotReceive().Add(Arg.Any<Cliente>());
        await _mediator.Received(1).Publish(Arg.Any<ExceptionNotification>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidCliente_ShouldThrowArgumentCpfInvalidoAndPublishExceptionNotification()
    {
        // Arrange
        var command = new CadastroClienteCommand("Test", "123", "test@test.com");
        _mediator.Publish(Arg.Any<ExceptionNotification>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Cpf inválido");
        await _repository.DidNotReceive().Add(Arg.Any<Cliente>());
        await _mediator.Received(1).Publish(Arg.Any<ExceptionNotification>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidCliente_ShouldThrowArgumentEmailInvalidoAndPublishExceptionNotification()
    {
        // Arrange
        var command = new CadastroClienteCommand("Test", "32131171617", "testtest");
        _mediator.Publish(Arg.Any<ExceptionNotification>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Email inválido");
        await _repository.DidNotReceive().Add(Arg.Any<Cliente>());
        await _mediator.Received(1).Publish(Arg.Any<ExceptionNotification>(), Arg.Any<CancellationToken>());
    }
}
