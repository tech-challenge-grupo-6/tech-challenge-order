using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Pedidos.Validators;

namespace ControladorPedido.Tests.Application.Pedidos.Validators;

public class PedidoValidadorTests
{
    [Fact]
    public void IsValid_WithEmptyClienteId_ThrowsArgumentException()
    {
        // Arrange
        var pedido = new Pedido
        {
            ClienteId = Guid.Empty,
            Produtos = []
        };

        // Act
        Action act = () => PedidoValidador.IsValid(pedido);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Id do pedido não pode ser vazio");
    }

    [Fact]
    public void IsValid_WithEmptyProdutos_ThrowsArgumentException()
    {
        // Arrange
        var pedido = new Pedido
        {
            ClienteId = Guid.NewGuid(),
            Produtos = []
        };

        // Act
        Action act = () => PedidoValidador.IsValid(pedido);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Lista de produtos do pedido não pode ser vazia");
    }

    [Fact]
    public void IsValid_WithValidPedido_ReturnsTrue()
    {
        // Arrange
        var pedido = new Pedido
        {
            ClienteId = Guid.NewGuid(),
            Produtos = [new()]
        };

        // Act
        var result = PedidoValidador.IsValid(pedido);

        // Assert
        result.Should().BeTrue();
    }
}
