namespace LogiMarket.Application.Contratos;
// Responsabilidad de IHashContrasena: colaborar desde LogiMarket.Application.Contratos sin mezclar reglas, persistencia y pantallas.
public interface IHashContrasena
{
    string Crear(string clave);
    bool Verificar(string clave, string hash);
}
