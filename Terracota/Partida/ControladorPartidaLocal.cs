using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;

namespace Terracota;
using static Constantes;

public class ControladorPartidaLocal : AsyncScript
{
    public ControladorCañón cañónAnfitrión;
    public ControladorCañón cañónHuesped;

    public ControladorFortaleza fortalezaAnfitrión;
    public ControladorFortaleza fortalezaHuesped;

    public ControladorCámara controladorCámara;
    public TransformComponent luzDireccional;

    public InterfazJuego interfaz;
    public UIComponent UIElección;

    private ControladorCañón cañónActual;
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
        maxEstatuas = 3;
        cantidadTurnos = 1;
        multiplicador = 1.0f;
        turnoJugador = TipoJugador.nada;

        // Comienza con elección
        UIElección.Enabled = true;
        interfaz.Activar(false);

        // Espera que partida inicie
        while (turnoJugador == TipoJugador.nada && !partidaActiva)
        {
            await Script.NextFrame();
        }

        // Start
        ComenzarPartida();
        while (Game.IsRunning)
        {
            // Entradas
            if (Input.IsKeyPressed(Keys.Space) && !interfaz.ObtenerPausa() && partidaActiva && !cambiandoTurno)
            {
                Disparar();
                CambiarTurno();
            }
            await Script.NextFrame();
        }
    }

    public void CargarFortaleza(int ranura, bool anfitrión)
    {
        var fortaleza = SistemaMemoria.ObtenerFortaleza(ranura);

        if(anfitrión)
            fortalezaAnfitrión.Inicializar(fortaleza, true);
        else
            fortalezaHuesped.Inicializar(fortaleza, false);
    }

    public void AsignarTurno(bool ganaAnfitrión)
    {
        if (ganaAnfitrión)
            turnoJugador = TipoJugador.anfitrión;
        else
            turnoJugador = TipoJugador.huesped;
    }

    private async void ComenzarPartida()
    {
        UIElección.Enabled = false;

        proyectilAnfitrión = TipoProyectil.bola;
        proyectilHuesped = TipoProyectil.bola;

        cañónAnfitrión.Asignar(interfaz);
        cañónHuesped.Asignar(interfaz);

        if (turnoJugador == TipoJugador.anfitrión)
        {
            cañónAnfitrión.Activar(true);
            cañónHuesped.Activar(false);

            turnoJugador = TipoJugador.anfitrión;
            cañónActual = cañónAnfitrión;

            await controladorCámara.RotarCámara(-90, false);
        }
        else
        {
            cañónAnfitrión.Activar(false);
            cañónHuesped.Activar(true);

            turnoJugador = TipoJugador.huesped;
            cañónActual = cañónHuesped;

            await controladorCámara.RotarCámara(90, true);
        }

        // Recarga interfaz
        interfaz.Activar(true);
        interfaz.ActualizarTurno(cantidadTurnos, multiplicador);
        partidaActiva = true;
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
        interfaz.PausarInterfaz();
        await Task.Delay(duraciónTurno);
        interfaz.ActivarTurno(false);

        if (!partidaActiva)
            return;

        cañónAnfitrión.Activar(false);
        cañónHuesped.Activar(false);

        if (turnoJugador == TipoJugador.anfitrión)
        {
            await controladorCámara.RotarCámara(180, true, luzDireccional);
            cambiandoTurno = false;

            cañónHuesped.Activar(true);
            cañónActual = cañónHuesped;

            turnoJugador = TipoJugador.huesped;
            interfaz.CambiarInterfaz(turnoJugador, proyectilHuesped);
        }
        else
        {
            await controladorCámara.RotarCámara(180, true, luzDireccional);
            cambiandoTurno = false;

            cañónAnfitrión.Activar(true);
            cañónActual = cañónAnfitrión;

            turnoJugador = TipoJugador.anfitrión;
            interfaz.CambiarInterfaz(turnoJugador, proyectilAnfitrión);

            // Suma potencia despues de 4 turnos
            SumarPotencia();
        }

        cantidadTurnos++;
        interfaz.ActualizarTurno(cantidadTurnos, multiplicador);
    }

    public void SumarPotencia()
    {
        if (sumarMultiplicador && multiplicador < multiplicadorMáximo)
            multiplicador += 0.1f;

        sumarMultiplicador = !sumarMultiplicador;
    }

    public void DesactivarEstatua(TipoJugador jugador)
    {/*
        if (jugador == TipoJugador.anfitrión)
        {
            interfaz.RestarAnfitrión(estatuasAnfitrión);
            estatuasAnfitrión++;
        }
        else
        {
            interfaz.RestarHuesped(estatuasHuesped);
            estatuasHuesped++;
        }
        VerificarPartida();*/
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
            interfaz.MostrarGanador(ganador, cantidadTurnos);
        }
    }

    private async void MirarGanador(TipoJugador ganador)
    {
        // En caso de que pierda el que tiene el turno
        if (ganador == TipoJugador.anfitrión && turnoJugador == TipoJugador.huesped)
            await controladorCámara.RotarCámara(0, true);
        if (ganador == TipoJugador.huesped && turnoJugador == TipoJugador.anfitrión)
            await controladorCámara.RotarCámara(180, true);
    }
}
