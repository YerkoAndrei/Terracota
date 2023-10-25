using Stride.Core.Mathematics;
using System.Collections.Generic;
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

public class Fortaleza
{
    public int ranura { get; set; }
    public string miniatura { get; set; }
    public List<Bloque> bloques { get; set; }
}