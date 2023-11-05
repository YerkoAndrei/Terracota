using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Constantes;

public class ControladorBola : AsyncScript
{
    public int maxColisiones;

    private RigidbodyComponent cuerpo;
    private bool destruyendo;
    private int colisiones;

    public override async Task Execute()
    {
        cuerpo = Entity.Get<RigidbodyComponent>();

        // Tiempo de vida
        ContarVida();

        while (Game.IsRunning)
        {
            var colisión = await cuerpo.NewCollision();

            // Metralla con metralla no cuenta como colisión
            if ((colisión.ColliderA == cuerpo && colisión.ColliderB.Entity.Get<ControladorBola>() == null) ||
               (colisión.ColliderB == cuerpo && colisión.ColliderA.Entity.Get<ControladorBola>() == null))
            {
                colisiones++;
            }
            
            // Evita colisiones innesesarias
            if (colisiones >= maxColisiones)
                await Destruir();

            await Script.NextFrame();
        }
    }

    private async Task ContarVida()
    {
        await Task.Delay(duraciónTurno);
        await Destruir();
    }

    private async Task Destruir()
    {
        if (destruyendo) return;
        destruyendo = true;

        float duraciónLerp = 1;
        float tiempoLerp = 0;
        float tiempo = 0;

        var inicial = Entity.Transform.Scale;
        while (tiempoLerp < duraciónLerp)
        {
            tiempo = tiempoLerp / duraciónLerp;

            Entity.Transform.Scale = Vector3.Lerp(inicial, Vector3.Zero, tiempo);
            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Script.NextFrame();
        }

        // Fin
        cuerpo.Enabled = false;
        Entity.Scene.Entities.Remove(Entity);
    }
}
