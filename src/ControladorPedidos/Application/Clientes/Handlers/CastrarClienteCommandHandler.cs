using System.Text.Json;
using ControladorPedidos.Application.Clientes.Commands;
using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Application.Clientes.Notifications;
using ControladorPedidos.Application.Clientes.Repositories;
using ControladorPedidos.Application.Clientes.Validators;
using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;

namespace ControladorPedidos.Application.Clientes.Handlers;

public class CastrarClienteCommandHandler(IMediator mediator, IClienteRepository repository, CacheConfiguration cacheConfiguration) : IRequestHandler<CadastroClienteCommand, string>
{
    public async Task<string> Handle(CadastroClienteCommand request, CancellationToken cancellationToken)
    {
        Cliente cliente = (Cliente)request;
        try
        {
            ClienteValidador.IsValid(cliente);

            await repository.Add(cliente);
            await mediator.Publish((ClienteCriadoNotification)cliente, cancellationToken);
            string key = $"{cacheConfiguration.ClientePrefix}:{cliente.Cpf}";
            string value = JsonSerializer.Serialize(cliente);
            await mediator.Publish(new CacheNotification(key, value), cancellationToken);
            return cliente.Id.ToString();
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }

    }
}
