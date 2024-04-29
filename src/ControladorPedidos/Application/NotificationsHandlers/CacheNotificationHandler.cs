using ControladorPedidos.Application.Shared.Notifications;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace ControladorPedidos.Application.NotificationsHandlers;

public class CacheNotificationHandler(IDistributedCache cache) : INotificationHandler<CacheNotification>
{
    public async Task Handle(CacheNotification notification, CancellationToken cancellationToken)
    {
        await cache.SetStringAsync(notification.Key, notification.Value, cancellationToken);
    }
}
