using ControladorPedidos.Application.Clientes.Commands;
using ControladorPedidos.Application.Clientes.Queries;
using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Exceptions.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControladorPedidos.Controllers;

[ApiController]
[Route("[controller]")]
// [Authorize]
public class ClienteController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Consulta Cliente por CPF
    /// </summary>
    /// <param name="cpf">Filtra o cliente por CPF</param>
    /// <returns>Retorna cliente por CPF.</returns>
    /// <response code="200">Retorno dados do cliente.</response>
    /// <response code="404">Não encontrado.</response>
    /// <response code="500">Erro interno.</response>
    [HttpGet("{cpf}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BuscarPorCpf(string cpf)
    {
        try
        {
            PegarClientePorCpfQueryResponse cliente = await mediator.Send(new PegarClientePorCpfQuery(cpf));
            return Ok(cliente);
        }
        catch (ArgumentException e)
        {
            await mediator.Publish((ExceptionNotification)e);
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (NotFoundException e)
        {
            await mediator.Publish((ExceptionNotification)e);
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e);
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno");
        }
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    /// <param name="clienteDto">Dados do novo cliente</param>
    /// <returns>Retorna o ID do novo cliente.</returns>
    /// <response code="201">Cliente criado com sucesso.</response>
    /// <response code="400">Erro ao fazer a Request.</response>
    /// <response code="500">Erro interno.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] CadastroClienteCommand clienteDto)
    {
        try
        {
            string id = await mediator.Send(clienteDto);
            return CreatedAtAction(nameof(Post), new { id });
        }
        catch (ArgumentException e)
        {
            await mediator.Publish((ExceptionNotification)e);
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e);
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno");
        }
    }

}
