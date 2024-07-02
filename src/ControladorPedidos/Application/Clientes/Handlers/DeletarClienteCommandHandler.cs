using ControladorPedidos.Application.Clientes.Commands;
using ControladorPedidos.Application.Clientes.Repositories;
using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Pedidos.Repositories;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;

namespace ControladorPedidos.Application.Clientes.Handlers;

public class DeletarClienteCommandHandler(
    IMediator mediator,
    IClienteRepository repository,
    IPedidoRepository pedidoRepository,
    CacheConfiguration cacheConfiguration) : IRequestHandler<DeletarClienteCommand, Unit>
{
    public async Task<Unit> Handle(DeletarClienteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await repository.GetByCpf(request.Cpf) ?? throw new NotFoundException("Cliente não encontrado");

            cliente.Excluido = true;
            await repository.Update(cliente);

            string key = $"{cacheConfiguration.ClientePrefix}:{cliente.Cpf}";
            await mediator.Publish(new CacheNotification(key, null!), cancellationToken);

            var pedidos = await pedidoRepository.GetByCliente(cliente.Id);
            if (pedidos.Any())
            {
                foreach (var pedido in pedidos)
                {
                    if (pedido is not null)
                    {
                        pedido.Excluido = true;
                        await pedidoRepository.UpdateStatus(pedido);
                        key = $"{cacheConfiguration.PedidoPrefix}:{pedido.Id}";
                        await mediator.Publish(new CacheNotification(key, null!), cancellationToken);
                    }
                }
            }

            return Unit.Value;
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }
    }
}
