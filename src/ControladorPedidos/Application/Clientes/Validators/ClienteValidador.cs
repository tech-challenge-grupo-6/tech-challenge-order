using System.Text.RegularExpressions;
using ControladorPedidos.Application.Clientes.Models;
using ControladorPedidos.Application.Shared.Validators;

namespace ControladorPedidos.Application.Clientes.Validators;

public partial class ClienteValidador : IValidador<Cliente>
{
    public static bool IsValid(Cliente entidade)
    {
        if (string.IsNullOrEmpty(entidade.Nome))
            throw new ArgumentException("Nome não pode ser vazio");


        if (!ValidarEmail(entidade.Email))
            throw new ArgumentException("Email inválido");

        if (!ValidarCpf(entidade.Cpf))
            throw new ArgumentException("Cpf inválido");

        return true;
    }

    public static bool ValidarCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        try
        {
            // Remove caracteres não numéricos do CPF
            cpf = ReplaceRegex().Replace(cpf, "");

            // Verifica se o CPF possui 11 dígitos
            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos do CPF são iguais
            if (new string(cpf[0], 11) == cpf)
                return false;

            // Calcula o primeiro dígito verificador do CPF
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * (10 - i);
            int resto = soma % 11;
            int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

            // Calcula o segundo dígito verificador do CPF
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * (11 - i);
            resto = soma % 11;
            int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

            // Verifica se os dígitos verificadores do CPF são válidos
            return cpf.EndsWith(digitoVerificador1.ToString() + digitoVerificador2.ToString());
        }
        catch
        {
            return false;
        }
    }

    public static bool ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Cria um objeto Regex com a expressão regular
            Regex regex = EmailRegex();

            // Verifica se o e-mail corresponde à expressão regular
            return regex.IsMatch(email);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    [GeneratedRegex(@"[^\d]")]
    private static partial Regex ReplaceRegex();
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
