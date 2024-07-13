using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Pedidos.Queue.Models;
using MediatR;

namespace ControladorPedidos.Application.Pedidos.Commands;

public record AtualizarPedidoCommand(string Id, Status Status, bool Pago) : IRequest<Unit>
{
    public static explicit operator AtualizarPedidoCommand(PedidoQueue pedidoQueue)
    {
        return new AtualizarPedidoCommand(
            pedidoQueue.Id,
            pedidoQueue.Status,
            pedidoQueue.Pago
        );
    }

    public static explicit operator AtualizarPedidoCommand(PedidoCozinhaQueue pedidoCozinhaQueue)
    {
        return new AtualizarPedidoCommand(
            pedidoCozinhaQueue.OrderId,
            (Status)pedidoCozinhaQueue.Status,
            true
        );
    }
}
