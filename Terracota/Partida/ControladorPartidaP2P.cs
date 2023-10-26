using System.Threading.Tasks;
using Stride.Input;
using Stride.Engine;

namespace Terracota;
using static Constantes;

public class ControladorPartidaP2P : AsyncScript
{
    public ControladorCañón cañónAnfitrión;
    public ControladorCañón cañónHuesped;

    private ControladorCañón cañónActual;
    private TipoJugador turnoJugador;
    private bool cambiandoTurno;

    public override async Task Execute()
    {
        cañónAnfitrión.Activar(true);
        cañónHuesped.Activar(false);

        turnoJugador = TipoJugador.anfitrión;
        cañónActual = cañónAnfitrión;

        while (Game.IsRunning)
        {
            if (Input.IsKeyPressed(Keys.Z) && !cambiandoTurno)
            {
                cañónActual.Disparar(TipoProyectil.bola, 1);
                CambiarTurno();
            }

            if (Input.IsKeyPressed(Keys.X) && !cambiandoTurno)
            {
                cañónActual.Disparar(TipoProyectil.metralla, 1);
                CambiarTurno();
            }
            
            await Script.NextFrame();
        }
    }

    private async void CambiarTurno()
    {
        cambiandoTurno = true;
        await Task.Delay(duraciónTurno);

        if (turnoJugador == TipoJugador.anfitrión)
        {
            cañónAnfitrión.Activar(false);
            cañónHuesped.Activar(true);
            cañónActual = cañónHuesped;

            turnoJugador = TipoJugador.huesped;
        }
        else
        {
            cañónAnfitrión.Activar(true);
            cañónHuesped.Activar(false);
            cañónActual = cañónAnfitrión;

            turnoJugador = TipoJugador.anfitrión;
        }
        cambiandoTurno = false;
    }
}
