namespace ControladorPedidos.Infrastructure.Configurations;

public record CacheConfiguration(string ClientePrefix, string ProdutoPrefix, string CategoriaPrefix, string PedidoPrefix, string Configuration)
{
    public static CacheConfiguration FromConfiguration(IConfiguration configuration)
    {
        return new CacheConfiguration(
            configuration["Cache:ClientePrefix"] ?? "cliente",
            configuration["Cache:ProdutoPrefix"] ?? "produto",
            configuration["Cache:CategoriaPrefix"] ?? "categoria",
            configuration["Cache:PedidoPrefix"] ?? "pedido",
            configuration["Cache:Configuration"] ?? "localhost:6379");
    }
}
