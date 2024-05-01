using ControladorPedidos.Application.Shared.Notifications;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace ControladorPedidos.Application.NotificationsHandlers;

public class CacheNotificationHandler(IDistributedCache cache) : INotificationHandler<CacheNotification>
{
    public async Task Handle(CacheNotification notification, CancellationToken cancellationToken)
    {
        var (key, value) = notification;
        if (string.IsNullOrWhiteSpace(value))
        {
            await cache.RemoveAsync(key, cancellationToken);
            return;
        }
        await cache.SetStringAsync(key, value, cancellationToken);
    }
}
