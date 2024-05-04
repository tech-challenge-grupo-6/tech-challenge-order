using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Validators;

namespace ControladorPedido.Tests.Application.Produtos.Validators;

public class CategoriaValidadorTests
{
    [Fact]
    public void IsValid_WithValidCategoria_ReturnsTrue()
    {
        // Arrange
        var categoria = new Categoria
        {
            Nome = "Categoria 1"
        };

        // Act
        var result = CategoriaValidador.IsValid(categoria);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithEmptyNome_ThrowsArgumentException()
    {
        // Arrange
        var categoria = new Categoria
        {
            Nome = string.Empty
        };

        // Act
        var action = new Action(() => CategoriaValidador.IsValid(categoria));

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Nome não pode ser vazio");
    }
}
