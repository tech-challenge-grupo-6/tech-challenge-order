using ControladorPedidos.Application.Produtos.Models;
using MediatR;

namespace ControladorPedidos.Application.Produtos.Notifications;

public record CategoriaCriadaNotification(Guid Id, string Nome) : INotification
{
    public static explicit operator CategoriaCriadaNotification(Categoria categoria) => new(categoria.Id, categoria.Nome);
}
