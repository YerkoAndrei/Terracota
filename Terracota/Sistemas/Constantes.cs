using Stride.Core.Mathematics;

namespace Terracota;

public static class Constantes
{
    public static int duraciónTurno = 3000;
    public static float multiplicadorMáximo = 1.5f;

    // Color botones
    public static Color colorNormal = new Color(255, 255, 255, 255);
    public static Color colorEnCursor = new Color(200, 200, 200, 255);
    public static Color colorEnClic = new Color(155, 155, 155, 255);
    public static Color colorBloqueado = new Color(155, 155, 155, 155);

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

    public enum Estatua
    {
        chimpancé,
        orangután,
        gorila
    }
}
