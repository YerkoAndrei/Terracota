using Stride.Core.Mathematics;
using System.Collections.Generic;

namespace Terracota;

public static class Constantes
{
    // Duración remota debe ser mitad de local
    public const int duraciónTurnoLocal = 3000;
    public const int duraciónTurnoRemoto = 1500;
    public const float multiplicadorMáximo = 1.5f;

    public enum Escenas
    {
        menú,
        creación,
        local,
        remoto
    }

    // Juego
    public enum TipoJugador
    {
        nada,
        anfitrión,
        huésped
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

    // Configuración
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
        vSync,
        volumenGeneral,
        volumenMúsica,
        volumenEfectos,
        velocidadRed,
        puertoRed,
        pantallaCompleta,
        resolución
    }
    
    public enum Calidades
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
        cerrar,

        // Partida
        iniciarPartida,
        finalizarPartida,
        cambioTurno,

        // Elección
        cargarFortaleza,
        jugadorListo,
        comenzarRuleta,
        finalizarRuleta,

        // Juego
        físicas,
        cañón,
        disparo,
        estatua,
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
