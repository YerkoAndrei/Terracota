using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Constantes;

public class ControladorSensor : StartupScript
{
    public List<RigidbodyComponent> sensores = new List<RigidbodyComponent> { };

    public override void Start() { }

    public void ActualizarPosición(Vector3 nuevaPosición)
    {
        nuevaPosición.Y = 0;
        Entity.Transform.Position = nuevaPosición;
    }

    public float ObtenerAltura()
    {
        // PENDIENTE
        if (sensores[2].Collisions.Count > 1)
            return 3;
        if (sensores[1].Collisions.Count > 1)
            return 2;
        if (sensores[0].Collisions.Count > 1)
            return 1;

        return 0;
    }

    public void ReiniciarCuerpo(TipoBloque tipoBloque, Quaternion rotación)
    {
        Entity.Transform.Rotation = rotación;
        switch (tipoBloque)
        {
            case TipoBloque.estatua:
                Entity.Transform.Scale = new Vector3(1.5f, 1, 1.5f);
                break;
            case TipoBloque.corto:
                Entity.Transform.Scale = new Vector3(1, 1, 1);
                break;
            case TipoBloque.largo:
                Entity.Transform.Scale = new Vector3(2, 1, 1);
                break;
        }
    }

    public void Rotar(float rotación)
    {
        Entity.Transform.Rotation *= Quaternion.RotationY(MathUtil.DegreesToRadians(rotación));
    }
}
