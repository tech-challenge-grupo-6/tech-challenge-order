using ControladorPedidos.Application.Clientes.Notifications;
using ControladorPedidos.Application.Exceptions.Notifications;
using MediatR;

namespace ControladorPedidos.Application.NotificationHandlers;

public class LogNotificationHandler(ILogger<LogNotificationHandler> logger)
    : INotificationHandler<ExceptionNotification>,
    INotificationHandler<ClienteCriadoNotification>

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
}
