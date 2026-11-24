namespace LogiMarket.Domain.Operaciones;
// Responsabilidad de Aviso: colaborar desde LogiMarket.Domain.Operaciones sin mezclar reglas, persistencia y pantallas.
public sealed class Aviso
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid EventoId { get; private set; }
    public string Canal { get; private set; } = "";
    public string Mensaje { get; private set; } = "";

    private Aviso()
    {
    }

    public Aviso(Guid evento, string canal, string mensaje)
    {
        EventoId = evento;
        Canal = canal;
        Mensaje = mensaje;
    }
}
