using LogiMarket.Application.Contratos;
using LogiMarket.Domain.Operaciones;
using System.Collections.Concurrent;

namespace LogiMarket.Application.Eventos;
// Patron Observer publica a cada suscriptor y comprueba recepciones para no repetir reacciones completadas
// Observer el publicador desconoce los concretos que atienden el evento
public sealed class PublicadorEventos(IEnumerable<ISuscriptorEvento> suscriptores, IRegistroRecepciones recepciones, IRepositorio repo)
{
    public async Task Publicar(EventoOutbox evento)
    {
        foreach (var suscriptor in suscriptores)
        {
            if (await recepciones.Recibido(evento.Id, suscriptor.Nombre))
                continue;
            await suscriptor.Recibir(evento);
            recepciones.Marcar(evento.Id, suscriptor.Nombre);
            await repo.Guardar();
        }
    }
}
