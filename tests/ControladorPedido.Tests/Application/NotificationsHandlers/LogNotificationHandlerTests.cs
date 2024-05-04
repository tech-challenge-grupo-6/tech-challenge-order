using ControladorPedidos.Application.Clientes.Notifications;
using ControladorPedidos.Application.Exceptions.Notifications;
using ControladorPedidos.Application.NotificationHandlers;
using ControladorPedidos.Application.Pedidos.Notifications;
using ControladorPedidos.Application.Produtos.Notifications;
using Microsoft.Extensions.Logging;

namespace ControladorPedido.Tests.Application.NotificationsHandlers;

public class LogNotificationHandlerTests
{
    [Fact]
    public void Handle_ExceptionNotification_ShouldLogError()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LogNotificationHandler>>();
        var handler = new LogNotificationHandler(logger);
        var notification = new ExceptionNotification(new Exception("Test Exception"));

        // Act
        handler.Handle(notification, CancellationToken.None);

        // Assert
        logger.ReceivedWithAnyArgs(1).LogError(notification.Exception, "Erro: {Message}", notification.Exception.Message);
    }

    [Fact]
    public void Handle_ClienteCriadoNotification_ShouldLogInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LogNotificationHandler>>();
        var handler = new LogNotificationHandler(logger);
        var notification = new ClienteCriadoNotification(Guid.NewGuid(), "Teste", "123123", "test@test.com");

        // Act
        handler.Handle(notification, CancellationToken.None);

        // Assert
        logger.ReceivedWithAnyArgs(1).LogInformation("Cliente criado: {Cliente}", notification);
    }

    [Fact]
    public void Handle_CategoriaCriadaNotification_ShouldLogInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LogNotificationHandler>>();
        var handler = new LogNotificationHandler(logger);
        var notification = new CategoriaCriadaNotification(Guid.NewGuid(), "Teste");

        // Act
        handler.Handle(notification, CancellationToken.None);

        // Assert
        logger.ReceivedWithAnyArgs(1).LogInformation("Categoria criada: {Categoria}", notification);
    }

    [Fact]
    public void Handle_ProdutoCriadoNotification_ShouldLogInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LogNotificationHandler>>();
        var handler = new LogNotificationHandler(logger);
        var notification = new ProdutoCriadoNotification(Guid.NewGuid(), "Teste", Guid.NewGuid(), 10, "Test", "teste.jpg");

        // Act
        handler.Handle(notification, CancellationToken.None);

        // Assert
        logger.ReceivedWithAnyArgs(1).LogInformation("Produto criado: {Produto}", notification);
    }

    [Fact]
    public void Handle_ProdutoEditadoNotification_ShouldLogInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LogNotificationHandler>>();
        var handler = new LogNotificationHandler(logger);
        var notification = new ProdutoEditadoNotification(Guid.NewGuid(), "Teste", Guid.NewGuid(), 10, "Test", "teste.jpg");

        // Act
        handler.Handle(notification, CancellationToken.None);

        // Assert
        logger.ReceivedWithAnyArgs(1).LogInformation("Produto editado: {Produto}", notification);
    }

    [Fact]
    public void Handle_ProdutoRemovidoNotification_ShouldLogInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LogNotificationHandler>>();
        var handler = new LogNotificationHandler(logger);
        var notification = new ProdutoRemovidoNotification(Guid.NewGuid());

        // Act
        handler.Handle(notification, CancellationToken.None);

        // Assert
        logger.ReceivedWithAnyArgs(1).LogInformation("Produto removido: {Produto}", notification);
    }

    [Fact]
    public void Handle_PedidoCadastradoNotification_ShouldLogInformation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LogNotificationHandler>>();
        var handler = new LogNotificationHandler(logger);
        var notification = new PedidoCadastradoNotification(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), 100);

        // Act
        handler.Handle(notification, CancellationToken.None);

        // Assert
        logger.ReceivedWithAnyArgs(1).LogInformation("Pedido cadastrado: {Pedido}", notification);
    }

}
