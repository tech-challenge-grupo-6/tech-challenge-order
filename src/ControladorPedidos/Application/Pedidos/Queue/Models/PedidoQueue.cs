using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Produtos.Models;

namespace ControladorPedidos.Application.Pedidos.Queue.Models;

public record ProdutoQueue(string Id, string Descricao, double Preco)
{
    public static explicit operator ProdutoQueue(Produto produto) =>
        new(produto.Id.ToString(), produto.Descricao, produto.Preco);

}
public record PedidoQueue(string Id, Status Status, string ClienteId, ProdutoQueue[] Produtos, double ValorTotal, bool Pago)
{
    public static explicit operator PedidoQueue(Pedido pedido) =>
        new(pedido.Id.ToString(), pedido.Status, pedido.ClienteId.ToString()!, pedido.Produtos.Select(p => (ProdutoQueue)p).ToArray(), pedido.ValorTotal, pedido.Pago);
}

public record PedidoCozinhaQueue(string OrderId, int Status);