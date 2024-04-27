using ControladorPedidos.Application.Clientes.Queries;
using ControladorPedidos.Application.Clientes.Repositories;
using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Exceptions.Notifications;
using MediatR;

namespace ControladorPedidos.Application.Clientes.Handlers;

public class PegarClienteQueryHandler(IMediator mediator, IClienteRepository repository) : IRequestHandler<PegarClientePorCpfQuery, PegarClientePorCpfQueryResponse>
{
    public async Task<PegarClientePorCpfQueryResponse> Handle(PegarClientePorCpfQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await repository.GetByCpf(request.Cpf) ?? throw new NotFoundException("Cliente não encontrado");
            return (PegarClientePorCpfQueryResponse)cliente;
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }
    }
}
