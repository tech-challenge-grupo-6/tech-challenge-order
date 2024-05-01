using ControladorPedidos.Application.Produtos.Models;

namespace ControladorPedidos.Application.Produtos.Repositories;

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> GetAll();
    Task<Produto?> GetById(Guid id);
    Task<IEnumerable<Guid>> GetIdsByCategoria(Guid categoriaId);
    Task Add(Produto produto);
    Task Remove(Produto produto);
    Task Update(Produto produto);
    Task Add(Categoria categoria);
    Task Update(Categoria categoria);
    Task<IEnumerable<Categoria>> GetAllCategorias();
    Task<Categoria?> GetCategoriaById(Guid id);
}
