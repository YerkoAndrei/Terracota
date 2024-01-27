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
    public float[] BolaHuésped { get; set; }

    public List<float[]> MetrallaAnfitrión { get; set; }
    public List<float[]> MetrallaHuésped { get; set; }
}

public class Turno
{
    public TipoJugador Jugador { get; set; }
    public int CantidadTurnos { get; set; }
}

// Matriz:
// 0: Fuerza sonido
// 1: Posición X
// 2: Posición Y
// 3: Posición Z
// 4: Rotación X
// 5: Rotación Y
// 6: Rotación Z
// 7: Rotación W
// 8: Escala X (Opcional)
// 9: Escala Y (Opcional)
// 10: Escala Z (Opcional)

// Matriz cañón:
// 0: Rotación cañón X
// 1: Rotación cañón Y
// 2: Rotación cañón Z
// 3: Rotación cañón W
// 4: Rotación soporte X
// 5: Rotación soporte Y
// 6: Rotación soporte Z
// 7: Rotación soporte W
