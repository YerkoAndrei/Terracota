using Stride.Core.Mathematics;
using System.Collections.Generic;
using static Terracota.Constantes;

public class Bloque
{
    public TipoBloque TipoBloque { get; set; }
    public Vector3 Posición { get; set; }
    public Quaternion Rotación { get; set; }

    public Bloque (TipoBloque tipoBloque, Vector3 posición, Quaternion rotación)
    {
        TipoBloque = tipoBloque;
        Posición = posición;
        Rotación = rotación;
    }
}

public class Fortaleza
{
    public string Nombre { get; set; }
    public string Fecha { get; set; }
    public List<Bloque> Bloques { get; set; }
}