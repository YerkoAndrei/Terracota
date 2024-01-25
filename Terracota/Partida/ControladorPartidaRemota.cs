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

    public ControladorBola bolaAnfitrión;
    public List<ControladorBola> metrallaAnfitrión = new List<ControladorBola> { };

    public ControladorBola bolaHuesped;
    public List<ControladorBola> metrallaHuesped = new List<ControladorBola> { };

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
    private int cantidadBloques;
    private int cantidadMetralla;

    private InterfazElecciónRemota elección;
    private List<ElementoBloque> bloques;

    // PENDIENTE
    private static InterfazElecciónRemota elecciónEstática;
    private static TipoJugador cargaPendienteJugador;
    private static Fortaleza cargaPendienteFortaleza;

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
        elección.Inicializar();

        elecciónEstática = elección;
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
        cantidadBloques = bloques.Count;
        cantidadMetralla = metrallaAnfitrión.Count;

        // Cargas pendientes
        if(cargaPendienteJugador != TipoJugador.nada)
        {
            CargarFortaleza(cargaPendienteFortaleza, cargaPendienteJugador);
            cargaPendienteJugador = TipoJugador.nada;
            cargaPendienteFortaleza = null;
            elecciónEstática = null;
        }
    }

    public override void Update()
    {
        if (turnoJugador == TipoJugador.nada && !partidaActiva)
            return;

        // Entradas
        if (Input.IsKeyPressed(Keys.Space) && !interfaz.ObtenerPausa() && partidaActiva && !cambiandoTurno &&
            SistemaRed.ObtenerTipoJugador() == turnoJugador)
        {
            Disparar();

            // Cambia turno o espera cambio
            if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
                CambiarTurno();
            else
                EsperarCambioTurno();
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

        // Anfitrión controla físicas
        // Huesped solo controla su propio cañón
        SistemaRed.ActivarActualizaciónRed(true);

        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
        {
            // Activa colisiones
            fortalezaAnfitrión.Activar();
            fortalezaHuesped.Activar();

            // Turno inicial
            if (ganaAnfitrión)
                turnoJugador = TipoJugador.anfitrión;
            else
                turnoJugador = TipoJugador.huesped;

            var turno = new Turno
            {
                Jugador = turnoJugador,
                CantidadTurnos = 1
            };
            _ = SistemaRed.EnviarData(DataRed.cambioTurno, turno);
        }

        // Cañones
        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
        {
            cañónAnfitrión.Activar(true);
            cañónHuesped.Activar(false);

            controladorCámara.RotarYCámara(90, false, EnFinalizarCámara);
        }
        else
        {
            cañónAnfitrión.Activar(false);
            cañónHuesped.Activar(true);

            controladorCámara.RotarYCámara(90, true, EnFinalizarCámara);
        }
    }

    public void EnFinalizarCámara()
    {
        // Recarga interfaz
        interfaz.Activar(true);
        interfaz.ComenzarInterfazRemoto(SistemaRed.ObtenerTipoJugador(), TipoProyectil.bola);
        interfaz.MostrarInterfazRemoto(turnoJugador, cantidadTurnos, multiplicador);
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

        _ = SistemaRed.EnviarData(DataRed.disparo, proyectilActual);

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

    public async void CambiarTurno()
    {
        // Pausa turno
        cambiandoTurno = true;
        interfaz.OcultarInterfazRemoto();
        await Task.Delay(duraciónTurnoRemoto);

        // Verifica partida
        if (!partidaActiva)
        {
            cambiandoTurno = false;
            VerificarPartida();
            return;
        }

        controladorCámara.RotarLuz(luzDireccional);

        // Suma potencia cada de 4 turnos
        cantidadTurnos++;

        // Envia cambio
        var nuevoTurno = TipoJugador.anfitrión;
        if (turnoJugador == TipoJugador.anfitrión)
            nuevoTurno = TipoJugador.huesped;

        var turno = new Turno
        {
            Jugador = nuevoTurno,
            CantidadTurnos = cantidadTurnos
        };
        await SistemaRed.EnviarData(DataRed.cambioTurno, turno);

        // Actualiza turno
        ActualizarTurno(nuevoTurno, cantidadTurnos);
    }

    public void EsperarCambioTurno()
    {
        cambiandoTurno = true;
        interfaz.OcultarInterfazRemoto();
        controladorCámara.RotarLuz(luzDireccional);
    }

    public void ActualizarTurno(TipoJugador _turnoJugador, int _cantidadTurnos)
    {
        turnoJugador = _turnoJugador;
        cantidadTurnos = _cantidadTurnos;

        // Activa turno
        cambiandoTurno = false;
        interfaz.MostrarInterfazRemoto(turnoJugador, cantidadTurnos, CalcularPotencia(cantidadTurnos));
    }

    public float CalcularPotencia(int turnos)
    {
        // Aumenta cada 4 turnos
        if ((turnos - 1) % 4 == 0 && multiplicador < multiplicadorMáximo)
            multiplicador += 0.1f;
        
        return multiplicador;
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
        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
        {
            _ = SistemaRed.EnviarData(DataRed.estatua, jugador);
            VerificarPartida();
        }
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
            // Anfitrión envía data
            _ = SistemaRed.EnviarData(DataRed.finalizarPartida, ganador);
            MostrarGanador(ganador);
        }
    }

    public void MostrarGanador(TipoJugador ganador)
    {
        interfaz.MostrarGanador(ganador, cantidadTurnos);
        partidaActiva = false;
    }

    // Remoto
    public static bool VerificarIniciado()
    {
        // Por si remoto envia antes de que cargue escena
        return (elecciónEstática != null && elecciónEstática.ObtenerIniciada());
    }

    public static void GuardarFortalezaPendiente(Fortaleza fortaleza, TipoJugador tipoJugador)
    {
        cargaPendienteFortaleza = fortaleza;
        cargaPendienteJugador = tipoJugador;
    }

    public void CargarFortaleza(Fortaleza fortaleza, TipoJugador tipoJugador)
    {
        if (tipoJugador == TipoJugador.anfitrión)
            fortalezaAnfitrión.Inicializar(fortaleza, true);
        else
            fortalezaHuesped.Inicializar(fortaleza, false);

        elección.MostrarNombreFortaleza(fortaleza.Nombre, tipoJugador);
    }

    public void ActivarDisparo(TipoJugador tipoJugador, TipoProyectil proyectil)
    {
        SistemaSonido.SonarCañonazo();

        if (tipoJugador == TipoJugador.anfitrión)
            cañónAnfitrión.ActivarPartículas();
        else
            cañónHuesped.Disparar(proyectil, multiplicador);
    }

    public void RevisarJugadoresListos(TipoJugador tipoJugador)
    {
        elección.MostrarJugadorListo(tipoJugador);

        if (tipoJugador == TipoJugador.anfitrión)
            anfitriónListo = true;
        else
            huespedListo = true;

        if ((tipoJugador == TipoJugador.anfitrión && huespedListo) ||
            (tipoJugador == TipoJugador.huesped && anfitriónListo))
        {
            if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
                elección.ComenzarRuleta();
        }
    }

    public Físicas ObtenerFísicas()
    {
        var físicas = new Físicas
        {
            Bloques = new List<float[]>(),
            MetrallaAnfitrión = new List<float[]>(),
            MetrallaHuesped = new List<float[]>()
        };

        // Jugador anfitrión
        físicas.RotaciónCañónAnfitrión = ObtenerRotaciónCañón(TipoJugador.anfitrión);
        
        // Bloques
        for (int i = 0; i < cantidadBloques; i++)
        {
            físicas.Bloques.Add(bloques[i].ObtenerFísicas());
        }
        
        // Proyectiles
        físicas.BolaAnfitrión = bolaAnfitrión.ObtenerFísicas();
        físicas.BolaHuesped = bolaHuesped.ObtenerFísicas();
        
        for (int i = 0; i < cantidadMetralla; i++)
        {
            físicas.MetrallaAnfitrión.Add(metrallaAnfitrión[i].ObtenerFísicas());
            físicas.MetrallaHuesped.Add(metrallaHuesped[i].ObtenerFísicas());
        }

        return físicas;
    }

    public void ActualizarFísicas(Físicas físicas)
    {
        // Jugador huesped      
        ActualizarCañón(físicas.RotaciónCañónAnfitrión, TipoJugador.anfitrión);

        // Bloques
        for (int i = 0; i < cantidadBloques; i++)
        {
            bloques[i].ActualizarFísicas(físicas.Bloques[i]);
        }

        // Proyectiles
        bolaAnfitrión.ActualizarFísicas(físicas.BolaAnfitrión);
        bolaHuesped.ActualizarFísicas(físicas.BolaHuesped);

        for (int i = 0; i < cantidadMetralla; i++)
        {
            metrallaAnfitrión[i].ActualizarFísicas(físicas.MetrallaAnfitrión[i]);
            metrallaHuesped[i].ActualizarFísicas(físicas.MetrallaHuesped[i]);
        }
    }

    // Anfitrión actualiza su cañón junto con físicas
    // Huesped actualiza su cañón en llamado independiente  
    public float[] ObtenerRotaciónCañón(TipoJugador jugador)
    {
        if (jugador == TipoJugador.anfitrión)
            return cañónAnfitrión.ObtenerMatriz();
        else
            return cañónHuesped.ObtenerMatriz();
    }

    public void ActualizarCañón(float[] matriz, TipoJugador tipoJugador)
    {
        if (tipoJugador == TipoJugador.anfitrión)
            cañónAnfitrión.ActualizarRotación(matriz);
        else
            cañónHuesped.ActualizarRotación(matriz);
    }
}
