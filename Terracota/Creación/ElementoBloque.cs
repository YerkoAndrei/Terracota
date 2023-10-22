using System;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;

public class ElementoBloque : AsyncScript
{
    public RigidbodyComponent cuerpo;

    public bool moviendo;
    private bool tocandoBloque;

    public override async Task Execute()
    {
        while (Game.IsRunning)
        {
            var colisión = await cuerpo.NewCollision();

            // Identifica colisión
            var tocandoBase = false;
            if (colisión.ColliderA.Entity.Get<ElementoBloqueBase>() != null)
                tocandoBase = true;
            else if (colisión.ColliderB.Entity.Get<ElementoBloqueBase>() != null)
                tocandoBase = true;

            if (!tocandoBase)
            {
                tocandoBloque = true;
                await cuerpo.CollisionEnded();
                tocandoBloque = false;
            }
            await Script.NextFrame();
        }
    }

    public void ActualizarPosición(Vector3 nuevaPosición, bool tocando)
    {
        moviendo = true;
        var altura = 0;
        if (tocando)
            altura = 1;

        Entity.Transform.Position = new Vector3(nuevaPosición.X, altura, nuevaPosición.Z);
    }

    public bool Colocar()
    {
        moviendo = false;
        return !tocandoBloque;
    }
}
