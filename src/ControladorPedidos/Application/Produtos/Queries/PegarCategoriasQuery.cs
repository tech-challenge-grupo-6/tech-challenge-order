using ControladorPedidos.Application.Produtos.Models;
using MediatR;

namespace ControladorPedidos.Application.Produtos.Queries;

public record PegarCategoriaQueryResponse(Guid Id, string Nome)
{
    public static explicit operator PegarCategoriaQueryResponse(Categoria categoria) => new(categoria.Id, categoria.Nome);
}
public record PegarCategoriasQueryResponse(IEnumerable<PegarCategoriaQueryResponse> Categorias);

public record PegarCategoriasQuery : IRequest<PegarCategoriasQueryResponse>;
