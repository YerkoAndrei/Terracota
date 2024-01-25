using System.Collections.Generic;

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
    public float[] RotaciónCañónAnfitrión { get; set; }

    public List<float[]> Bloques { get; set; }

    public float[] BolaAnfitrión { get; set; }
    public float[] BolaHuesped { get; set; }

    public List<float[]> MetrallaAnfitrión { get; set; }
    public List<float[]> MetrallaHuesped { get; set; }
}

public class Turno
{
    public TipoJugador Jugador { get; set; }
    public int CantidadTurnos { get; set; }
}

// Matriz:
// 0: Posición X
// 1: Posición Y
// 2: Posición Z
// 3: Rotación X
// 4: Rotación Y
// 5: Rotación Z
// 6: Rotación W
// 7: Escala X (Opcional)
// 8: Escala Y (Opcional)
// 9: Escala Z (Opcional)

// Matriz cañón:
// 0: Rotación tubo X
// 1: Rotación tubo Y
// 2: Rotación tubo Z
// 3: Rotación tubo W
// 4: Rotación soporte X
// 5: Rotación soporte Y
// 6: Rotación soporte Z
// 7: Rotación soporte W
