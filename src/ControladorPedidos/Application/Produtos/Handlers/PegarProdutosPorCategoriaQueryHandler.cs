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

public class PegarProdutosPorCategoriaQueryHandler(IMediator mediator, IProdutoRepository repository, CacheConfiguration cacheConfiguration, IDistributedCache cache) : IRequestHandler<PegarProdutosPorCategoriaQuery, IEnumerable<PegarProdutoPorCategoriaQuery>>
{
    public async Task<IEnumerable<PegarProdutoPorCategoriaQuery>> Handle(PegarProdutosPorCategoriaQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var idsByCategoria = await repository.GetIdsByCategoria(request.CategoriaId);
            List<PegarProdutoPorCategoriaQuery> produtos = [];
            foreach (Guid id in idsByCategoria)
            {
                string key = $"{cacheConfiguration.ProdutoPrefix}:{id}";
                string cacheValue = await cache.GetStringAsync(key, cancellationToken) ?? string.Empty;
                Produto produto = null!;
                if (string.IsNullOrWhiteSpace(cacheValue))
                {
                    produto = await repository.GetById(id) ?? null!;
                    if (produto is not null)
                    {
                        cacheValue = JsonSerializer.Serialize(produto);
                        await mediator.Publish(new CacheNotification(key, cacheValue), cancellationToken);
                    }
                }
                else
                {
                    produto = JsonSerializer.Deserialize<Produto>(cacheValue)!;
                }
                produtos.Add((PegarProdutoPorCategoriaQuery)produto!);
            }
            return produtos;
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }
    }
}
