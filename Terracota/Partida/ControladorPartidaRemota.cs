using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stride.Engine;
using Stride.Input;

namespace Terracota;
using static Constantes;

public class ControladorPartidaRemota : SyncScript
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

    private List<ElementoBloque> bloques;

    public override void Start()
    {
        // Predeterminado
        maxEstatuas = 3;
        cantidadTurnos = 1;
        multiplicador = 1.0f;
        turnoJugador = TipoJugador.nada;

        proyectilAnfitrión = TipoProyectil.bola;
        proyectilHuesped = TipoProyectil.bola;

        switch (SistemaRed.ObtenerTipoJugador())
        {
            case TipoJugador.anfitrión:
                cañónAnfitrión.Iniciar(interfaz);
                break;
            case TipoJugador.huesped:
                cañónHuesped.Iniciar(interfaz);
                break;
        }

        // Comienza con elección
        UIElección.Enabled = true;
        interfaz.Activar(false);

        // Red
        bloques = new List<ElementoBloque>();

        // Físicas anfitrión
        var índice = 0;
        var bloquesAnfitrión = new List<ElementoBloque>();
        bloquesAnfitrión.AddRange(fortalezaAnfitrión.estatuas);
        bloquesAnfitrión.AddRange(fortalezaAnfitrión.cortos);
        bloquesAnfitrión.AddRange(fortalezaAnfitrión.largos);
        foreach (var bloque in bloquesAnfitrión)
        {
            bloque.CrearCódigo("0_" + índice);
            bloques.Add(bloque);
            índice++;
        }

        // Físicas huespes
        índice = 0;
        var bloquesHuesped = new List<ElementoBloque>();
        bloquesHuesped.AddRange(fortalezaHuesped.estatuas);
        bloquesHuesped.AddRange(fortalezaHuesped.cortos);
        bloquesHuesped.AddRange(fortalezaHuesped.largos);
        foreach (var bloque in bloquesHuesped)
        {
            bloque.CrearCódigo("1_" + índice);
            bloques.Add(bloque);
            índice++;
        }

        // Código es para ordenar una sola vez
        bloques = bloques.OrderBy(o => o.ObtenerCódigo()).ToList();
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

        if (anfitrión)
            fortalezaAnfitrión.Inicializar(fortaleza, true);
        else
            fortalezaHuesped.Inicializar(fortaleza, false);
    }

    public void ComenzarPartida(bool ganaAnfitrión)
    {
        UIElección.Enabled = false;

        // Activa colisiones
        //fortalezaAnfitrión.Activar();
        //fortalezaHuesped.Activar();

        // Al finalizar rotación
        var enFin = () =>
        {
            // Recarga interfaz
            interfaz.Activar(true);
            interfaz.ActualizarTurno(cantidadTurnos, multiplicador);
            interfaz.CambiarInterfaz(turnoJugador, TipoProyectil.bola);
            partidaActiva = true;
        };

        if (ganaAnfitrión)
        {
            cañónAnfitrión.Activar(true);
            cañónHuesped.Activar(false);

            turnoJugador = TipoJugador.anfitrión;
            cañónActual = cañónAnfitrión;

            cambiarHaciaDerecha = true;
            controladorCámara.RotarYCámara(90, false, enFin);
        }
        else
        {
            cañónAnfitrión.Activar(false);
            cañónHuesped.Activar(true);

            turnoJugador = TipoJugador.huesped;
            cañónActual = cañónHuesped;

            cambiarHaciaDerecha = false;
            controladorCámara.RotarYCámara(90, true, enFin);
        }
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
        interfaz.PausarInterfaz();
        await Task.Delay(duraciónTurnoLocal);
        interfaz.ActivarTurno(false);

        // Verifica partida
        if (!partidaActiva)
        {
            cambiandoTurno = false;
            VerificarPartida();
            return;
        }

        cañónAnfitrión.Activar(false);
        cañónHuesped.Activar(false);

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

                interfaz.CambiarInterfaz(turnoJugador, proyectilHuesped);
            }, luzDireccional);
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

                interfaz.CambiarInterfaz(turnoJugador, proyectilAnfitrión);
            }, luzDireccional);
        }

        // Suma potencia cada de 4 turnos
        cantidadTurnos++;
        SumarPotencia();

        interfaz.ActualizarTurno(cantidadTurnos, multiplicador);
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

    // Red
    public void CargarFortaleza(Fortaleza fortaleza, TipoJugador tipoJugador)
    {

    }

    public void CambiarTurno(TipoJugador tipoJugador)
    {

    }

    public void ActivarDisparo(TipoJugador tipoJugador)
    {

    }

    public void Pausar(TipoJugador tipoJugador)
    {

    }

    public void ActualizarTexto(Texto nuevoTexto)
    {

    }

    public Físicas ObtenerFísicas()
    {
        var físicas = new Físicas();

        // Cañón
        switch(SistemaRed.ObtenerTipoJugador())
        {
            case TipoJugador.anfitrión:
                físicas.SoporteCañón = cañónHuesped.ObtenerRotaciónSoporte();
                físicas.TuboCañón = cañónHuesped.ObtenerRotaciónCañón();
                break;
            case TipoJugador.huesped:
                físicas.SoporteCañón = cañónAnfitrión.ObtenerRotaciónSoporte();
                físicas.TuboCañón = cañónAnfitrión.ObtenerRotaciónCañón();
                break;
        }

        // Bloques
        foreach (var bloque in bloques)
        {
            físicas.Bloques.Add(new BloqueFísico(bloque.ObtenerCódigo(), bloque.Entity.Transform.Position, bloque.Entity.Transform.Rotation));
        }

        return físicas;
    }

    public void ActualizarFísicas(Físicas físicas)
    {
        // Cañón
        switch (SistemaRed.ObtenerTipoJugador())
        {
            case TipoJugador.anfitrión:
                 cañónHuesped.ActualizarRotación(físicas.TuboCañón, físicas.SoporteCañón);
                break;
            case TipoJugador.huesped:
                cañónAnfitrión.ActualizarRotación(físicas.TuboCañón, físicas.SoporteCañón);
                break;
        }

        // Bloques
        for (int i = 0; i < físicas.Bloques.Count; i++)
        {
            bloques[i].Posicionar(físicas.Bloques[i].Posición, físicas.Bloques[i].Rotación);
        }
    }
}
