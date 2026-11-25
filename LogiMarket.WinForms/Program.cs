using LogiMarket.Infrastructure.InyeccionDependencias;
using LogiMarket.Application.Comprobantes;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.Eventos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Infrastructure.Documentos;
using LogiMarket.Infrastructure.Persistencia;
using LogiMarket.WinForms.Compartido;
using LogiMarket.WinForms.Formularios;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms;
// Composition root de WinForms: prepara la base local, DI, acceso y ciclo de sesion. No contiene reglas de venta.
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        // Composition root: las implementaciones concretas se eligen solo aqui.
        var ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogiMarket360Alan");
        Directory.CreateDirectory(ruta);
        var servicios = new ServiceCollection().AgregarLogiMarket(Path.Combine(ruta, "logimarket.db"));
        servicios.AddSingleton<ISalidaDocumento>(new SalidaArchivoTxt(Path.Combine(ruta, "comprobantes")));
        using var proveedor = servicios.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        try
        {
            using (var scope = proveedor.CreateScope())
                scope.ServiceProvider.GetRequiredService<Inicializador>().Ejecutar().GetAwaiter().GetResult();
            var operaciones = new OperacionesUi(proveedor);
            var sesion = proveedor.GetRequiredService<SesionActual>();
            while (true)
            {
                using var acceso = new FormularioAcceso(operaciones);
                if (acceso.ShowDialog() != DialogResult.OK)
                    break;
                using var principal = new FormularioPrincipal(operaciones, sesion, proveedor.GetRequiredService<MetricasSesion>(), proveedor.GetRequiredService<OpcionesDemostracion>(), proveedor.GetRequiredService<ISalidaDocumento>());
                System.Windows.Forms.Application.Run(principal);
                sesion.Cerrar();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.GetBaseException().Message, "Error de inicio", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
