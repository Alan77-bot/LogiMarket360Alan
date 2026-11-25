using LogiMarket.Domain.Enums;

namespace LogiMarket.Domain.Entidades;
// El cliente conserva su categoria el descuento mayorista se decide por unidades no por una categoria adicional
public sealed class Cliente
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nombre { get; private set; } = "";
    public CategoriaCliente Categoria { get; private set; }
    public bool Activo { get; private set; } = true;

    private Cliente()
    {
    }

    public Cliente(string nombre, CategoriaCliente categoria) => Editar(nombre, categoria, true);
    public void Editar(string nombre, CategoriaCliente categoria, bool activo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        Nombre = nombre.Trim();
        Categoria = categoria;
        Activo = activo;
    }
}
