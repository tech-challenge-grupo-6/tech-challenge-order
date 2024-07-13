using ControladorPedidos.Application.Pedidos.Models;
using MediatR;

namespace ControladorPedidos.Application.Pedidos.Queries;

public record PegarPedidoQueryResponse(Guid Id, Guid? ClienteId, Status Status, IEnumerable<Guid> ProdutosIds, double ValorTotal, bool Pago)
{
    public static explicit operator PegarPedidoQueryResponse(Pedido pedido)
    {
        return new PegarPedidoQueryResponse(pedido.Id, pedido.ClienteId, pedido.Status, pedido.Produtos.Select(p => p.Id), pedido.ValorTotal, pedido.Pago);
    }
}
public record PegarPedidoQuery(Guid Id) : IRequest<PegarPedidoQueryResponse>;