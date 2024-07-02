using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Pedidos.Repositories;
using ControladorPedidos.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ControladorPedidos.Gateways;

public class PedidoRepository(DatabaseContext dbContext) : IPedidoRepository
{
    public async Task<IEnumerable<Pedido?>> GetAll()
    {
        return await dbContext
            .Pedidos
            .Include(p => p.Produtos)
            .Where(p => p.Excluido == false)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pedido?>> GetByStatus(Status status)
    {
        return await dbContext
            .Pedidos
            .Where(p => p.Status == status)
            .Where(p => p.Excluido == false)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pedido?>> GetByCliente(Guid clienteId)
    {
        return await dbContext
            .Pedidos
            .Where(p => p.ClienteId == clienteId)
            .Where(p => p.Excluido == false)
            .ToListAsync();
    }

    public async Task<Pedido?> GetById(Guid id)
    {
        return await dbContext
          .Pedidos
          .Include(p => p.Produtos)
          .Where(p => p.Excluido == false)
          .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task Add(Pedido Pedido)
    {
        dbContext.Pedidos.Add(Pedido);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateStatus(Pedido pedido)
    {
        dbContext.Pedidos.Update(pedido);
        await dbContext.SaveChangesAsync();
    }
}