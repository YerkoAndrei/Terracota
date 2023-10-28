using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Constantes;

public class ControladorSensor : StartupScript
{
    public List<RigidbodyComponent> sensores = new List<RigidbodyComponent> { };

    private TipoBloque bloqueActual;

    public override void Start() { }

    public void ActualizarPosición(Vector3 nuevaPosición)
    {
        nuevaPosición.Y = 0;
        Entity.Transform.Position = nuevaPosición;
    }

    public int ObtenerAltura()
    {
        if (bloqueActual == TipoBloque.estatua)
            return ObtenerAlturaEstatua();
        /*
        // Fuerza posición 0
        if (sensores[0].Collisions.Count <= 1 && (sensores[1].Collisions.Count + sensores[2].Collisions.Count + sensores[3].Collisions.Count) > 1)
            return 0;
        */
            // Controla posiciones
        if (sensores[2].Collisions.Count > 1)
            return 3;
        if (sensores[1].Collisions.Count > 1)
            return 2;
        if (sensores[0].Collisions.Count > 1)
            return 1;
        
        if (sensores[2].Collisions.Count > 0 && sensores[3].Collisions.Count > 0)
            return 3;
        if (sensores[1].Collisions.Count > 0 && sensores[2].Collisions.Count > 0)
            return 2;
        if (sensores[0].Collisions.Count > 0 && sensores[1].Collisions.Count > 0)
            return 1;
        
        return 0;
    }

    private int ObtenerAlturaEstatua()
    {
        return 0;
    }

    public void ReiniciarCuerpo(TipoBloque tipoBloque, Quaternion rotación)
    {
        Entity.Transform.Rotation = rotación;
        bloqueActual = tipoBloque;
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
