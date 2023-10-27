using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Constantes;

public class ElementoBloque : StartupScript
{
    public TipoBloque tipoBloque;
    public RigidbodyComponent cuerpo;

    // Posición hijo
    private Vector3 posiciónCuerpo;

    public override void Start()
    {
        posiciónCuerpo = cuerpo.Entity.Transform.Position;
        cuerpo.IsKinematic = true;
    }

    public void Activar()
    {
        cuerpo.Entity.Transform.Position = posiciónCuerpo;
        cuerpo.IsKinematic = false;
    }

    public void Posicionar(Vector3 posición, Quaternion rotación)
    {
        Entity.Transform.Position = posición;
        Entity.Transform.Rotation = rotación;
    }
}
