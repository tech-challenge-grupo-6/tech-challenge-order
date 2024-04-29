using System.Text.Json;
using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Application.Clientes.Queries;
using ControladorPedidos.Application.Clientes.Repositories;
using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace ControladorPedidos.Application.Clientes.Handlers;

public class PegarClienteQueryHandler(IMediator mediator, IClienteRepository repository, IDistributedCache cache, CacheConfiguration cacheConfiguration) : IRequestHandler<PegarClientePorCpfQuery, PegarClientePorCpfQueryResponse>
{
    public async Task<PegarClientePorCpfQueryResponse> Handle(PegarClientePorCpfQuery request, CancellationToken cancellationToken)
    {
        try
        {
            string key = $"{cacheConfiguration.ClientePrefix}:{request.Cpf}";
            string cacheValue = await cache.GetStringAsync(key, cancellationToken) ?? string.Empty;
            Cliente cliente = null!;
            if (!string.IsNullOrEmpty(cacheValue))
            {
                cliente = JsonSerializer.Deserialize<Cliente>(cacheValue) ?? throw new NotFoundException("Cache cliente inválido");
            }
            else
            {
                cliente = await repository.GetByCpf(request.Cpf) ?? throw new NotFoundException("Cliente não encontrado");
                cacheValue = JsonSerializer.Serialize(cliente);
                await mediator.Publish(new CacheNotification(key, cacheValue), cancellationToken);
            }
            return (PegarClientePorCpfQueryResponse)cliente;
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }
    }
}
