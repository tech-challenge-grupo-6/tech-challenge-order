using ControladorPedidos.Application.Clientes.Models;
using MediatR;

namespace ControladorPedidos.Application.Clientes.Notifications;

public record ClienteCriadoNotification(Guid Id, string Nome, string Cpf, string Email) : INotification
{
    public static explicit operator ClienteCriadoNotification(Cliente cliente) => new(cliente.Id, cliente.Nome, cliente.Cpf, cliente.Email);
}
