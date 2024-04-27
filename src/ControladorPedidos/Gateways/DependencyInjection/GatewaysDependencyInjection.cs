using ControladorPedidos.Application.Clientes.Repositories;
using ControladorPedidos.Application.Pedidos.Repositories;
using ControladorPedidos.Application.Produtos.Repositories;

namespace ControladorPedidos.Gateways.DependencyInjection;

public static class GatewaysDependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        return services;
    }
}
