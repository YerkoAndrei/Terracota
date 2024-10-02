using Stride.Engine;
using Terracota;

using var game = new Game();

// vSync
var vSync = false;
if (SistemaMemoria.ObtenerExistenciaArchivo())
    vSync = bool.Parse(SistemaMemoria.ObtenerConfiguración(Constantes.Configuraciones.vSync));

game.IsDrawDesynchronized = !vSync;
game.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = vSync;

game.Run();

