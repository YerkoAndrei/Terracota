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
    public TransformComponent luzDireccional;

    public ControladorInterfaz controladorInterfaz;
    public ControladorCámara controladorCámara;

    private ControladorCañon cañónActual;
    private bool cambiandoTurno;

    private TipoJugador turnoJugador;
    private TipoProyectil proyectilAnfitrión;
    private TipoProyectil proyectilHuesped;

    private int estatuasAnfitrión;
    private int estatuasHuesped;
    private int maxEstatuas;
    private int cantidadTurnos;
    private float multiplicador;

    private bool partidaActiva;
    private bool sumarMultiplicador;

    public override async Task Execute()
    {
        // Predeterminado
        cañónAnfitrión.Activar(true);
        cañónHuesped.Activar(false);

        cañónAnfitrión.Asignar(controladorInterfaz);
        cañónHuesped.Asignar(controladorInterfaz);

        turnoJugador = TipoJugador.anfitrión;
        cañónActual = cañónAnfitrión;

        proyectilAnfitrión = TipoProyectil.bola;
        proyectilHuesped = TipoProyectil.bola;

        maxEstatuas = 3;
        cantidadTurnos = 1;
        multiplicador = 1.0f;

        controladorInterfaz.ActualizarTurno(cantidadTurnos, multiplicador);

        partidaActiva = true;
        while (Game.IsRunning)
        {
            if (Input.IsKeyPressed(Keys.Space) && !controladorInterfaz.ObtenerPausa()
                && partidaActiva && !cambiandoTurno)
            {
                Disparar();
                CambiarTurno();
            }
            await Script.NextFrame();
        }
    }

    private void Disparar()
    {
        controladorCámara.ActivarEfectoDisparo();

        if (turnoJugador == TipoJugador.anfitrión)
            cañónActual.Disparar(proyectilAnfitrión, multiplicador);
        else
            cañónActual.Disparar(proyectilHuesped, multiplicador);
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
        controladorInterfaz.PausarInterfaz();
        await Task.Delay(duraciónTurno);
        controladorInterfaz.ActivarTurno(false);

        if (!partidaActiva)
            return;

        cañónAnfitrión.Activar(false);
        cañónHuesped.Activar(false);

        if (turnoJugador == TipoJugador.anfitrión)
        {
            await MoverCámara(180, true);

            cañónHuesped.Activar(true);
            cañónActual = cañónHuesped;

            turnoJugador = TipoJugador.huesped;
            controladorInterfaz.CambiarInterfaz(turnoJugador, proyectilHuesped);
        }
        else
        {
            await MoverCámara(0, true);
            cañónAnfitrión.Activar(true);
            cañónActual = cañónAnfitrión;

            turnoJugador = TipoJugador.anfitrión;
            controladorInterfaz.CambiarInterfaz(turnoJugador, proyectilAnfitrión);

            // Suma potencia despues de 4 turnos
            SumarPotencia();
        }

        cantidadTurnos++;
        controladorInterfaz.ActualizarTurno(cantidadTurnos, multiplicador);
    }

    public void SumarPotencia()
    {
        if (sumarMultiplicador && multiplicador < multiplicadorMáximo)
            multiplicador += 0.1f;

        sumarMultiplicador = !sumarMultiplicador;
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

    private void VerificarPartida()
    {
        var ganador = TipoJugador.nada;

        if (estatuasAnfitrión >= maxEstatuas)
        {
            ganador = TipoJugador.huesped;
            cañónAnfitrión.Activar(false);
            cañónHuesped.Activar(true);
        }
        else if (estatuasHuesped >= maxEstatuas)
        {
            ganador = TipoJugador.anfitrión;
            cañónAnfitrión.Activar(true);
            cañónHuesped.Activar(false);
        }

        // Muestra ganador
        if (ganador != TipoJugador.nada)
        {
            partidaActiva = false;
            MirarGanador(ganador);
            controladorInterfaz.MostrarGanador(ganador, cantidadTurnos);
        }
    }

    private async void MirarGanador(TipoJugador ganador)
    {
        // En caso de que pierda el que tiene el turno
        if (ganador == TipoJugador.anfitrión && turnoJugador == TipoJugador.huesped)
            await MoverCámara(0, true);
        if (ganador == TipoJugador.huesped && turnoJugador == TipoJugador.anfitrión)
            await MoverCámara(180, true);
    }

    private async Task MoverCámara(float YObjetivo, bool derecha)
    {
        float duraciónLerp = 1f;
        float tiempoLerp = 0;
        float tiempo = 0;

        var rotaciónInicial = ejeCámara.Rotation;
        var rotaciónObjetivo = Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo));

        // Ajusta dirección de movimiento
        var direcciónObjetivo = rotaciónObjetivo;
        if(derecha)
            direcciónObjetivo = Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo - 0.01f));
        else
            direcciónObjetivo = Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo + 0.01f));

        while (tiempoLerp < duraciónLerp)
        {
            tiempo = tiempoLerp / duraciónLerp;
            ejeCámara.Rotation = Quaternion.Lerp(rotaciónInicial, direcciónObjetivo, tiempo);

            // Mueve sol 45º
            luzDireccional.Rotation *= Quaternion.RotationY(0.005f);

            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Script.NextFrame();
        }

        // Fin
        ejeCámara.Rotation = rotaciónObjetivo;
        cambiandoTurno = false;
    }
}
