using System.Text.Json;
using System.Text.Json.Serialization;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Pedidos.Queries;
using ControladorPedidos.Application.Pedidos.Repositories;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace ControladorPedidos.Application.Pedidos.Handlers;

public class PegarPedidoQueryHandler(IMediator mediator, IPedidoRepository repository, CacheConfiguration cacheConfiguration, IDistributedCache cache) : IRequestHandler<PegarPedidoQuery, PegarPedidoQueryResponse>
{
    public async Task<PegarPedidoQueryResponse> Handle(PegarPedidoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            string key = $"{cacheConfiguration.PedidoPrefix}:{request.Id}";
            string value = await cache.GetStringAsync(key, cancellationToken) ?? string.Empty;
            Pedido pedido = null!;
            if (!string.IsNullOrWhiteSpace(value))
            {
                pedido = JsonSerializer.Deserialize<Pedido>(value, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve })!;
            }
            else
            {
                pedido = await repository.GetById(request.Id) ?? throw new ArgumentException("Pedido não encontrado");
                string pedidoValue = JsonSerializer.Serialize(pedido);
                await cache.SetStringAsync(key, pedidoValue, cancellationToken);
            }

            PegarPedidoQueryResponse response = (PegarPedidoQueryResponse)pedido;
            return response;
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }
    }
}

