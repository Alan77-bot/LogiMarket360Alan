using LogiMarket.Application.Eventos;
using LogiMarket.Domain.Pedidos;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Compartido;
// Patron Mediator en presentacion las vistas se registran y comunican cambios sin referencias directas entre ellas
// Mediator las vistas se registran y nunca llaman a otras vistas directamente
public sealed class FormularioPrincipalMediator : LogiMarket.Application.Eventos.MediadorVistas
{
    public event Func<LogiMarket.Domain.Pedidos.Pedido, Task>? BorradorSolicitado;
    public Task EditarBorrador(LogiMarket.Domain.Pedidos.Pedido pedido) => BorradorSolicitado?.Invoke(pedido) ?? Task.CompletedTask;
}
