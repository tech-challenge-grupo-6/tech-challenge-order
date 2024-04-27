using MediatR;

namespace ControladorPedidos.Application.Exceptions.Notifications;

public record ExceptionNotification(Exception Exception) : INotification
{
    public static explicit operator ExceptionNotification(Exception exception) => new(exception);
}
