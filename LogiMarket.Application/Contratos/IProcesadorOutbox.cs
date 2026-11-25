namespace LogiMarket.Application.Contratos;
// Este puerto define lo que necesita Application Infrastructure proporciona la implementacion concreta
public interface IProcesadorOutbox
{
    Task<int> Procesar();
}
