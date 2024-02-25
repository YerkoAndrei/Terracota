using Stride.Engine;
using System.Windows;

namespace Terracota
{
    class TerracotaApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                var vSync = false;

                // Primer inicio
                if (!SistemaMemoria.ObtenerExistenciaArchivo())
                {
                    var alto = (int)SystemParameters.FullPrimaryScreenHeight;
                    var ancho = (int)SystemParameters.FullPrimaryScreenWidth;
                    SistemaMemoria.EstablecerConfiguraciónPredeterminada(ancho, alto);
                }
                else
                    vSync = bool.Parse(SistemaMemoria.ObtenerConfiguración(Constantes.Configuraciones.vSync));

                // vSync
                game.IsDrawDesynchronized = !vSync;
                game.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = vSync;

                game.Run();
            }
        }
    }
}
