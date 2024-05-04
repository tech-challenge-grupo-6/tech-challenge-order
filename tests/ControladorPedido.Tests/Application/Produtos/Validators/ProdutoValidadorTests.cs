using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Validators;

namespace ControladorPedido.Tests.Application.Produtos.Validators;

public class ProdutoValidadorTests
{
    [Fact]
    public void IsValid_WhenNomeIsNullOrEmpty_ThrowsArgumentException()
    {
        // Arrange
        var produto = new Produto { Nome = null! };

        // Act
        var action = new Action(() => ProdutoValidador.IsValid(produto));

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Nome do produto não pode ser vazio");
    }

    [Fact]
    public void IsValid_WhenCategoriaIdIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var produto = new Produto { Nome = "Test", CategoriaId = Guid.Empty };

        // Act
        var action = new Action(() => ProdutoValidador.IsValid(produto));

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Categoria do produto não pode ser vazio");
    }

    [Fact]
    public void IsValid_WhenPrecoIsZero_ThrowsArgumentException()
    {
        // Arrange
        var produto = new Produto { Nome = "Test", CategoriaId = Guid.NewGuid(), Preco = 0 };

        // Act
        var action = new Action(() => ProdutoValidador.IsValid(produto));

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Preço do produto não pode ser 0");
    }

    [Fact]
    public void IsValid_WhenDescricaoIsNullOrEmpty_ThrowsArgumentException()
    {
        // Arrange
        var produto = new Produto { Nome = "Test", CategoriaId = Guid.NewGuid(), Preco = 10, Descricao = null! };

        // Act
        var action = new Action(() => ProdutoValidador.IsValid(produto));

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Descrição do produto não pode ser vazio");
    }

    [Fact]
    public void IsValid_WhenImagemIsNullOrEmpty_ThrowsArgumentException()
    {
        // Arrange
        var produto = new Produto { Nome = "Test", CategoriaId = Guid.NewGuid(), Preco = 10, Descricao = "Test", Imagem = null! };

        // Act
        var action = new Action(() => ProdutoValidador.IsValid(produto));

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Imagem do produto não pode ser vazio");
    }

    [Fact]
    public void IsValid_WhenProdutoIsValid_ReturnsTrue()
    {
        // Arrange
        var produto = new Produto { Nome = "Test", CategoriaId = Guid.NewGuid(), Preco = 10, Descricao = "Test", Imagem = "Test" };

        // Act
        var result = ProdutoValidador.IsValid(produto);

        // Assert
        result.Should().BeTrue();
    }
}
