using Stride.Core.Mathematics;
using System.Collections.Generic;

namespace Terracota;

public static class Constantes
{
    public const int duraciónTurnoLocal = 3000;
    public const int duraciónTurnoRemoto = 1500;
    public const float multiplicadorMáximo = 1.5f;

    // Color botones
    public static Color colorNormal = new Color(255, 255, 255, 255);
    public static Color colorEnCursor = new Color(200, 200, 200, 250);
    public static Color colorEnClic = new Color(155, 155, 155, 250);
    public static Color colorBloqueado = new Color(155, 155, 155, 155);

    // Paleta interfaz
    // Negro    #FA191919
    // Oscuro   #FFA03C00
    // Claro    #F0F0DCA0
    // Texto    #FF191414

    // Amarillo     #FFF0F064
    // Anfitrión    #FFC80027
    // Huesped      #FF2700C8

    // Tamaños fuentes
    // Predeterminado   25
    // En juego         30
    // Títulos          40
    // Grande           60

    public enum Escenas
    {
        menú,
        creación,
        local,
        remoto
    }

    public enum TipoJugador
    {
        nada,
        anfitrión,
        huesped
    }

    public enum TipoProyectil
    {
        bola,
        metralla
    }

    public enum TipoBloque
    {
        nada,
        estatua,
        corto,
        largo
    }

    public enum TipoCurva
    {
        nada,
        suave,
        rápida
    }

    public enum Direcciones
    {
        arriba,
        abajo,
        izquierda,
        derecha
    }

    public enum Idiomas
    {
        sistema,
        español,
        inglés
    }

    public enum Configuraciones
    {
        idioma,
        gráficos,
        sombras,
        volumenGeneral,
        volumenMúsica,
        volumenEfectos,
        velocidadRed,
        puertoRed,
        pantallaCompleta,
        resolución
    }
    
    public enum NivelesConfiguración
    {
        bajo,
        medio,
        alto
    }

    public enum TipoConexión
    {
        local,
        global
    }

    public enum DataRed
    {
        nada,
        conectar,
        iniciarPartida,
        cerrar,

        // Elección
        cargarFortaleza,
        anfitriónListo,
        huespedListo,
        comenzarRuleta,
        finalizarRuleta,

        // Turnos
        cambioTurno,
        finalizarPartida,  // pendiente

        // Juego
        físicas,
        cañón,
        disparo,
        estatua,  // pendiente
        pausa,  // pendiente
    }

    public static Fortaleza GenerarFortalezaVacía()
    {
        var bloques = new List<Bloque>
        {
            new Bloque(TipoBloque.estatua, new Vector3(4,0,2), Quaternion.RotationY(MathUtil.DegreesToRadians(180))),
            new Bloque(TipoBloque.estatua, new Vector3(0,0, 2), Quaternion.RotationY(MathUtil.DegreesToRadians(180))),
            new Bloque(TipoBloque.estatua, new Vector3(-4,0,2), Quaternion.RotationY(MathUtil.DegreesToRadians(180))),

            new Bloque(TipoBloque.corto, new Vector3(-6,    0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, new Vector3(-4.5f, 0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, new Vector3(-3,    0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, new Vector3(-1.5f, 0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, new Vector3(0,     0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, new Vector3(1.5f,  0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, new Vector3(3,     0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, new Vector3(4.5f,  0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, new Vector3(6,     0,-2), Quaternion.Identity),

            new Bloque(TipoBloque.largo, new Vector3(-7.5f, 0,0), Quaternion.Identity),
            new Bloque(TipoBloque.largo, new Vector3(-4.5f, 0,0), Quaternion.Identity),
            new Bloque(TipoBloque.largo, new Vector3(-1.5f, 0,0), Quaternion.Identity),
            new Bloque(TipoBloque.largo, new Vector3(1.5f,  0,0), Quaternion.Identity),
            new Bloque(TipoBloque.largo, new Vector3(4.5f,  0,0), Quaternion.Identity),
            new Bloque(TipoBloque.largo, new Vector3(7.5f,  0,0), Quaternion.Identity)
        };
        var fortaleza = new Fortaleza
        {
            Nombre = string.Empty,
            Fecha = "1996-02-08T00:00:00",
            Bloques = bloques
        };
        return fortaleza;
    }
}
