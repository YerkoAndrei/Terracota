using Stride.Core.Mathematics;
using static Terracota.Constantes;

public class Bloque
{
    public TipoBloque tipoBloque { get; set; }
    public Vector3 posición { get; set; }
    public Quaternion rotación { get; set; }

    public Bloque (TipoBloque tipoBloque, Vector3 posición, Quaternion rotación)
    {
        this.tipoBloque = tipoBloque;
        this.posición = posición;
        this.rotación = rotación;
    }
}