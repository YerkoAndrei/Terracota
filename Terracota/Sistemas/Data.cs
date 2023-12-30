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
    public Quaternion Cañón { get; set; }
    public BloqueFísico[] Bloques { get; set; }
}

public class BloqueFísico
{
    public int Id { get; set; }
    public Vector3 Posición { get; set; }
    public Quaternion Rotación { get; set; }
}
