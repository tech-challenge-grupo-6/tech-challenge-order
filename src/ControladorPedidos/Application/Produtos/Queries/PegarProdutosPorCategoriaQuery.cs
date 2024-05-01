using ControladorPedidos.Application.Produtos.Models;
using MediatR;

namespace ControladorPedidos.Application.Produtos.Queries;

public record PegarProdutoPorCategoriaQuery(Guid Id, string Nome, Guid CategoriaId, double Preco, string Descricao, string Imagem)
{
    public static explicit operator PegarProdutoPorCategoriaQuery(Produto produto) =>
        new(produto.Id, produto.Nome, produto.CategoriaId, produto.Preco, produto.Descricao, produto.Imagem);
}

public record PegarProdutosPorCategoriaQuery(Guid CategoriaId) : IRequest<IEnumerable<PegarProdutoPorCategoriaQuery>>;
