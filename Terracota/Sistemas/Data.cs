using System.Collections.Generic;
using Stride.Core.Mathematics;
using static Terracota.Constantes;

public class Conexión
{
    public string IP { get; set; }
    public TipoConexión TipoConexión { get; set; }
    public TipoJugador ConectarComo { get; set; }
}

public class Texto
{
    public string Emisor { get; set; }
    public string Tiempo { get; set; }
    public string Text { get; set; }
}

public class Físicas
{
    public Quaternion TuboCañón { get; set; }
    public Quaternion SoporteCañón { get; set; }
    /*
    public Vector3 PosiciónBola { get; set; }
    public Quaternion RotaciónBola { get; set; }
    */
    public List<BloqueFísico> Bloques { get; set; }
}

public class BloqueFísico
{
    public string Código { get; set; }
    public Vector3 Posición { get; set; }
    public Quaternion Rotación { get; set; }

    public BloqueFísico(string código, Vector3 posición, Quaternion rotación)
    {
        Código = código;
        Posición = posición;
        Rotación = rotación;
    }
}
