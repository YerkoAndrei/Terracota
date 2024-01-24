using System.Threading.Tasks;
using Stride.Engine;
using Stride.Input;

namespace Terracota;
using static Constantes;

public class ControladorPartidaLocal : SyncScript, IPartida
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
    private bool cambiarHaciaDerecha;

    private TipoJugador turnoJugador;
    private TipoProyectil proyectilAnfitrión;
    private TipoProyectil proyectilHuesped;

    private int estatuasAnfitrión;
    private int estatuasHuesped;
    private int maxEstatuas;
    private int cantidadTurnos;
    private float multiplicador;

    private bool partidaActiva;
    private bool esperandoReinicio;

    public override void Start()
    {
        // Predeterminado
        maxEstatuas = 3;
        cantidadTurnos = 1;
        multiplicador = 1.0f;
        turnoJugador = TipoJugador.nada;

        proyectilAnfitrión = TipoProyectil.bola;
        proyectilHuesped = TipoProyectil.bola;

        cañónAnfitrión.Iniciar(interfaz, TipoJugador.anfitrión);
        cañónHuesped.Iniciar(interfaz, TipoJugador.huesped);

        // Comienza con elección
        UIElección.Enabled = true;
        interfaz.Activar(false);
    }

    public override void Update()
    {
        if (turnoJugador == TipoJugador.nada && !partidaActiva)
            return;

        // Entradas
        if (Input.IsKeyPressed(Keys.Space) && !interfaz.ObtenerPausa() && partidaActiva && !cambiandoTurno)
        {
            Disparar();
            CambiarTurno();
        }
    }

    public void CargarFortaleza(string nombre, bool anfitrión)
    {
        var fortaleza = SistemaMemoria.ObtenerFortaleza(nombre);

        if(anfitrión)
            fortalezaAnfitrión.Inicializar(fortaleza, true);
        else
            fortalezaHuesped.Inicializar(fortaleza, false);
    }

    public void ComenzarPartida(bool ganaAnfitrión)
    {
        UIElección.Enabled = false;

        // Activa colisiones
        fortalezaAnfitrión.Activar();
        fortalezaHuesped.Activar();

        if (ganaAnfitrión)
        {
            cañónAnfitrión.Activar(true);
            cañónHuesped.Activar(false);

            turnoJugador = TipoJugador.anfitrión;
            cañónActual = cañónAnfitrión;

            cambiarHaciaDerecha = true;
            controladorCámara.RotarYCámara(90, false, EnFinalizarCámara);
        }
        else
        {
            cañónAnfitrión.Activar(false);
            cañónHuesped.Activar(true);

            turnoJugador = TipoJugador.huesped;
            cañónActual = cañónHuesped;

            cambiarHaciaDerecha = false;
            controladorCámara.RotarYCámara(90, true, EnFinalizarCámara);
        }
    }

    public void EnFinalizarCámara()
    {
        // Recarga interfaz
        interfaz.Activar(true);
        interfaz.MostrarInterfazLocal(turnoJugador, TipoProyectil.bola, cantidadTurnos, multiplicador);
        partidaActiva = true;
    }

    public void RotarXCámara(float tiempo)
    {
        controladorCámara.RotarXCámara(-40, tiempo);
    }

    private void Disparar()
    {
        controladorCámara.ActivarEfectoDisparo();
        SistemaSonido.SonarCañonazo();

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
        interfaz.OcultarInterfazLocal();
        await Task.Delay(duraciónTurnoLocal);

        // Verifica partida
        if (!partidaActiva)
        {
            cambiandoTurno = false;
            VerificarPartida();
            return;
        }

        cañónAnfitrión.Activar(false);
        cañónHuesped.Activar(false);
        controladorCámara.RotarLuz(luzDireccional);

        if (turnoJugador == TipoJugador.anfitrión)
        {
            controladorCámara.RotarYCámara(180, cambiarHaciaDerecha, () =>
            {
                cambiandoTurno = false;
                turnoJugador = TipoJugador.huesped;

                // Verifica partida por si se gana mientras gira
                if (!partidaActiva)
                {
                    VerificarPartida();
                    return;
                }

                cañónHuesped.Activar(true);
                cañónActual = cañónHuesped;

                interfaz.MostrarInterfazLocal(turnoJugador, proyectilAnfitrión, cantidadTurnos, multiplicador);
            });
        }
        else
        {
            controladorCámara.RotarYCámara(180, cambiarHaciaDerecha, () =>
            {
                cambiandoTurno = false;
                turnoJugador = TipoJugador.anfitrión;

                // Verifica partida por si se gana mientras gira
                if (!partidaActiva)
                {
                    VerificarPartida();
                    return;
                }

                cañónAnfitrión.Activar(true);
                cañónActual = cañónAnfitrión;

                interfaz.MostrarInterfazLocal(turnoJugador, proyectilAnfitrión, cantidadTurnos, multiplicador);
            });
        }

        // Suma potencia cada de 4 turnos
        cantidadTurnos++;
        SumarPotencia();
    }

    public void SumarPotencia()
    {
        // Aumenta cada 4 turnos
        if ((cantidadTurnos - 1) % 4 == 0 && multiplicador < multiplicadorMáximo)
            multiplicador += 0.1f;
    }

    public void DesactivarEstatua(TipoJugador jugador)
    {
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

        SistemaSonido.SonarEstatuaDesactivada();
        VerificarPartida();
    }

    private void VerificarPartida()
    {
        if (esperandoReinicio)
            return;

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
            // Evita animar de nuevo
            if (partidaActiva)
                interfaz.MostrarGanador(ganador, cantidadTurnos);

            partidaActiva = false;

            // Bloqueo final
            if (!cambiandoTurno && ganador == turnoJugador)
                esperandoReinicio = true;

            // En caso de que pierda el que tiene el turno
            if (!cambiandoTurno && ganador != turnoJugador)
            {
                esperandoReinicio = true;
                controladorCámara.RotarYCámara(180, cambiarHaciaDerecha);
            }
        }
    }
}
