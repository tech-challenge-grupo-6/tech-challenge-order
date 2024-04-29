using MediatR;

namespace ControladorPedidos.Application.Shared.Notifications;

public record CacheNotification(string Key, string Value) : INotification;

