using System.Text;
using System.Text.Json;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Produtos.Handlers;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Queries;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute.ExceptionExtensions;

namespace ControladorPedido.Tests.Application.Produtos.Handlers;

public class PegarCategoriasQueryHandlerTests
{
    private readonly IMediator _mediator;
    private readonly IProdutoRepository _repository;
    private readonly CacheConfiguration _cacheConfiguration;
    private readonly IDistributedCache _cache;
    private readonly PegarCategoriasQueryHandler _handler;

    public PegarCategoriasQueryHandlerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _repository = Substitute.For<IProdutoRepository>();
        _cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        _cache = Substitute.For<IDistributedCache>();
        _handler = new PegarCategoriasQueryHandler(_mediator, _repository, _cacheConfiguration, _cache);
    }

    [Fact]
    public async Task Handle_WithCachedCategories_ReturnsCachedCategories()
    {
        // Arrange
        var cacheKey = _cacheConfiguration.CategoriaPrefix;
        var cacheValue = JsonSerializer.Serialize(new Dictionary<Guid, Categoria>());
        _cache.GetAsync(cacheKey, Arg.Any<CancellationToken>()).Returns(Encoding.UTF8.GetBytes(cacheValue));

        // Act
        var response = await _handler.Handle(new PegarCategoriasQuery(), CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Categorias.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithNoCachedCategories_ReturnsCategoriesFromRepository()
    {
        // Arrange
        var cacheKey = _cacheConfiguration.CategoriaPrefix;
        var cacheValue = string.Empty;
        _cache.GetAsync(cacheKey, Arg.Any<CancellationToken>()).Returns(Encoding.UTF8.GetBytes(cacheValue));

        List<Categoria> categories =
            [
                new() { Id = Guid.NewGuid(), Nome = "Category 1" },
                new() { Id = Guid.NewGuid(), Nome = "Category 2" }
            ];
        _repository.GetAllCategorias().Returns(categories);

        // Act
        var response = await _handler.Handle(new PegarCategoriasQuery(), CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Categorias.Should().HaveCount(2);
        response.Categorias.Select(c => c.Nome).Should().Contain(["Category 1", "Category 2"]);
    }

    [Fact]
    public async Task Handle_WithException_PublishesExceptionNotificationAndThrows()
    {
        // Arrange
        var exception = new Exception("Something went wrong");
        _repository.GetAllCategorias().Throws(exception);

        // Act
        Func<Task> act = async () => await _handler.Handle(new PegarCategoriasQuery(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>();
        await _mediator.Received(1).Publish(Arg.Is<ExceptionNotification>(n => n.Exception == exception), Arg.Any<CancellationToken>());
    }
}
