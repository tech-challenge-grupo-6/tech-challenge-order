using ControladorPedidos.Application.Pedidos.Models;

namespace ControladorPedidos.Application.Pedidos.Queue.Models;

public record ProdutoQueue(string Id, string Name, double Valor);
public record PedidoQueue(string Id, Status Status, string ClienteId, ProdutoQueue[] Produtos, double ValorTotal, bool Pago);