namespace LogiMarket.Domain.Operaciones;
// Registro trazable del negocio las restricciones de persistencia completan su consistencia
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
