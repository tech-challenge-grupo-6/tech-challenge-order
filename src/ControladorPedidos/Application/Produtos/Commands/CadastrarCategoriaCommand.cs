using ControladorPedidos.Application.Produtos.Models;
using MediatR;

namespace ControladorPedidos.Application.Produtos.Commands;

public record CadastrarCategoriaCommand(string Nome) : IRequest<string>
{
    public static explicit operator Categoria(CadastrarCategoriaCommand command) => new()
    {
        Nome = command.Nome
    };
}
