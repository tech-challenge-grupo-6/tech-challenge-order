using System.Text.Json;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Produtos.Commands;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Notifications;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Application.Produtos.Validators;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;

namespace ControladorPedidos.Application.Produtos.Handlers;

public class EditarProdutoCommandHandler(
    IMediator mediator,
    IProdutoRepository produtoRepository,
    CacheConfiguration cacheConfiguration,
    JsonSerializerOptions jsonSerializerOptions) : IRequestHandler<EditarProdutoCommand, Unit>
{
    public async Task<Unit> Handle(EditarProdutoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Produto produto = (Produto)request;
            ProdutoValidador.IsValid(produto);

            await produtoRepository.Update(produto);
            string key = $"{cacheConfiguration.ProdutoPrefix}:{produto.Id}";
            string cacheValue = JsonSerializer.Serialize(produto, jsonSerializerOptions);
            await mediator.Publish(new CacheNotification(key, cacheValue), cancellationToken);
            await mediator.Publish((ProdutoEditadoNotification)produto, cancellationToken);
            return Unit.Value;
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }
    }
}
