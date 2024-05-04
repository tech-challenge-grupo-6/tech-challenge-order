using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Application.Clientes.Repositories;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Pedidos.Commands;
using ControladorPedidos.Application.Pedidos.Handlers;
using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Pedidos.Notifications;
using ControladorPedidos.Application.Pedidos.Repositories;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;

namespace ControladorPedido.Tests.Application.Pedidos.Handlers;

public class CadastrarPedidoCommandHandlerTests
{
    private readonly IMediator _mediator;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly CacheConfiguration _cacheConfiguration;
    private readonly CadastrarPedidoCommandHandler _handler;

    public CadastrarPedidoCommandHandlerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _pedidoRepository = Substitute.For<IPedidoRepository>();
        _clienteRepository = Substitute.For<IClienteRepository>();
        _produtoRepository = Substitute.For<IProdutoRepository>();
        _cacheConfiguration = new CacheConfiguration("Cliente", "Produto", "Categoria", "Pedido", "localhost:6379");

        _handler = new CadastrarPedidoCommandHandler(
            _mediator,
            _pedidoRepository,
            _clienteRepository,
            _produtoRepository,
            _cacheConfiguration
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsPedidoId()
    {
        // Arrange
        var command = new CadastrarPedidoCommand(Guid.NewGuid(), [Guid.NewGuid(), Guid.NewGuid()]);

        var cliente = new Cliente();
        _clienteRepository.GetById(command.ClienteId).Returns(cliente);

        List<Produto> produtos = [new Produto(), new Produto()];
        _produtoRepository.GetById(Arg.Any<Guid>()).Returns(produtos[0], produtos[1]);

        var pedido = new Pedido();
        _pedidoRepository.Add(Arg.Any<Pedido>()).ReturnsForAnyArgs(x =>
        {
            pedido.Id = x.Arg<Pedido>().Id;
            return Task.CompletedTask;
        });

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(pedido.Id);
        await _mediator.Received(1).Publish(Arg.Any<PedidoCadastradoNotification>(), cancellationToken);
        await _mediator.Received(1).Publish(Arg.Any<CacheNotification>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ThrowsException()
    {
        // Arrange
        var command = new CadastrarPedidoCommand(new Guid(), []);

        var cancellationToken = CancellationToken.None;

        // Act
        Func<Task> act = async () => await _handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        await _mediator.Received(1).Publish(Arg.Any<ExceptionNotification>(), cancellationToken);
    }
}
