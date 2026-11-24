namespace LogiMarket.Domain.Pedidos;
// Patron Memento: conservamos copias completas para deshacer y rehacer. Un pago no se puede deshacer aqui.
// Memento: cada entrada es una copia profunda; el historial nunca revierte pagos.
public sealed class HistorialBorrador
{
    private readonly List<Pedido?> estados = [null ];
    private int posicion;
    private bool bloqueado;
    public void Capturar(Pedido pedido)
    {
        Exigir();
        pedido.ExigirEditable();
        Agregar(pedido.ClonarBorrador());
    }

    public void CapturarVacio()
    {
        Exigir();
        Agregar(null);
    }

    private void Agregar(Pedido? pedido)
    {
        estados.RemoveRange(posicion + 1, estados.Count - posicion - 1);
        estados.Add(pedido);
        if (estados.Count > 5)
            estados.RemoveAt(0);
        posicion = estados.Count - 1;
    }

    public Pedido? DeshacerCompleto()
    {
        Exigir();
        if (posicion <= 0)
            throw new InvalidOperationException("No hay cambios para deshacer.");
        return estados[--posicion]?.ClonarBorrador();
    }

    public Pedido? RehacerCompleto()
    {
        Exigir();
        if (posicion >= estados.Count - 1)
            throw new InvalidOperationException("No hay cambios para rehacer.");
        return estados[++posicion]?.ClonarBorrador();
    }

    public Pedido Deshacer() => DeshacerCompleto() ?? throw new InvalidOperationException("El carrito restaurado esta vacio.");
    public Pedido Rehacer() => RehacerCompleto() ?? throw new InvalidOperationException("El carrito restaurado esta vacio.");
    public void Bloquear()
    {
        estados.Clear();
        posicion = -1;
        bloqueado = true;
    }

    private void Exigir()
    {
        if (bloqueado)
            throw new InvalidOperationException("El pedido confirmado no admite deshacer.");
    }
}
