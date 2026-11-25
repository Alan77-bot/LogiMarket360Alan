using LogiMarket.Domain.Entidades;

namespace LogiMarket.Domain.Operaciones;
// Registro trazable del negocio; las restricciones de persistencia completan su consistencia.
public sealed class Auditoria
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Usuario { get; private set; } = "";
    public string Operacion { get; private set; } = "";
    public string Resultado { get; private set; } = "";
    public string Correlacion { get; private set; } = "";
    public DateTimeOffset Fecha { get; private set; } = DateTimeOffset.UtcNow;

    private Auditoria()
    {
    }

    public Auditoria(string usuario, string operacion, string resultado, string correlacion)
    {
        Usuario = usuario;
        Operacion = operacion;
        Resultado = resultado;
        Correlacion = correlacion;
    }
}
