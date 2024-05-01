using ControladorPedidos.Application.Produtos.Models;
using MediatR;

namespace ControladorPedidos.Application.Produtos.Commands;

public record EditarProdutoCommand(Guid Id, string Nome, Guid CategoriaId, double Preco, string Descricao, string Imagem) : IRequest<Unit>
{
    public static explicit operator Produto(EditarProdutoCommand command) => new()
    {
        Id = command.Id,
        Nome = command.Nome,
        CategoriaId = command.CategoriaId,
        Preco = command.Preco,
        Descricao = command.Descricao,
        Imagem = command.Imagem
    };
}
