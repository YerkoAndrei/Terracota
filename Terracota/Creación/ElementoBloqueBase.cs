using System;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;

public class ElementoBloqueBase : AsyncScript
{
    private RigidbodyComponent cuerpo;
    private bool tocandoBloque;

    public override async Task Execute()
    {
        cuerpo = Entity.Get<RigidbodyComponent>();
        while (Game.IsRunning)
        {
            var colisión = await cuerpo.NewCollision();
            
            // Identifica colisión
            ElementoBloque bloque = null;
            if (colisión.ColliderA.Entity.GetParent() != null && colisión.ColliderA.Entity.GetParent().Get<ElementoBloque>() != null)
                bloque = colisión.ColliderA.Entity.GetParent().Get<ElementoBloque>();
            else if (colisión.ColliderB.Entity.GetParent() != null && colisión.ColliderB.Entity.GetParent().Get<ElementoBloque>() != null)
                bloque = colisión.ColliderB.Entity.GetParent().Get<ElementoBloque>();

            if (!bloque.moviendo)
            {
                tocandoBloque = true;
                await cuerpo.CollisionEnded();
                tocandoBloque = false;
            }
            await Script.NextFrame();
        }
    }

    public void ActualizarPosición(Vector3 nuevaPosición)
    {
        nuevaPosición.Y = 0.5f;
        Entity.Transform.Position = nuevaPosición;
    }

    public bool ObtenerColisión()
    {
        return tocandoBloque;
    }
}
