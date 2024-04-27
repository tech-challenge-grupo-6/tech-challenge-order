using ControladorPedidos.Application.Produtos.Models;

namespace ControladorPedidos.Application.Produtos.Repositories;

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> GetAll();
    Task<Produto?> GetById(Guid id);
    Task<IEnumerable<Produto>> GetByCategoria(Guid categoriaId);
    Task Add(Produto produto);
    void Remove(Produto produto);
    void Update(Produto produto);
    void Add(Categoria categoria);
    void Update(Categoria categoria);
    Task<IEnumerable<Categoria>> GetAllCategorias();
    Task<Categoria?> GetCategoriaById(Guid id);
}
