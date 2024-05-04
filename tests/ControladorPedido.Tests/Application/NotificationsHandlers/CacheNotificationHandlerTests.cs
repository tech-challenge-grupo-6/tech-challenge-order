using ControladorPedidos.Application.NotificationsHandlers;
using ControladorPedidos.Application.Shared.Notifications;
using Microsoft.Extensions.Caching.Distributed;

namespace ControladorPedido.Tests.Application.NotificationsHandlers;

public class CacheNotificationHandlerTests
{
    [Fact]
    public async Task Handle_WhenValueIsNullOrWhiteSpace_RemovesKeyFromCache()
    {
        // Arrange
        var cache = Substitute.For<IDistributedCache>();
        var handler = new CacheNotificationHandler(cache);
        var notification = new CacheNotification("key", null!);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        await cache.Received(1).RemoveAsync("key", CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenValueIsNotNullOrWhiteSpace_SetsValueInCache()
    {
        // Arrange
        var cache = Substitute.For<IDistributedCache>();
        var handler = new CacheNotificationHandler(cache);
        var notification = new CacheNotification("key", "value");

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        await cache.ReceivedWithAnyArgs(1).SetStringAsync("key", "value", CancellationToken.None);
    }
}
