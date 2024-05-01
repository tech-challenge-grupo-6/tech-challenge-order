using ControladorPedidos.Application.Produtos.Models;
using MediatR;

namespace ControladorPedidos.Application.Produtos.Notifications;

public record ProdutoRemovidoNotification(Guid Id) : INotification
{
    public static explicit operator ProdutoRemovidoNotification(Produto produto) => new(produto.Id);
}
