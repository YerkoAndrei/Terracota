using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;

namespace Terracota;
using static Constantes;
using static Terracota.Constantes;

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

    private int estatuasAnfitrión;
    private int estatuasHuesped;
    private int maxEstatuas;

    private bool partidaActiva;

    public override async Task Execute()
    {
        // Predeterminado
        cañónAnfitrión.Activar(true);
        cañónHuesped.Activar(false);

        turnoJugador = TipoJugador.anfitrión;
        cañónActual = cañónAnfitrión;

        proyectilAnfitrión = TipoProyectil.bola;
        proyectilHuesped = TipoProyectil.bola;

        maxEstatuas = 3;

        partidaActiva = true;
        while (Game.IsRunning && partidaActiva)
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

        if (!partidaActiva)
            return;

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

    public void DesactivarEstatua(TipoJugador jugador)
    {
        if (jugador == TipoJugador.anfitrión)
        {
            controladorInterfaz.RestarAnfitrión(estatuasAnfitrión);
            estatuasAnfitrión++;
        }
        else
        {
            controladorInterfaz.RestarHuesped(estatuasHuesped);
            estatuasHuesped++;
        }
        VerificarPartida();
    }

    // PENDIENTE: Esto rompe el juego
    private void VerificarPartida()
    {
        var ganador = TipoJugador.nada;

        if (estatuasAnfitrión >= maxEstatuas)
            ganador = TipoJugador.huesped;
        else if(estatuasHuesped >= maxEstatuas)
            ganador = TipoJugador.anfitrión;

        if (ganador != TipoJugador.nada)
        {
            partidaActiva = false;
            //MirarGanador(ganador);
            controladorInterfaz.MostrarGanador(ganador);
        }
    }

    private async void MirarGanador(TipoJugador ganador)
    {
        if (ganador == TipoJugador.anfitrión && turnoJugador == TipoJugador.huesped)
            await MoverCámara(Quaternion.RotationY(MathUtil.DegreesToRadians(0)));
        if (ganador == TipoJugador.huesped && turnoJugador == TipoJugador.anfitrión)
            await MoverCámara(Quaternion.RotationY(MathUtil.DegreesToRadians(-180)));
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
