using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;

namespace Terracota;
using static Constantes;

public class ControladorPartidaLocal : AsyncScript
{
    public ControladorCañon cañónAnfitrión;
    public ControladorCañon cañónHuesped;

    public TransformComponent ejeCámara;

    private ControladorCañon cañónActual;
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
            // pruebas
            if (Input.IsKeyPressed(Keys.Z) && !cambiandoTurno)
            {
                cañónActual.Disparar(TipoProyectil.bola);
                CambiarTurno();
            }

            if (Input.IsKeyPressed(Keys.X) && !cambiandoTurno)
            {
                cañónActual.Disparar(TipoProyectil.metralla);
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
            await MoverCámara(Quaternion.RotationY(MathUtil.DegreesToRadians(-180)));
        }
        else
        {
            cañónAnfitrión.Activar(true);
            cañónHuesped.Activar(false);
            cañónActual = cañónAnfitrión;

            turnoJugador = TipoJugador.anfitrión;
            await MoverCámara(Quaternion.RotationY(MathUtil.DegreesToRadians(0)));
        }
    }

    // PENDIENTE: mejorar movimiento cuando entienda mejor las rotaciones
    private async Task MoverCámara(Quaternion rotaciónObjetivo)
    {
        float duraciónLerp = 1.5f;
        float tiempoLerp = 0;
        float tiempo = 0;

        var rotaciónInicial = ejeCámara.Rotation;

        while (tiempoLerp < duraciónLerp)
        {
            tiempo = tiempoLerp / duraciónLerp;
            ejeCámara.Rotation = Quaternion.Lerp(rotaciónInicial, rotaciónObjetivo, tiempo);

            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Script.NextFrame();
        }

        // Fin
        ejeCámara.Rotation = rotaciónObjetivo;
        cambiandoTurno = false;
    }
}
