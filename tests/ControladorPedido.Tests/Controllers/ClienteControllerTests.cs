using ControladorPedidos.Application.Clientes.Commands;
using ControladorPedidos.Application.Clientes.Queries;
using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ExceptionExtensions;

namespace ControladorPedido.Tests.Controllers;

public class ClienteControllerTests
{
    private readonly IMediator _mediator;
    private readonly ClienteController _controller;

    public ClienteControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new ClienteController(_mediator);
    }

    [Fact]
    public async Task BuscarPorCpf_ExistingCpf_ReturnsOk()
    {
        // Arrange
        string cpf = "12345678900";
        var queryResponse = new PegarClientePorCpfQueryResponse(Guid.NewGuid(), "John Doe", "12345678900", "test@test.com");
        _mediator.Send(Arg.Any<PegarClientePorCpfQuery>()).Returns(queryResponse);

        // Act
        var result = await _controller.BuscarPorCpf(cpf);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(queryResponse);
    }

    [Fact]
    public async Task BuscarPorCpf_InvalidCpf_ReturnsBadRequest()
    {
        // Arrange
        string cpf = "invalid-cpf";
        var exception = new ArgumentException("Invalid CPF");
        _mediator.Send(Arg.Any<PegarClientePorCpfQuery>()).Throws(exception);

        // Act
        var result = await _controller.BuscarPorCpf(cpf);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusCodeResult = (ObjectResult)result;
        statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task BuscarPorCpf_NonExistingCpf_ReturnsNotFound()
    {
        // Arrange
        string cpf = "non-existing-cpf";
        var exception = new NotFoundException("Cliente not found");
        _mediator.Send(Arg.Any<PegarClientePorCpfQuery>()).Throws(exception);

        // Act
        var result = await _controller.BuscarPorCpf(cpf);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be(exception.Message);
    }

    [Fact]
    public async Task BuscarPorCpf_InternalServerError_ReturnsInternalServerError()
    {
        // Arrange
        string cpf = "12345678900";
        var exception = new Exception("Internal server error");
        _mediator.Send(Arg.Any<PegarClientePorCpfQuery>()).Throws(exception);

        // Act
        var result = await _controller.BuscarPorCpf(cpf);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusCodeResult = (ObjectResult)result;
        statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task Post_ValidClienteDto_ReturnsCreated()
    {
        // Arrange
        string id = "123";
        _mediator.Send(Arg.Any<CadastroClienteCommand>()).Returns(id);

        // Act
        var result = await _controller.Post(Arg.Any<CadastroClienteCommand>());

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)result;
        createdAtActionResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdAtActionResult.ActionName.Should().Be(nameof(ClienteController.Post));
    }

    [Fact]
    public async Task Post_InvalidClienteDto_ReturnsInternalServerError()
    {
        // Arrange
        var exception = new ArgumentException("Invalid clienteDto");
        var command = Arg.Any<CadastroClienteCommand>();
        _mediator.Send(command).Throws(exception);

        // Act
        var result = await _controller.Post(command);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusCodeResult = (ObjectResult)result;
        statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task Post_InternalServerError_ReturnsInternalServerError()
    {
        // Arrange
        var exception = new Exception("Internal server error");
        var command = Arg.Any<CadastroClienteCommand>();
        _mediator.Send(command).Throws(exception);

        // Act
        var result = await _controller.Post(command);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusCodeResult = (ObjectResult)result;
        statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}
