namespace ControladorPedidos.Application.Shared.Validators;

public interface IValidador<T> where T : class
{
    static abstract bool IsValid(T entidade);
}
