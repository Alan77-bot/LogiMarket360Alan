namespace LogiMarket.Application.Validaciones;
// Chain: el orden cliente, lineas, stock y estado determina el primer mensaje de rechazo.
public static class CadenaConfirmacion
{
    public static ResultadoValidacion Validar(ContextoValidacion c, bool exigirValidado = true)
    {
        var primero = new ValidadorCliente();
        var ultimo = primero.Luego(new ValidadorLineas()).Luego(new ValidadorStock());
        if (exigirValidado)
            ultimo.Luego(new ValidadorEstado());
        return primero.Validar(c);
    }
}
