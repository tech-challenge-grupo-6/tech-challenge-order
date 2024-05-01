using ControladorPedidos.Application.Produtos.Models;
using MediatR;

namespace ControladorPedidos.Application.Produtos.Notifications;

public record ProdutoCriadoNotification(Guid Id, string Nome, Guid CategoriaId, double Preco, string Descricao, string Imagem) : INotification
{
    public static explicit operator ProdutoCriadoNotification(Produto produto) => new(produto.Id, produto.Nome, produto.CategoriaId, produto.Preco, produto.Descricao, produto.Imagem);
}
