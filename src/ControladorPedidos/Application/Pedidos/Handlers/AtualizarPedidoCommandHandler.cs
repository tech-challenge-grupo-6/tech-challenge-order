using System.Text.Json;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Pedidos.Commands;
using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Pedidos.Repositories;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;

namespace ControladorPedidos.Application.Pedidos.Handlers;

public class AtualizarPedidoCommandHandler(
    IMediator mediator,
    IPedidoRepository pedidoRepository,
    CacheConfiguration cacheConfiguration,
    JsonSerializerOptions jsonSerializerOptions) : IRequestHandler<AtualizarPedidoCommand, Unit>
{
    public async Task<Unit> Handle(AtualizarPedidoCommand request, CancellationToken cancellationToken)
    {
        var (id, status, pago) = request;
        try
        {
            Pedido pedido = await pedidoRepository.GetById(new Guid(id)) ?? throw new ArgumentException("Pedido não encontrado");

            pedido.Status = status;
            pedido.Pago = pago;

            await pedidoRepository.UpdateStatus(pedido);
            string key = $"{cacheConfiguration.PedidoPrefix}:{pedido.Id}";
            string value = JsonSerializer.Serialize(pedido, jsonSerializerOptions);
            await mediator.Publish(new CacheNotification(key, value), cancellationToken);
            return Unit.Value;
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }
    }
}
