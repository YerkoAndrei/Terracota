using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stride.Engine;
using Stride.Input;

namespace Terracota;
using static Constantes;

public class ControladorPartidaRemota : SyncScript, IPartida
{
    public ControladorCañón cañónAnfitrión;
    public ControladorCañón cañónHuesped;

    public ControladorFortaleza fortalezaAnfitrión;
    public ControladorFortaleza fortalezaHuesped;

    public ControladorCámara controladorCámara;
    public TransformComponent luzDireccional;

    public InterfazJuego interfaz;
    public UIComponent UIElección;

    private bool anfitriónListo;
    private bool huespedListo;
    private bool cambiandoTurno;

    private TipoJugador turnoJugador;
    private TipoProyectil proyectilActual;

    private int estatuasAnfitrión;
    private int estatuasHuesped;
    private int maxEstatuas;
    private int cantidadTurnos;
    private float multiplicador;

    private bool partidaActiva;

    private InterfazElecciónRemota elección;
    private List<ElementoBloque> bloques;

    public override void Start()
    {
        // Predeterminado
        maxEstatuas = 3;
        cantidadTurnos = 1;
        multiplicador = 1.0f;
        turnoJugador = TipoJugador.nada;

        proyectilActual = TipoProyectil.bola;
        
        switch (SistemaRed.ObtenerTipoJugador())
        {
            case TipoJugador.anfitrión:
                cañónAnfitrión.Iniciar(interfaz, TipoJugador.anfitrión);
                cañónHuesped.Iniciar(interfaz, TipoJugador.huesped);
                break;
            case TipoJugador.huesped:
                cañónHuesped.Iniciar(interfaz, TipoJugador.huesped);
                break;
        }

        // Comienza con elección
        elección = UIElección.Entity.Get<InterfazElecciónRemota>();
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
        if (Input.IsKeyPressed(Keys.Space) && !interfaz.ObtenerPausa() && partidaActiva && !cambiandoTurno && turnoJugador == SistemaRed.ObtenerTipoJugador())
        {
            Disparar();
            CambiarTurno();
        }
    }

    public void CargarFortaleza(string nombre, bool anfitrión)
    {
        var fortaleza = SistemaMemoria.ObtenerFortaleza(nombre);

        // Local
        if (anfitrión)
            fortalezaAnfitrión.Inicializar(fortaleza, true);
        else
            fortalezaHuesped.Inicializar(fortaleza, false);

        // Remoto
        _ = SistemaRed.EnviarData(DataRed.cargarFortaleza, fortaleza);
    }

    public void ComenzarPartida(bool ganaAnfitrión)
    {
        UIElección.Enabled = false;

        // Activa colisiones
        fortalezaAnfitrión.Activar();
        fortalezaHuesped.Activar();

        // Al finalizar rotación
        var enFin = () =>
        {
            // Recarga interfaz
            interfaz.Activar(true);
            interfaz.ActualizarTurno(cantidadTurnos, multiplicador);
            interfaz.CambiarInterfaz(turnoJugador, TipoProyectil.bola);
            partidaActiva = true;
        };

        // Turno inicial
        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
        {
            SistemaRed.ActivarActualizaciónFísicas(true);

            if (ganaAnfitrión)
            {
                turnoJugador = TipoJugador.anfitrión;
                _ = SistemaRed.EnviarData(DataRed.turnoAnfitrión);
            }
            else
            {
                turnoJugador = TipoJugador.huesped;
                _ = SistemaRed.EnviarData(DataRed.turnoHuesped);
            }
        }

        // Cañones
        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
        {
            cañónAnfitrión.Activar(true);
            cañónHuesped.Activar(false);

            controladorCámara.RotarYCámara(90, false, enFin);
        }
        else
        {
            cañónAnfitrión.Activar(false);
            cañónHuesped.Activar(true);

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

        _ = SistemaRed.EnviarData(DataRed.disparo);

        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
            cañónAnfitrión.Disparar(proyectilActual, multiplicador);
        else
            cañónHuesped.Disparar(proyectilActual, multiplicador);
    }

    public TipoProyectil CambiarProyectil()
    {
        switch (proyectilActual)
        {
            case TipoProyectil.bola:
                proyectilActual = TipoProyectil.metralla;
                break;
            case TipoProyectil.metralla:
                proyectilActual = TipoProyectil.bola;
                break;
        }
        return proyectilActual;
    }

    private async void CambiarTurno()
    {
        cambiandoTurno = true;
        interfaz.PausarInterfaz();
        await Task.Delay(duraciónTurnoRemoto);
        interfaz.ActivarTurno(false);
        cambiandoTurno = false;

        // Verifica partida
        if (!partidaActiva)
        {
            cambiandoTurno = false;
            VerificarPartida();
            return;
        }

        // Envia cambio
        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
        {
            if (turnoJugador == TipoJugador.anfitrión)
                _ = SistemaRed.EnviarData(DataRed.turnoHuesped);
            else
                _ = SistemaRed.EnviarData(DataRed.turnoAnfitrión);
        }

        // Rota solo luz
        //controladorCámara.RotarYCámara(180, cambiarHaciaDerecha, null, luzDireccional);

        // Actualiza turno
        if (turnoJugador == TipoJugador.anfitrión)
            ActualizarTurno(TipoJugador.huesped);
        else
            ActualizarTurno(TipoJugador.anfitrión);
    }

    public void ActualizarTurno(TipoJugador _turnoJugador)
    {
        turnoJugador = _turnoJugador;

        // Suma potencia cada de 4 turnos
        cantidadTurnos++;
        SumarPotencia();

        interfaz.CambiarInterfaz(turnoJugador, proyectilActual);
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
        if (!partidaActiva)
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
            interfaz.MostrarGanador(ganador, cantidadTurnos);
            partidaActiva = false;
        }
    }

    // Remoto
    public void CargarFortaleza(Fortaleza fortaleza, TipoJugador tipoJugador)
    {
        if (tipoJugador == TipoJugador.anfitrión)
            fortalezaAnfitrión.Inicializar(fortaleza, true);
        else
            fortalezaHuesped.Inicializar(fortaleza, false);

        elección.MostrarNombreFortaleza(fortaleza.Nombre, tipoJugador);
    }

    public void ActivarDisparo(TipoJugador tipoJugador)
    {
        SistemaSonido.SonarCañonazo();

        if (tipoJugador == TipoJugador.anfitrión)
            cañónAnfitrión.ActivarPartículas();
        else
            cañónHuesped.ActivarPartículas();
    }

    public void Pausar(TipoJugador tipoJugador)
    {

    }

    public void ActualizarTexto(Texto nuevoTexto)
    {

    }

    public bool RevisarJugadoresListos(TipoJugador tipoJugador)
    {
        elección.MostrarJugadorListo(tipoJugador);

        if (tipoJugador == TipoJugador.anfitrión)
            anfitriónListo = true;
        else
            huespedListo = true;

        if (tipoJugador == TipoJugador.anfitrión && huespedListo)
            return true;
        else if (tipoJugador == TipoJugador.huesped && anfitriónListo)
            return true;
        else
            return false;
    }

    public Físicas ObtenerFísicas()
    {
        var físicas = new Físicas();
        físicas.Bloques = new List<BloqueFísico>();

        // Cañón
        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
            físicas.RotaciónCañón = ObtenerRotaciónCañón();

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
        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.huesped)
            ActualizarCañón(físicas.RotaciónCañón, TipoJugador.anfitrión);

        // Bloques
        for (int i = 0; i < físicas.Bloques.Count; i++)
        {
            bloques[i].Posicionar(físicas.Bloques[i].Posición, físicas.Bloques[i].Rotación);
        }
    }

    public RotaciónCañón ObtenerRotaciónCañón()
    {
        var cañón = new RotaciónCañón();
        cañón.SoporteCañón = cañónHuesped.ObtenerRotaciónSoporte();
        cañón.TuboCañón = cañónHuesped.ObtenerRotaciónCañón();

        return cañón;
    }

    public void ActualizarCañón(RotaciónCañón rotaciónCañón, TipoJugador tipoJugador)
    {
        if (tipoJugador == TipoJugador.anfitrión)
            cañónAnfitrión.ActualizarRotación(rotaciónCañón.TuboCañón, rotaciónCañón.SoporteCañón);
        else
            cañónHuesped.ActualizarRotación(rotaciónCañón.TuboCañón, rotaciónCañón.SoporteCañón);
    }
}
