namespace LogiMarket.Domain.Promociones;
// Patron Strategy: cada promocion decide cuando aplica y que tasa utiliza.
// Strategy: la calculadora depende del contrato y admite nuevas politicas.
public interface IPoliticaDescuento
{
    int Prioridad { get; }

    decimal Tasa { get; }

    bool Aplica(ContextoPromocion contexto);
}
