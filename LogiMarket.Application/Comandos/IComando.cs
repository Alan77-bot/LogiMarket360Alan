namespace LogiMarket.Application.Comandos;
// Patron Command: representa una solicitud ejecutable y su entrada para la bitacora.
public interface IComando
{
    string Entrada { get; }

    Task Ejecutar();
}
