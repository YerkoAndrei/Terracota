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
        Log.Warning("a");
        while (Game.IsRunning)
        {/*
            var colisión = await cuerpo.NewCollision();
            
            // Identifica colisión
            ElementoBloque bloque;
            if (colisión.ColliderA.Entity.GetParent().Get<ElementoBloque>() != null)
                bloque = colisión.ColliderA.Entity.GetParent().Get<ElementoBloque>();
            else
                bloque = colisión.ColliderB.Entity.GetParent().Get<ElementoBloque>();

            if (bloque.moviendo)
            {
                tocandoBloque = true;
                await cuerpo.CollisionEnded();
                tocandoBloque = false;
            }*/
            await Script.NextFrame();
        }
    }

    public void ActualizarPosición(Vector3 nuevaPosición)
    {
        Entity.Transform.Position = nuevaPosición;
    }

    public bool ObtenerColisión()
    {
        return tocandoBloque;
    }
}
