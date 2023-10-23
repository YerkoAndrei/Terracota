using Stride.Core.Mathematics;
using System.Text;

namespace Terracota;

public static class Constantes
{
    public static int duraciónTurno = 3000;
    public static float multiplicadorMáximo = 1.5f;

    private static int llave = 08021996;

    // Color botones
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

    // XOR
    public static string DesEncriptar(string texto)
    {
        var entrada = new StringBuilder(texto);
        var salida = new StringBuilder(texto.Length);
        char c;

        for (int i = 0; i < texto.Length; i++)
        {
            c = entrada[i];
            c = (char)(c ^ llave);
            salida.Append(c);
        }
        return salida.ToString();
    }
}
