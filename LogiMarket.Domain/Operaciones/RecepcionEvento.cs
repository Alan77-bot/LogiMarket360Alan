namespace LogiMarket.Domain.Operaciones;
// Responsabilidad de RecepcionEvento: colaborar desde LogiMarket.Domain.Operaciones sin mezclar reglas, persistencia y pantallas.
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
