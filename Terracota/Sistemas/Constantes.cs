using Stride.Core.Mathematics;

namespace Terracota;

public static class Constantes
{
    public static int duraciónTurno = 3000;
    public static float multiplicadorMáximo = 1.5f;

    public static Color colorNormal = new Color(255, 255, 255, 255);
    public static Color colorEnCursor = new Color(200, 200, 200, 255);
    public static Color colorEnClic = new Color(155, 155, 155, 255);

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

    public enum Estatua
    {
        chimpancé,
        orangután,
        gorila
    }
}
