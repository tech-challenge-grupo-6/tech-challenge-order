using System.Text.Json;
using ControladorPedidos.Application.Clientes.Repositories;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Pedidos.Commands;
using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Pedidos.Notifications;
using ControladorPedidos.Application.Pedidos.Queue.Models;
using ControladorPedidos.Application.Pedidos.Queue.Send;
using ControladorPedidos.Application.Pedidos.Repositories;
using ControladorPedidos.Application.Pedidos.Validators;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;

namespace ControladorPedidos.Application.Pedidos.Handlers;

public class CadastrarPedidoCommandHandler(
    IMediator mediator,
    IPedidoRepository pedidoRepository,
    IClienteRepository clienteRepository,
    IProdutoRepository produtoRepository,
    CacheConfiguration cacheConfiguration,
    JsonSerializerOptions jsonSerializerOptions) : IRequestHandler<CadastrarPedidoCommand, Guid>
{
    public async Task<Guid> Handle(CadastrarPedidoCommand request, CancellationToken cancellationToken)
    {
        Pedido pedido = (Pedido)request;
        try
        {
            PedidoValidador.IsValid(pedido);

            pedido.Cliente = await clienteRepository.GetById(request.ClienteId) ?? throw new ArgumentException("Cliente não encontrado");

            pedido.Produtos = [];
            foreach (Guid produtoId in request.ProdutosIds)
            {
                Produto produto = await produtoRepository.GetById(produtoId) ?? throw new ArgumentException("Produto não encontrado");
                pedido.Produtos.Add(produto);
            }

            pedido.CalcularValorTotal();
            await pedidoRepository.Add(pedido);
            await mediator.Publish((PedidoCadastradoNotification)pedido, cancellationToken);
            string key = $"{cacheConfiguration.PedidoPrefix}:{pedido.Id}";
            string value = JsonSerializer.Serialize(pedido, jsonSerializerOptions);
            await mediator.Publish(new CacheNotification(key, value), cancellationToken);
            await mediator.Publish(new PedidoCriadoQueueSendMessage((PedidoQueue)pedido), cancellationToken);
            return pedido.Id;
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }

    }
}
