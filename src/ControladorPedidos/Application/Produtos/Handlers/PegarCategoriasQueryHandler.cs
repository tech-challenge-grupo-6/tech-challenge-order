using System.Text.Json;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Queries;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace ControladorPedidos.Application.Produtos.Handlers;

public class PegarCategoriasQueryHandler(IMediator mediator, IProdutoRepository repository, CacheConfiguration cacheConfiguration, IDistributedCache cache) : IRequestHandler<PegarCategoriasQuery, PegarCategoriasQueryResponse>
{
    public async Task<PegarCategoriasQueryResponse> Handle(PegarCategoriasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            string key = cacheConfiguration.CategoriaPrefix;
            string cacheValue = await cache.GetStringAsync(key, cancellationToken) ?? string.Empty;
            Dictionary<Guid, Categoria> categorias = [];
            if (!string.IsNullOrEmpty(cacheValue))
            {
                categorias = JsonSerializer.Deserialize<Dictionary<Guid, Categoria>>(cacheValue) ?? [];
            }
            else
            {
                categorias = (await repository.GetAllCategorias())
                    .Select(c => new KeyValuePair<Guid, Categoria>(c.Id, c))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                cacheValue = JsonSerializer.Serialize(categorias);
                await mediator.Publish(new CacheNotification(key, cacheValue), cancellationToken);
            }
            return new PegarCategoriasQueryResponse(categorias.Select(c => (PegarCategoriaQueryResponse)c.Value));
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }
    }
}
