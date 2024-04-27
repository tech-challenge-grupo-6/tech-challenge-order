using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Shared.Validators;

namespace ControladorPedidos.Application.Produtos.Validators;

public class CategoriaValidador : IValidador<Categoria>
{
    public static bool IsValid(Categoria entidade)
    {
        if (string.IsNullOrEmpty(entidade.Nome))
            throw new ArgumentException("Nome não pode ser vazio");

        return true;
    }
}
