using Stride.Core.Mathematics;
using Stride.Engine;

namespace Terracota;
using static Constantes;

public class ElementoBloque : StartupScript
{
    public TipoBloque tipoBloque;

    public override void Start() { }

    public void Posicionar(Vector3 posición, Quaternion rotación)
    {
        Entity.Transform.Position = posición;
        Entity.Transform.Rotation = rotación;
    }
}
