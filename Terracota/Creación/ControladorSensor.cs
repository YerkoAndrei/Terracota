using System.Collections.Generic;
using System.Linq;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Constantes;

public class ControladorSensor : StartupScript
{
    public List<RigidbodyComponent> sensores = new List<RigidbodyComponent> { };

    private TipoBloque tipoBloqueActual;

    public override void Start() { }

    public void ActualizarPosición(Vector3 nuevaPosición)
    {
        nuevaPosición.Y = 0;
        Entity.Transform.Position = nuevaPosición;
    }

    public int ObtenerAltura(Entity bloqueActual)
    {
        // Estatua solo toca piso
        if (tipoBloqueActual == TipoBloque.estatua)
            return 0;

        // Controla posiciones
        if (sensores[2].Collisions.Count > 1 && sensores[3].Collisions.Count == 0)
            return VerificarSuelo(bloqueActual, 3);
        if (sensores[1].Collisions.Count > 1 && sensores[2].Collisions.Count == 0)
            return VerificarSuelo(bloqueActual, 2);
        if (sensores[0].Collisions.Count > 1 && sensores[1].Collisions.Count == 0) 
            return VerificarSuelo(bloqueActual, 1);
        
        if (sensores[2].Collisions.Count > 0 && sensores[3].Collisions.Count > 0)
            return VerificarSuelo(bloqueActual, 3);
        if (sensores[1].Collisions.Count > 0 && sensores[2].Collisions.Count > 0)
            return VerificarSuelo(bloqueActual, 2);
        if (sensores[0].Collisions.Count > 0 && sensores[1].Collisions.Count > 0) 
            return VerificarSuelo(bloqueActual, 1);
        
        return 0;
    }

    public int VerificarSuelo(Entity bloqueActual, int altura)
    {
        // Revisa colisiones, excepto bloque actual, para forzar altura actual
        var colisiones0 = sensores[0].Collisions.Where(o => o.ColliderA.Entity.GetParent() != bloqueActual && o.ColliderB.Entity.GetParent() != bloqueActual).ToArray();
        var colisiones1 = sensores[1].Collisions.Where(o => o.ColliderA.Entity.GetParent() != bloqueActual && o.ColliderB.Entity.GetParent() != bloqueActual).ToArray();
        var colisiones2 = sensores[2].Collisions.Where(o => o.ColliderA.Entity.GetParent() != bloqueActual && o.ColliderB.Entity.GetParent() != bloqueActual).ToArray();
        var colisiones3 = sensores[3].Collisions.Where(o => o.ColliderA.Entity.GetParent() != bloqueActual && o.ColliderB.Entity.GetParent() != bloqueActual).ToArray();

        // ¿Optimizar?
        if (colisiones0.Length == 0)
            return 0;
        if (colisiones1.Length == 0)
            return 1;
        if (colisiones2.Length == 0)
            return 2;
        if (colisiones3.Length == 0)
            return 3;

        return altura;
    }

    public void ReiniciarCuerpo(TipoBloque tipoBloque, Quaternion rotación)
    {
        Entity.Transform.Rotation = rotación;
        tipoBloqueActual = tipoBloque;
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
