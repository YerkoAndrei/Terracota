using System.Runtime.InteropServices;
using Stride.Engine;

namespace Terracota;

class TerracotaApp
{
    static void Main(string[] args)
    {
        using (var game = new Game())
        {
            var primerInicio = !SistemaMemoria.ObtenerExistenciaArchivo();
            var vSync = false;

            if (!primerInicio)
                vSync = bool.Parse(SistemaMemoria.ObtenerConfiguración(Constantes.Configuraciones.vSync));
            else
            {
                var ancho = GetSystemMetrics(0);
                var alto = GetSystemMetrics(1);
                SistemaEscenas.GuardarPrimeraPantalla(ancho, alto);
            }

            // vSync
            game.IsDrawDesynchronized = !vSync;
            game.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = vSync;

            game.Run();
        }
    }

    [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    public static extern int GetSystemMetrics(int nIndex);
}
