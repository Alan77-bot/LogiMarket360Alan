using LogiMarket.Application.DTOs;

namespace LogiMarket.Application.Contratos;
// Este puerto define lo que necesita Application. Infrastructure proporciona la implementacion concreta.
public interface IPasarelaPago
{
    Task<ResultadoCobro> CobrarAsync(decimal importe, string clave);
}
