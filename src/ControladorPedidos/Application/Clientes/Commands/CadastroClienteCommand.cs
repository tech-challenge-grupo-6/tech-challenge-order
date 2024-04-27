using ControladorPedidos.Application.Clientes.Models;
using MediatR;

namespace ControladorPedidos.Application.Clientes.Commands;

public record CadastroClienteCommand(string Nome, string Cpf, string Email) : IRequest<string>
{
    public static explicit operator Cliente(CadastroClienteCommand command) => new()
    {
        Nome = command.Nome,
        Cpf = command.Cpf,
        Email = command.Email
    };
}
