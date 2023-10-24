using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Constantes;

public class ElementoBloqueBase : StartupScript
{
    public List<RigidbodyComponent> bloquesBase = new List<RigidbodyComponent> { };

    public override void Start() { }

    public void ActualizarPosición(Vector3 nuevaPosición)
    {
        nuevaPosición.Y = 0;
        Entity.Transform.Position = nuevaPosición;
    }

    public float ObtenerAltura()
    {
        if (bloquesBase[2].Collisions.Count > 1)
            return 3;
        if (bloquesBase[1].Collisions.Count > 1)
            return 2;
        if (bloquesBase[0].Collisions.Count > 1)
            return 1;
        
        return 0;
    }

    public void ReiniciarCuerpo(TipoBloque tipoBloque, Quaternion rotación)
    {
        Entity.Transform.Rotation = rotación;
        switch (tipoBloque)
        {
            case TipoBloque.estatua:
                Entity.Transform.Scale = new Vector3(1.2f, 1, 1.2f);
                break;
            case TipoBloque.corto:
                Entity.Transform.Scale = new Vector3(1, 1, 1);
                break;
            case TipoBloque.largo:
                Entity.Transform.Scale = new Vector3(2, 1, 1);
                break;
        }
    }

    public void Rotar()
    {
        Entity.Transform.Rotation *= Quaternion.RotationY(MathUtil.DegreesToRadians(-45));
    }
}
