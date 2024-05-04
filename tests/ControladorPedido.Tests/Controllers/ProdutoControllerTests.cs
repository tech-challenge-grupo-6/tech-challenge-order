using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Produtos.Commands;
using ControladorPedidos.Application.Produtos.Queries;
using ControladorPedidos.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ExceptionExtensions;

namespace ControladorPedido.Tests.Controllers;

public class ProdutoControllerTests
{
    private readonly IMediator _mediator;
    private readonly ProdutoController _controller;

    public ProdutoControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new ProdutoController(_mediator);
    }

    [Fact]
    public async Task ListarPorCategoria_WithValidCategoriaId_ReturnsOkResult()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        List<PegarProdutoPorCategoriaQuery> response = [];
        _mediator.Send(Arg.Any<PegarProdutosPorCategoriaQuery>()).Returns(response);

        // Act
        var result = await _controller.ListarPorCategoria(categoriaId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ListarPorCategoria_WithNotFoundException_ReturnsNotFoundResult()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();

        _mediator.Send(Arg.Any<PegarProdutosPorCategoriaQuery>()).Throws(new NotFoundException("Categoria não encontrada"));

        // Act
        var result = await _controller.ListarPorCategoria(categoriaId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ListarPorCategoria_WithException_ReturnsInternalServerErrorResult()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();

        _mediator.Send(Arg.Any<PegarProdutosPorCategoriaQuery>()).Throws(new Exception("Erro interno"));

        // Act
        var result = await _controller.ListarPorCategoria(categoriaId);

        // Assert
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task Get_WhenCategoriasExist_ReturnsOkResultWithCategorias()
    {
        // Arrange
        var expectedCategorias = new PegarCategoriasQueryResponse([]);

        _mediator.Send(Arg.Any<PegarCategoriasQuery>()).Returns(expectedCategorias);

        // Act
        var result = await _controller.Get();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(expectedCategorias);
    }

    [Fact]
    public async Task Get_WhenCategoriasNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        _mediator.Send(Arg.Any<PegarCategoriasQuery>()).Throws(new NotFoundException("Categorias não encontradas"));

        // Act
        var result = await _controller.Get();

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().Be("Categorias não encontradas");
    }

    [Fact]
    public async Task Get_WhenExceptionThrown_ReturnsInternalServerErrorResult()
    {
        // Arrange
        _mediator.Send(Arg.Any<PegarCategoriasQuery>()).Throws(new Exception("Internal Server Error"));

        // Act
        var result = await _controller.Get();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        objectResult.Value.Should().Be("Erro interno");
    }

    [Fact]
    public async Task Post_WithValidData_ReturnsCreated()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var controller = new ProdutoController(mediator);
        var produtoDto = Arg.Any<CadastrarProdutoCommand>();

        mediator.Send(produtoDto).Returns("123"); // Set up the mock behavior

        // Act
        var result = await controller.Post(produtoDto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)result;
        createdAtActionResult.ActionName.Should().Be(nameof(ProdutoController.Post));
    }

    [Fact]
    public async Task Post_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var controller = new ProdutoController(mediator);
        var produtoDto = Arg.Any<CadastrarProdutoCommand>();

        mediator.Send(produtoDto).Throws(new ArgumentException("Invalid data")); // Set up the mock behavior

        // Act
        var result = await controller.Post(produtoDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var badRequestObjectResult = (ObjectResult)result;
        badRequestObjectResult.Value.Should().Be("Invalid data");
    }

    [Fact]
    public async Task Post_WithException_ReturnsInternalServerError()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var controller = new ProdutoController(mediator);
        var produtoDto = Arg.Any<CadastrarProdutoCommand>();

        mediator.Send(produtoDto).Throws(new Exception("Internal error")); // Set up the mock behavior

        // Act
        var result = await controller.Post(produtoDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        objectResult.Value.Should().Be("Erro interno");
    }

    [Fact]
    public async Task PostCategora_WithValidData_ReturnsCreated()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var controller = new ProdutoController(mediator);
        var categoriaDto = Arg.Any<CadastrarCategoriaCommand>();

        mediator.Send(categoriaDto).Returns("123"); // Set up the mock behavior

        // Act
        var result = await controller.Post(categoriaDto);

        // Assert
        result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PostCategoria_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var controller = new ProdutoController(mediator);
        var categoriaDto = Arg.Any<CadastrarCategoriaCommand>();

        mediator.Send(categoriaDto).Throws(new ArgumentException("Invalid data")); // Set up the mock behavior

        // Act
        var result = await controller.Post(categoriaDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var badRequestObjectResult = (ObjectResult)result;
        badRequestObjectResult.Value.Should().Be("Invalid data");
    }

    [Fact]
    public async Task PostCategoria_WithException_ReturnsInternalServerError()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var controller = new ProdutoController(mediator);
        var categoriaDto = Arg.Any<CadastrarCategoriaCommand>();

        mediator.Send(categoriaDto).Throws(new Exception("Internal server error")); // Set up the mock behavior

        // Act
        var result = await controller.Post(categoriaDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        objectResult.Value.Should().Be("Erro interno");
    }

    [Fact]
    public async Task Put_WithValidIdAndProdutoDto_ReturnsNoContent()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var controller = new ProdutoController(mediator);
        var id = Guid.NewGuid();
        var produtoDto = Arg.Any<EditarProdutoCommand>();

        // Act
        var result = await controller.Put(id, produtoDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Put_WithNotFound_ReturnsNotFound()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var controller = new ProdutoController(mediator);
        var id = Guid.NewGuid();
        var produtoDto = Arg.Any<EditarProdutoCommand>();
        mediator.Send(produtoDto).Throws(new NotFoundException("Produto não encontrado"));

        // Act
        var result = await controller.Put(id, produtoDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be("Produto não encontrado");
    }

    [Fact]
    public async Task Put_WithInvalidArgument_ReturnsBadRequest()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var controller = new ProdutoController(mediator);
        var id = Guid.NewGuid();
        var produtoDto = Arg.Any<EditarProdutoCommand>();
        mediator.Send(produtoDto).Throws(new ArgumentException("Invalid argument"));

        // Act
        var result = await controller.Put(id, produtoDto);

        // Assert
        result.Should().BeOfType<ObjectResult>()
            .Which.Value.Should().Be("Invalid argument");
    }

    [Fact]
    public async Task Delete_ExistingProductId_ReturnsNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mediator.Send(Arg.Any<RemoverProdutoCommand>()).Returns(Unit.Value);

        // Act
        var result = await _controller.Delete(productId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_NonExistingProductId_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mediator.Send(Arg.Any<RemoverProdutoCommand>()).Throws(new NotFoundException("Produto não encontrado"));

        // Act
        var result = await _controller.Delete(productId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be("Produto não encontrado");
    }

    [Fact]
    public async Task Delete_InvalidProductId_ReturnsBadRequest()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mediator.Send(Arg.Any<RemoverProdutoCommand>()).Throws(new ArgumentException("ID de produto inválido"));

        // Act
        var result = await _controller.Delete(productId);

        // Assert
        result.Should().BeOfType<ObjectResult>()
            .Which.Value.Should().Be("ID de produto inválido");
    }
}
