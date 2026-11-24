using LogiMarket.Application.DTOs;

namespace LogiMarket.Application.Contratos;
// Responsabilidad de IPasarelaPago: colaborar desde LogiMarket.Application.Contratos sin mezclar reglas, persistencia y pantallas.
public interface IPasarelaPago
{
    Task<ResultadoCobro> CobrarAsync(decimal importe, string clave);
}
