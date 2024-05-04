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
using Microsoft.Extensions.Caching.Distributed;

namespace ControladorPedidos.Application.Produtos.Handlers;

public class CadastrarCategoriaCommandHandler(IMediator mediator, IProdutoRepository repository, CacheConfiguration cacheConfiguration, IDistributedCache cache) : IRequestHandler<CadastrarCategoriaCommand, string>
{
    public async Task<string> Handle(CadastrarCategoriaCommand request, CancellationToken cancellationToken)
    {
        Categoria categoria = (Categoria)request;
        try
        {
            CategoriaValidador.IsValid(categoria);

            await repository.Add(categoria);
            await mediator.Publish((CategoriaCriadaNotification)categoria, cancellationToken);

            string cacheCategorias = await cache.GetStringAsync(cacheConfiguration.CategoriaPrefix, cancellationToken) ?? string.Empty;
            Dictionary<Guid, Categoria> categorias = [];
            if (!string.IsNullOrWhiteSpace(cacheCategorias))
            {
                categorias = JsonSerializer.Deserialize<Dictionary<Guid, Categoria>>(cacheCategorias) ?? [];
            }
            categorias[categoria.Id] = categoria;
            cacheCategorias = JsonSerializer.Serialize(categorias);
            await mediator.Publish(new CacheNotification(cacheConfiguration.CategoriaPrefix, cacheCategorias), cancellationToken);

            return categoria.Id.ToString();
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }

    }
}
