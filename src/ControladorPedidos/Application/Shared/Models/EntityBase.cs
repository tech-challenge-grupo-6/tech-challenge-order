namespace ControladorPedidos.Application.Shared.Models;

public class EntityBase
{
    public Guid Id { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}
