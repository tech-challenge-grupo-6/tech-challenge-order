using ControladorPedidos.Application.Exceptions.Models;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Produtos.Commands;
using ControladorPedidos.Application.Produtos.Models;
using ControladorPedidos.Application.Produtos.Notifications;
using ControladorPedidos.Application.Produtos.Repositories;
using ControladorPedidos.Application.Shared.Notifications;
using ControladorPedidos.Infrastructure.Configurations;
using MediatR;

namespace ControladorPedidos.Application.Produtos.Handlers;

public class RemoverProdutoCommandHandler(IMediator mediator, IProdutoRepository produtoRepository, CacheConfiguration cacheConfiguration) : IRequestHandler<RemoverProdutoCommand, Unit>
{
    public async Task<Unit> Handle(RemoverProdutoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Produto produto = await produtoRepository.GetById(request.Id) ?? throw new NotFoundException("Produto não encontrado");
            await produtoRepository.Remove(produto);
            string key = $"{cacheConfiguration.ProdutoPrefix}:{produto.Id}";
            await mediator.Publish(new CacheNotification(key, string.Empty), cancellationToken);
            await mediator.Publish((ProdutoRemovidoNotification)produto, cancellationToken);
            return Unit.Value;
        }
        catch (Exception e)
        {
            await mediator.Publish((ExceptionNotification)e, cancellationToken);
            throw;
        }
    }
}