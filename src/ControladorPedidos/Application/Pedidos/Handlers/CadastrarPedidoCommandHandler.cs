using System.Text.Json;
using System.Text.Json.Serialization;
using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Application.Clientes.Repositories;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Pedidos.Commands;
using ControladorPedidos.Application.Pedidos.Models;
using ControladorPedidos.Application.Pedidos.Notifications;
using ControladorPedidos.Application.Pedidos.Repositories;
using ControladorPedidos.Application.Pedidos.Validators;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace ControladorPedidos.Application.Pedidos.Handlers;

public class CadastrarPedidoCommandHandler(IMediator mediator, IPedidoRepository pedidoRepository, IClienteRepository clienteRepository, IProdutoRepository produtoRepository, CacheConfiguration cacheConfiguration) : IRequestHandler<CadastrarPedidoCommand, Guid>
{
    public async Task<Guid> Handle(CadastrarPedidoCommand request, CancellationToken cancellationToken)
    {
        Pedido pedido = (Pedido)request;
        try
        {
            if (!PedidoValidador.IsValid(pedido))
            {
                throw new ArgumentException("Pedido inválido");
            }

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
            string value = JsonSerializer.Serialize(pedido, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            await mediator.Publish(new CacheNotification(key, value), cancellationToken);
            return pedido.Id;
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }

    }
}
