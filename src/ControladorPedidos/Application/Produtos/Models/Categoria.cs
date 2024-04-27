using ControladorPedidos.Application.Shared.Models;

namespace ControladorPedidos.Application.Produtos.Models;

public class Categoria : EntityBase
{
    public string Nome { get; set; } = null!;
    public virtual ICollection<Produto> Produtos { get; set; } = null!;
}
