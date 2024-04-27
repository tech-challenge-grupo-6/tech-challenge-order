using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Shared.Models;

namespace ControladorPedidos.Application.Produtos.Models;

public class Produto : EntityBase
{
    public string Nome { get; set; } = null!;
    public virtual Categoria Categoria { get; set; } = null!;
    public Guid CategoriaId { get; set; }
    public double Preco { get; set; }
    public string Descricao { get; set; } = null!;
    public string Imagem { get; set; } = null!;
    public virtual ICollection<Pedido> Pedidos { get; set; } = null!;
}
