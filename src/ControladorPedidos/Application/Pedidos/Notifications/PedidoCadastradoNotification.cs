using ControladorPedidos.Application.Pedidos.Models;
using MediatR;

namespace ControladorPedidos.Application.Pedidos.Notifications;

public record PedidoCadastradoNotification(Guid Id, Guid ClienteId, string ProdutosIds, double ValorTotal) : INotification
{
    public static explicit operator PedidoCadastradoNotification(Pedido pedido)
    {
        return new PedidoCadastradoNotification(pedido.Id, pedido.ClienteId ?? new Guid(), string.Join(",", pedido.Produtos.Select(p => p.Id)), pedido.CalcularValorTotal());
    }
}
