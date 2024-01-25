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
    public List<BloqueFísico> Bloques { get; set; }

    public ProyectilFísico BolaAnfitrión { get; set; }
    public List<ProyectilFísico> MetrallaAnfitrión { get; set; }

    public ProyectilFísico BolaHuesped { get; set; }
    public List<ProyectilFísico> MetrallaHuesped { get; set; }
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
    public float[] Posición { get; set; }
    public Quaternion Rotación { get; set; }
}

public class ProyectilFísico
{
    public float[] Posición { get; set; }
    public float[] Escala { get; set; }
    public Quaternion Rotación { get; set; }
}
