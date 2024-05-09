using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Pedidos.Commands;
using ControladorPedidos.Application.Pedidos.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControladorPedidos.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PedidoController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Consulta todos os pedidos
    /// </summary>
    /// <returns>Retorna todos os pedidos.</returns>
    /// <response code="200">Retorno todos os pedidos.</response>
    /// <response code="404">Não encontrado.</response>
    /// <response code="500">Erro interno.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            PegarPedidoQueryResponse pedidos = await mediator.Send(new PegarPedidoQuery(id));
            return Ok(pedidos);
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
    /// Cria um novo pedido
    /// </summary>
    /// <param name="pedidoDto">Dados do novo pedido.</param>
    /// <returns>Retorna o ID do novo pedido.</returns>
    /// <response code="201">Pedido criado com sucesso.</response>
    /// <response code="400">Erro ao fazer a Request.</response>
    /// <response code="500">Erro interno.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Post([FromBody] CadastrarPedidoCommand pedidoDto)
    {
        try
        {
            Guid id = await mediator.Send(pedidoDto);
            return CreatedAtAction(nameof(Post), new { id = id });
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
}
