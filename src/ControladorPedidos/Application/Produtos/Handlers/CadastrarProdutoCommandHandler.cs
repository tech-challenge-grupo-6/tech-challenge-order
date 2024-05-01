using System.Text.Json;
using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Produtos.Commands;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Application.Produtos.Validators;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;

namespace ControladorPedidos.Application.Produtos.Handlers;

public class CadastrarProdutoCommandHandler(IMediator mediator, IProdutoRepository produtoRepository, CacheConfiguration cacheConfiguration) : IRequestHandler<CadastrarProdutoCommand, string>
{
    public async Task<string> Handle(CadastrarProdutoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Produto produto = (Produto)request;
            if (!ProdutoValidador.IsValid(produto))
            {
                throw new BusinessException("Produto inválido");
            }
            await produtoRepository.Add(produto);
            string key = $"{cacheConfiguration.ProdutoPrefix}:{produto.Id}";
            string cacheValue = JsonSerializer.Serialize(produto);
            await mediator.Publish(new CacheNotification(key, cacheValue), cancellationToken);
            return produto.Id.ToString();
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }
    }
}
