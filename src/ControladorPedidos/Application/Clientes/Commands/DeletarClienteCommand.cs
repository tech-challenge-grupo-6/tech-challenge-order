using MediatR;

namespace ControladorPedidos.Application.Clientes.Commands;

public record DeletarClienteCommand(string Cpf) : IRequest<Unit>;
