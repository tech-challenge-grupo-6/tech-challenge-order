using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Shared.Models;

namespace ControladorPedidos.Application.Clientes.Models;

public class Cliente : EntityBase
{
    public string Nome { get; set; } = null!;
    public string Cpf { get; set; } = null!;
    public string Email { get; set; } = null!;
    public virtual ICollection<Pedido> Pedidos { get; set; } = null!;
}
