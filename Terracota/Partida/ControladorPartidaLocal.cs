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

    public ControladorInterfaz controladorInterfaz;

    private ControladorCañon cañónActual;
    private bool cambiandoTurno;

    private TipoJugador turnoJugador;
    private TipoProyectil proyectilAnfitrión;
    private TipoProyectil proyectilHuesped;

    public override async Task Execute()
    {
        // Predeterminado
        cañónAnfitrión.Activar(true);
        cañónHuesped.Activar(false);

        turnoJugador = TipoJugador.anfitrión;
        cañónActual = cañónAnfitrión;

        proyectilAnfitrión = TipoProyectil.bola;
        proyectilHuesped = TipoProyectil.bola;

        while (Game.IsRunning)
        {
            if (Input.IsKeyPressed(Keys.Space) && !cambiandoTurno)
            {
                Disparar();
                CambiarTurno();
            }
            await Script.NextFrame();
        }
    }

    private void Disparar()
    {
        if (turnoJugador == TipoJugador.anfitrión)
            cañónActual.Disparar(proyectilAnfitrión);
        else
            cañónActual.Disparar(proyectilHuesped);
    }

    public TipoProyectil CambiarProyectil()
    {
        if (turnoJugador == TipoJugador.anfitrión)
        {
            switch (proyectilAnfitrión)
            {
                case TipoProyectil.bola:
                    proyectilAnfitrión = TipoProyectil.metralla;
                    break;
                case TipoProyectil.metralla:
                    proyectilAnfitrión = TipoProyectil.bola;
                    break;
            }
            return proyectilAnfitrión;
        }
        else
        {
            switch (proyectilHuesped)
            {
                case TipoProyectil.bola:
                    proyectilHuesped = TipoProyectil.metralla;
                    break;
                case TipoProyectil.metralla:
                    proyectilHuesped = TipoProyectil.bola;
                    break;
            }
            return proyectilHuesped;
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

            controladorInterfaz.PausarInterfaz();
            await MoverCámara(Quaternion.RotationY(MathUtil.DegreesToRadians(-180)));
            controladorInterfaz.CambiarInterfaz(turnoJugador, proyectilHuesped);
        }
        else
        {
            cañónAnfitrión.Activar(true);
            cañónHuesped.Activar(false);
            cañónActual = cañónAnfitrión;

            turnoJugador = TipoJugador.anfitrión;

            controladorInterfaz.PausarInterfaz();
            await MoverCámara(Quaternion.RotationY(MathUtil.DegreesToRadians(0)));
            controladorInterfaz.CambiarInterfaz(turnoJugador, proyectilAnfitrión);
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
