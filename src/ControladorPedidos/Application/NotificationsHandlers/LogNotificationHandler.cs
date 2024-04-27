using ControladorPedidos.Application.Exceptions.Notifications;
using MediatR;

namespace ControladorPedidos.Application.NotificationHandlers;

public class LogNotificationHandler(ILogger<LogNotificationHandler> logger) : INotificationHandler<ExceptionNotification>
{
    public Task Handle(ExceptionNotification notification, CancellationToken cancellationToken)
    {
        logger.LogError(notification.Exception, "Erro: {Message}", notification.Exception.Message);
        return Task.CompletedTask;
    }
}
