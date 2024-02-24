using Stride.Engine;

namespace Terracota
{
    class TerracotaApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                var vSync = false;

                if (SistemaMemoria.ObtenerExistenciaArchivo())
                    vSync = bool.Parse(SistemaMemoria.ObtenerConfiguración(Constantes.Configuraciones.vSync));

                // vSync
                game.IsDrawDesynchronized = !vSync;
                game.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = vSync;

                game.Run();
            }
        }
    }
}
