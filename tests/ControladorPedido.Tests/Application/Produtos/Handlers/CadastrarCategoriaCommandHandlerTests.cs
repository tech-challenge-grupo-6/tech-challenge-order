using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Produtos.Commands;
using ControladorPedidos.Application.Produtos.Handlers;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Notifications;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace ControladorPedido.Tests.Application.Produtos.Handlers;

public class CadastrarCategoriaCommandHandlerTests
{
    private readonly IMediator _mediator;
    private readonly IProdutoRepository _repository;
    private readonly CacheConfiguration _cacheConfiguration;
    private readonly IDistributedCache _cache;
    private readonly CadastrarCategoriaCommandHandler _handler;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public CadastrarCategoriaCommandHandlerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _repository = Substitute.For<IProdutoRepository>();
        _cacheConfiguration = new CacheConfiguration("Client", "Produto", "Categoria", "Pedido", "localhost:6379");
        _cache = Substitute.For<IDistributedCache>();
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _handler = new CadastrarCategoriaCommandHandler(_mediator, _repository, _cacheConfiguration, _cache, _jsonSerializerOptions);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCategoryId()
    {
        // Arrange
        var command = new CadastrarCategoriaCommand("Teste");
        var cancellationToken = CancellationToken.None;
        var categoria = new Categoria { Id = Guid.NewGuid() };
        var cacheCategorias = new Dictionary<Guid, Categoria> { { categoria.Id, categoria } };
        var cacheCategoriasJson = JsonSerializer.Serialize(cacheCategorias);

        _repository.Add(categoria).ReturnsForAnyArgs(x =>
        {
            Categoria c = x.Arg<Categoria>();
            c.Id = categoria.Id;
            return Task.CompletedTask;
        });
        _cache.GetAsync(Arg.Any<string>(), cancellationToken)!.Returns(Encoding.UTF8.GetBytes(cacheCategoriasJson, 0, cacheCategoriasJson.Length));

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(categoria.Id.ToString());
        await _mediator.Received(1).Publish(Arg.Any<CategoriaCriadaNotification>(), cancellationToken);
        await _mediator.Received(1).Publish(Arg.Any<CacheNotification>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ThrowsException()
    {
        // Arrange
        var command = new CadastrarCategoriaCommand(null!);
        var cancellationToken = CancellationToken.None;
        var exception = new Exception();

        // Act
        Func<Task> act = async () => await _handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        await _mediator.Received(1).Publish(Arg.Any<ExceptionNotification>(), cancellationToken);
    }
}
