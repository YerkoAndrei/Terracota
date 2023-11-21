using Stride.Core.Mathematics;
using System.Collections.Generic;

namespace Terracota;

public static class Constantes
{
    public static int duraciónTurno = 3000;
    public static float multiplicadorMáximo = 1.5f;

    // Color botones
    public static Color colorNormal = new Color(255, 255, 255, 255);
    public static Color colorEnCursor = new Color(200, 200, 200, 250);
    public static Color colorEnClic = new Color(155, 155, 155, 250);
    public static Color colorBloqueado = new Color(155, 155, 155, 155);

    // Paleta interfaz
    // Negro    #FA191919
    // Oscuro   #FFA03C00
    // Claro    #F0F0DCA0
    // Texto    #FF280000

    // Amarillo     #FFF0F064
    // Anfitrión    #FFC80027
    // Huesped      #FF2700C8

    // Tamaños fuentes
    // Predeterminado   25
    // En juego         30
    // Fotraleza        35
    // Títulos          45
    // Grande           60

    public enum Escenas
    {
        menú,
        creación,
        local,
        LAN,
        P2P
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

    public enum TipoEstatua
    {
        nada,
        chimpancé,
        gorila,
        orangután
    }

    public static Fortaleza GenerarFortalezaVacía()
    {
        var bloques = new List<Bloque>
        {
            new Bloque(TipoBloque.estatua, TipoEstatua.chimpancé, new Vector3(4,0,2), Quaternion.RotationY(MathUtil.DegreesToRadians(180))),
            new Bloque(TipoBloque.estatua, TipoEstatua.gorila, new Vector3(0,0, 2), Quaternion.RotationY(MathUtil.DegreesToRadians(180))),
            new Bloque(TipoBloque.estatua, TipoEstatua.orangután, new Vector3(-4,0,2), Quaternion.RotationY(MathUtil.DegreesToRadians(180))),

            new Bloque(TipoBloque.corto, TipoEstatua.nada, new Vector3(-6,    0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, TipoEstatua.nada, new Vector3(-4.5f, 0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, TipoEstatua.nada, new Vector3(-3,    0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, TipoEstatua.nada, new Vector3(-1.5f, 0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, TipoEstatua.nada, new Vector3(0,     0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, TipoEstatua.nada, new Vector3(1.5f,  0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, TipoEstatua.nada, new Vector3(3,     0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, TipoEstatua.nada, new Vector3(4.5f,  0,-2), Quaternion.Identity),
            new Bloque(TipoBloque.corto, TipoEstatua.nada, new Vector3(6,     0,-2), Quaternion.Identity),

            new Bloque(TipoBloque.largo, TipoEstatua.nada, new Vector3(-7.5f, 0,0), Quaternion.Identity),
            new Bloque(TipoBloque.largo, TipoEstatua.nada, new Vector3(-4.5f, 0,0), Quaternion.Identity),
            new Bloque(TipoBloque.largo, TipoEstatua.nada, new Vector3(-1.5f, 0,0), Quaternion.Identity),
            new Bloque(TipoBloque.largo, TipoEstatua.nada, new Vector3(1.5f,  0,0), Quaternion.Identity),
            new Bloque(TipoBloque.largo, TipoEstatua.nada, new Vector3(4.5f,  0,0), Quaternion.Identity),
            new Bloque(TipoBloque.largo, TipoEstatua.nada, new Vector3(7.5f,  0,0), Quaternion.Identity)
        };
        var fortaleza = new Fortaleza
        {
            Nombre = string.Empty,
            Fecha = "1996-02-08T00:00:00",
            Miniatura = string.Empty,
            Bloques = bloques
        };
        return fortaleza;
    }
}
