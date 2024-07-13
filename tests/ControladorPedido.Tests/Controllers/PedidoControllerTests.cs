using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Pedidos.Commands;
using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Pedidos.Queries;
using ControladorPedidos.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ExceptionExtensions;

namespace ControladorPedido.Tests.Controllers;

public class PedidoControllerTests
{
    private readonly IMediator _mediator;
    private readonly PedidoController _pedidoController;

    public PedidoControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _pedidoController = new PedidoController(_mediator);
    }

    [Fact]
    public async Task Get_ExistingId_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var query = new PegarPedidoQuery(id);
        var response = new PegarPedidoQueryResponse(id, Guid.NewGuid(), Status.Criado, [Guid.NewGuid()], 10, false);
        _mediator.Send(query).Returns(response);

        // Act
        var result = await _pedidoController.Get(id);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Get_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var query = new PegarPedidoQuery(id);
        _mediator.Send(query).Throws<NotFoundException>();

        // Act
        var result = await _pedidoController.Get(id);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Get_InternalServerError_ReturnsStatusCode500()
    {
        // Arrange
        var id = Guid.NewGuid();
        var query = new PegarPedidoQuery(id);
        _mediator.Send(query).Throws<Exception>();

        // Act
        var result = await _pedidoController.Get(id);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        objectResult.Value.Should().Be("Erro interno");
    }

    [Fact]
    public async Task Post_ValidPedidoDto_ReturnsCreated()
    {
        // Arrange
        var pedidoDto = Arg.Any<CadastrarPedidoCommand>();
        var id = Guid.NewGuid();
        _mediator.Send(pedidoDto).Returns(id);

        // Act
        var result = await _pedidoController.Post(pedidoDto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = result as CreatedAtActionResult;
        createdAtActionResult!.ActionName.Should().Be(nameof(PedidoController.Post));
    }

    [Fact]
    public async Task Post_InvalidPedidoDto_ReturnsBadRequest()
    {
        // Arrange
        var pedidoDto = Arg.Any<CadastrarPedidoCommand>();
        _mediator.Send(pedidoDto).Throws<ArgumentException>();

        // Act
        var result = await _pedidoController.Post(pedidoDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Post_InternalServerError_ReturnsStatusCode500()
    {
        // Arrange
        var pedidoDto = Arg.Any<CadastrarPedidoCommand>();
        _mediator.Send(pedidoDto).Throws<Exception>();

        // Act
        var result = await _pedidoController.Post(pedidoDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        objectResult.Value.Should().Be("Erro interno");
    }
}
