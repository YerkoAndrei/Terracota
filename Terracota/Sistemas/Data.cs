using System.Collections.Generic;
using Stride.Core.Mathematics;

namespace Terracota;
using static Constantes;

public class Conexión
{
    public string IP { get; set; }
    public TipoConexión TipoConexión { get; set; }
    public TipoJugador ConectarComo { get; set; }
}

public class Físicas
{
    /*
    public Vector3 PosiciónBola { get; set; }
    public Quaternion RotaciónBola { get; set; }
    */
    public RotaciónCañón RotaciónCañón { get; set; }    
    public List<BloqueFísico> Bloques { get; set; }
}

public class Turno
{
    public TipoJugador Jugador { get; set; }
    public int CantidadTurnos { get; set; }
}

public class RotaciónCañón
{
    public Quaternion TuboCañón { get; set; }
    public Quaternion SoporteCañón { get; set; }
}

public class BloqueFísico
{
    public Vector3 Posición { get; set; }
    public Quaternion Rotación { get; set; }

    public BloqueFísico(Vector3 posición, Quaternion rotación)
    {
        Posición = posición;
        Rotación = rotación;
    }
}

