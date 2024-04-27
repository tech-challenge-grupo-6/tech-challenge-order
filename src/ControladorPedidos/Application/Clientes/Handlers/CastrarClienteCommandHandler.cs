using ControladorPedidos.Application.Clientes.Commands;
using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Application.Clientes.Notifications;
using ControladorPedidos.Application.Clientes.Repositories;
using ControladorPedidos.Application.Clientes.Validators;
using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Exceptions.Notifications;
using MediatR;

namespace ControladorPedidos.Application.Clientes.Handlers;

public class CastrarClienteCommandHandler(IMediator mediator, IClienteRepository repository) : IRequestHandler<CadastroClienteCommand, string>
{
    public async Task<string> Handle(CadastroClienteCommand request, CancellationToken cancellationToken)
    {
        Cliente cliente = (Cliente)request;
        try
        {
            if (!ClienteValidador.IsValid(cliente))
            {
                throw new BusinessException("Cliente inválido");
            }

            await repository.Add(cliente);
            await mediator.Publish((ClienteCriadoNotification)cliente, cancellationToken);
            return cliente.Id.ToString();
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }

    }
}
