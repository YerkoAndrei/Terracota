using Stride.Engine;
using Terracota;

var vSync = false;

// Primer inicio
if (!SistemaMemoria.ObtenerExistenciaArchivo())
{
    // 63 = barra de título
    var alto = 720; //(int)SystemParameters.FullPrimaryScreenHeight + 63;
    var ancho = 1280; //(int)SystemParameters.FullPrimaryScreenWidth;
    SistemaMemoria.EstablecerConfiguraciónPredeterminada(ancho, alto);
}
else
    vSync = bool.Parse(SistemaMemoria.ObtenerConfiguración(Constantes.Configuraciones.vSync));

// vSync
using var game = new Game();
game.IsDrawDesynchronized = !vSync;
game.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = vSync;
game.Run();
