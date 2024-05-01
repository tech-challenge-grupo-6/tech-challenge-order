using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Produtos.Models;
using MediatR;

namespace ControladorPedidos.Application.Pedidos.Commands;

public record CadastrarPedidoCommand(Guid ClienteId, IEnumerable<Guid> ProdutosIds) : IRequest<Guid>
{
    public static explicit operator Pedido(CadastrarPedidoCommand command)
    {
        return new Pedido
        {
            ClienteId = command.ClienteId,
            Produtos = command.ProdutosIds.Select(id => new Produto { Id = id }).ToList()
        };
    }
}
