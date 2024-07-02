using ControladorPedidos.Application.Clientes.Notifications;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.Pedidos.Notifications;
using ControladorPedidos.Application.Produtos.Notifications;
using MediatR;

namespace ControladorPedidos.Application.NotificationsHandlers;

public class LogNotificationHandler(ILogger<LogNotificationHandler> logger)
    : INotificationHandler<ExceptionNotification>,
    INotificationHandler<ClienteCriadoNotification>,
    INotificationHandler<CategoriaCriadaNotification>,
    INotificationHandler<ProdutoCriadoNotification>,
    INotificationHandler<ProdutoEditadoNotification>,
    INotificationHandler<ProdutoRemovidoNotification>,
    INotificationHandler<PedidoCadastradoNotification>

{
    public Task Handle(ExceptionNotification notification, CancellationToken cancellationToken)
    {
        logger.LogError(notification.Exception, "Erro: {Message}", notification.Exception.Message);
        return Task.CompletedTask;
    }

    public Task Handle(ClienteCriadoNotification notification, CancellationToken cancellationToken)
    {

        logger.LogInformation("Cliente criado: {Cliente}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(CategoriaCriadaNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Categoria criada: {Categoria}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(ProdutoCriadoNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Produto criado: {Produto}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(ProdutoEditadoNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Produto editado: {Produto}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(ProdutoRemovidoNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Produto removido: {Produto}", notification);
        return Task.CompletedTask;
    }

    public Task Handle(PedidoCadastradoNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Pedido cadastrado: {Pedido}", notification);
        return Task.CompletedTask;
    }
}
