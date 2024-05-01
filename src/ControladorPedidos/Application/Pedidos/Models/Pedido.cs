using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Shared.Models;

namespace ControladorPedidos.Application.Pedidos.Models;

public class Pedido : EntityBase
{
    public virtual Cliente? Cliente { get; set; } = null!;
    public Guid? ClienteId { get; set; }
    public Status Status { get; set; } = Status.Criado;
    public virtual ICollection<Produto> Produtos { get; set; } = null!;
    public double ValorTotal { get; set; }

    public double CalcularValorTotal()
    {
        ValorTotal = Produtos.Sum(p => p.Preco);
        return ValorTotal;
    }
}
