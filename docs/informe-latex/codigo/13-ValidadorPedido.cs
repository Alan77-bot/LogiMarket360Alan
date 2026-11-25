namespace LogiMarket.Application.Validaciones;
// Patron Chain of Responsibility procesa una regla y delega en la siguiente solo si puede continuar
// Chain of Responsibility se detiene en la primera causa verificable
public abstract class ValidadorPedido
{
    private ValidadorPedido? siguiente;
    public ValidadorPedido Luego(ValidadorPedido valor)
    {
        siguiente = valor;
        return valor;
    }

    protected abstract ResultadoValidacion Revisar(ContextoValidacion c);
    public ResultadoValidacion Validar(ContextoValidacion c)
    {
        var resultado = Revisar(c);
        return resultado.Valido ? siguiente?.Validar(c) ?? resultado : resultado;
    }
}
