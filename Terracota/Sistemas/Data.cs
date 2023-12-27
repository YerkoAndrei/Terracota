using Stride.Core.Mathematics;
using static Terracota.Constantes;

public class Conexión
{
    public string IP { get; set; }
    public TipoConexión TipoConexión { get; set; }
    public TipoJugador ConectarComo { get; set; }
}

public class Físicas
{
    public Vector3 Posición { get; set; }
    public Quaternion Rotación { get; set; }
}
