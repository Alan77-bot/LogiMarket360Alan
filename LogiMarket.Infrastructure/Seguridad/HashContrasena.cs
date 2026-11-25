using LogiMarket.Application.Contratos;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LogiMarket.Infrastructure.Seguridad;
// PBKDF2 con sal aleatoria y comparacion constante Las cuentas de demostracion nunca justifican guardar claves en texto
public sealed class HashContrasena : IHashContrasena
{
    private const int Iteraciones = 210000;
    public string Crear(string clave)
    {
        var sal = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(clave, sal, Iteraciones, HashAlgorithmName.SHA256, 32);
        return $"{Iteraciones}:{Convert.ToBase64String(sal)}:{Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string clave, string hash)
    {
        try
        {
            var partes = hash.Split(':');
            var esperado = Convert.FromBase64String(partes[2]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(clave, Convert.FromBase64String(partes[1]), int.Parse(partes[0]), HashAlgorithmName.SHA256, esperado.Length);
            return CryptographicOperations.FixedTimeEquals(actual, esperado);
        }
        catch (Exception ex)when (ex is FormatException or IndexOutOfRangeException or ArgumentException)
        {
            return false;
        }
    }
}
