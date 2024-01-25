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
    public RotaciónCañón RotaciónCañónAnfitrión { get; set; }
    public List<float[]> Bloques { get; set; }

    public float[] BolaAnfitrión { get; set; }
    public List<float[]> MetrallaAnfitrión { get; set; }

    public float[] BolaHuesped { get; set; }
    public List<float[]> MetrallaHuesped { get; set; }
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

// Matriz:
// 0: Posición X
// 1: Posición Y
// 2: Posición Z
// 3: Rotación X
// 4: Rotación Y
// 5: Rotación Z
// 6: Rotación W

// 7: Escala X
// 8: Escala Y
// 9: Escala Z
