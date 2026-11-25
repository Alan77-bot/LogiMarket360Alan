using System.Collections;

namespace LogiMarket.Domain.Catalogo;
// Patron Iterator recorre las hojas con sus cantidades sin exponer como esta organizado el arbol
// Iterator pila propia orden estable y cantidades propagadas sin exponer listas
public sealed class IteradorFisico : IEnumerator<ComponenteFisico>
{
    private readonly Stack<(IElementoCatalogo Elemento, int Cantidad)> pendientes = new();
    public ComponenteFisico Current { get; private set; } = null !;

    object IEnumerator.Current => Current;

    public IteradorFisico(IElementoCatalogo raiz, int cantidad = 1)
    {
        if (cantidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(cantidad));
        pendientes.Push((raiz, cantidad));
    }

    public bool MoveNext()
    {
        while (pendientes.TryPop(out var item))
        {
            if (item.Elemento is ProductoSimple producto)
            {
                Current = new(producto.Sku, item.Cantidad, producto.Precio, producto.Peso);
                return true;
            }

            var kit = (KitCompuesto)item.Elemento;
            foreach (var hijo in kit.Componentes.Reverse())
                pendientes.Push((hijo.Elemento, checked(hijo.Cantidad * item.Cantidad)));
        }

        return false;
    }

    public void Reset() => throw new NotSupportedException("Cree otro iterador.");
    public void Dispose() => pendientes.Clear();
}
