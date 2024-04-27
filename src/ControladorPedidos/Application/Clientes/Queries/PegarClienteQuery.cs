using ControladorPedidos.Application.Clientes.Models;
using MediatR;

namespace ControladorPedidos.Application.Clientes.Queries;

public record PegarClientePorCpfQueryResponse(Guid Id, string Nome, string Cpf, string Email)
{
    public static explicit operator PegarClientePorCpfQueryResponse(Cliente cliente) => new(cliente.Id, cliente.Nome, cliente.Cpf, cliente.Email);
}

public record PegarClientePorCpfQuery(string Cpf) : IRequest<PegarClientePorCpfQueryResponse>;
