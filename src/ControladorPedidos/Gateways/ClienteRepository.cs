using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Application.Clientes.Repositories;
using ControladorPedidos.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ControladorPedidos.Gateways;

public class ClienteRepository(DatabaseContext dbContext) : IClienteRepository
{
    public async Task<Cliente?> GetById(Guid id)
    {
        return await dbContext.Clientes.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cliente?> GetByCpf(string cpf)
    {
        return await dbContext.Clientes.FirstOrDefaultAsync(c => c.Cpf == cpf);
    }

    public async Task Add(Cliente Cliente)
    {
        dbContext.Clientes.Add(Cliente);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(Cliente Cliente)
    {
        dbContext.Clientes.Update(Cliente);
        await dbContext.SaveChangesAsync();
    }
}
