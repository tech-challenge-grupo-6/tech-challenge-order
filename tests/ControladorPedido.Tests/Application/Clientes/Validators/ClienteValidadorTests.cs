using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Application.Clientes.Validators;

namespace ControladorPedido.Tests.Application.Clientes.Validators;

public class ClienteValidadorTests
{
    [Fact]
    public void IsValid_WithValidCliente_ShouldReturnTrue()
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "John Doe",
            Email = "johndoe@example.com",
            Cpf = "32131171617"
        };

        // Act
        var result = ClienteValidador.IsValid(cliente);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void IsValid_WithNullOrEmptyNome_ShouldThrowArgumentException(string nome)
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = nome,
            Email = "johndoe@example.com",
            Cpf = "32131171617"
        };

        // Act
        var action = new Action(() => ClienteValidador.IsValid(cliente));

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Nome não pode ser vazio");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalidemail")]
    public void IsValid_WithInvalidEmail_ShouldThrowArgumentException(string email)
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "John Doe",
            Email = email,
            Cpf = "32131171617"
        };

        // Act
        var action = new Action(() => ClienteValidador.IsValid(cliente));

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Email inválido");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    public void IsValid_WithInvalidCpf_ShouldThrowArgumentException(string cpf)
    {
        // Arrange
        var cliente = new Cliente
        {
            Nome = "John Doe",
            Email = "johndoe@example.com",
            Cpf = cpf
        };

        // Act
        var action = new Action(() => ClienteValidador.IsValid(cliente));

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Cpf inválido");
    }
}
