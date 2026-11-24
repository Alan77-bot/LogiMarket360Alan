namespace LogiMarket.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        // PENDIENTE: configurar DI, inicializar la base y abrir el acceso.
        // La aplicacion todavia no muestra ventanas ni ejecuta operaciones.
    }
}