using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Produtos.Commands;
using ControladorPedidos.Application.Produtos.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControladorPedidos.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProdutoController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Consulta produtos por categoria
    /// </summary>
    /// <param name="categoriaId">Filtra produtos por categoria</param>
    /// <returns>Retorna produtos por categoria.</returns>
    /// <response code="200">Retorno dados do produto.</response>
    /// <response code="404">Não encontrado.</response>
    /// <response code="500">Erro interno.</response>
    [HttpGet("{categoriaId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListarPorCategoria(Guid categoriaId)
    {
        try
        {
            var produtos = await mediator.Send(new PegarProdutosPorCategoriaQuery(categoriaId));
            return Ok(produtos);
        }
        catch (NotFoundException ex)
        {
            await mediator.Publish((ExceptionNotification)ex);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            await mediator.Publish((ExceptionNotification)ex);
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno");
        }
    }

    /// <summary>
    /// Listar categorias de produto
    /// </summary>
    /// <returns>Retorna lista de categorias de produtos</returns>
    /// <response code="200">Retorno da lista de produto(s) consultado(s).</response>
    /// <response code="400">Erro ao fazer a Request.</response>
    /// <response code="500">Erro Interno.</response>
    [HttpGet("/categorias")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var categorias = await mediator.Send(new PegarCategoriasQuery());
            return Ok(categorias);
        }
        catch (NotFoundException e)
        {
            await mediator.Publish((ExceptionNotification)e);
            return NotFound("Categorias não encontradas");
        }
        catch (Exception ex)
        {
            await mediator.Publish((ExceptionNotification)ex);
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno");
        }
    }

    /// <summary>
    /// Criar uma categoria de produto
    /// </summary>
    /// <param name="categoria">Dados da nova categoria produto</param>
    /// <returns>Retorna ID nova categoria produto.</returns>
    /// <response code="201">Categoria criada com sucesso</response>
    /// <response code="500">Erro Interno.</response>
    [HttpPost("/categorias")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] CadastrarCategoriaCommand categoria)
    {
        try
        {
            var id = await mediator.Send(categoria);
            return Created($"/Categoria/{id}", new { id = id });
        }

        catch (ArgumentException ex)
        {
            await mediator.Publish((ExceptionNotification)ex);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (Exception ex)
        {
            await mediator.Publish((ExceptionNotification)ex);
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno");
        }
    }

    /// <summary>
    /// Criar um produto
    /// </summary>
    /// <param name="produtoDto">Dados do novo produto</param>
    /// <returns>Retorna ID novo produto.</returns>
    /// <response code="201">Produto criado com sucesso</response>
    /// <response code="400">Bad request</response>
    /// <response code="500">Erro Interno.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Post([FromBody] CadastrarProdutoCommand produtoDto)
    {
        try
        {
            string id = await mediator.Send(produtoDto);
            return CreatedAtAction(nameof(Post), new { id = id }, null);
        }
        catch (ArgumentException ex)
        {
            await mediator.Publish((ExceptionNotification)ex);
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            await mediator.Publish((ExceptionNotification)ex);
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno");
        }
    }

    /// <summary>
    /// Editar um produto
    /// </summary>
    /// <param name="id">Id do produto</param>
    /// <param name="produtoDto">Dados do produto</param>
    /// <returns>Retorna ID novo produto.</returns>
    /// <response code="200">Produto editado com sucesso</response>
    /// <response code="404">Produto não encontrado</response>
    /// <response code="400">Bad request.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Put(Guid id, [FromBody] EditarProdutoCommand produtoDto)
    {
        try
        {
            await mediator.Send(produtoDto);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            await mediator.Publish((ExceptionNotification)ex);
            return NotFound("Produto não encontrado");
        }
        catch (ArgumentException ex)
        {
            await mediator.Publish((ExceptionNotification)ex);
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    /// <summary>
    /// Deletar um produto
    /// </summary>
    /// <param name="id">Id do produto</param>
    /// <returns>Retorna ID novo produto.</returns>
    /// <response code="200">Produto deletado com sucesso</response>
    /// <response code="404">Produto não encontrado</response>
    /// <response code="400">Bad request.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await mediator.Send(new RemoverProdutoCommand(id));
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            await mediator.Publish((ExceptionNotification)ex);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            await mediator.Publish((ExceptionNotification)ex);
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
    }
}
