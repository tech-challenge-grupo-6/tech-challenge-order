using ControladorPedidos.Application.Produtos.Models;
using MediatR;

namespace ControladorPedidos.Application.Produtos.Commands;

public record RemoverProdutoCommand(Guid Id) : IRequest<Unit>
{
    public static explicit operator Produto(RemoverProdutoCommand command) => new()
    {
        Id = command.Id
    };
}
