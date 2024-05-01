using ControladorPedidos.Application.Produtos.Models;
using MediatR;

namespace ControladorPedidos.Application.Produtos.Commands;

public record CadastrarProdutoCommand(string Nome, Guid CategoriaId, double Preco, string Descricao, string Imagem) : IRequest<string>
{
    public static explicit operator Produto(CadastrarProdutoCommand command) => new()
    {
        Nome = command.Nome,
        CategoriaId = command.CategoriaId,
        Preco = command.Preco,
        Descricao = command.Descricao,
        Imagem = command.Imagem
    };
}
