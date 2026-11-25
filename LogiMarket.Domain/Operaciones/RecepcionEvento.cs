namespace LogiMarket.Domain.Operaciones;
// Registro trazable del negocio las restricciones de persistencia completan su consistencia
public sealed class RecepcionEvento
{
    public Guid EventoId { get; private set; }
    public string Suscriptor { get; private set; } = "";

    private RecepcionEvento()
    {
    }

    public RecepcionEvento(Guid evento, string suscriptor)
    {
        EventoId = evento;
        Suscriptor = suscriptor;
    }
}
