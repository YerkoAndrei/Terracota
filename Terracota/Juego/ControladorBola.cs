using System.Collections.Generic;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Constantes;

public class ControladorBola : AsyncScript
{
    public int maxColisiones;
    public Prefab prefabPartículas;

    private RigidbodyComponent cuerpo;
    private bool destruyendo;
    private int colisiones;

    private List<Entity> partículas;

    public override async Task Execute()
    {
        cuerpo = Entity.Get<RigidbodyComponent>();
        partículas = new List<Entity>();

        // Tiempo de vida
        var contarVida = ContarVida();

        while (Game.IsRunning)
        {
            var colisión = await cuerpo.NewCollision();

            // Metralla con metralla no cuenta como colisión
            if ((colisión.ColliderA == cuerpo && colisión.ColliderB.Entity.Get<ControladorBola>() == null) ||
               (colisión.ColliderB == cuerpo && colisión.ColliderA.Entity.Get<ControladorBola>() == null))
            {
                colisiones++;
                MostrarEfectos();
            }
            
            // Evita colisiones innesesarias
            if (colisiones >= maxColisiones)
                await Destruir();

            await Script.NextFrame();
        }
    }

    private void MostrarEfectos()
    {
        var partícula = prefabPartículas.Instantiate()[0];
        partícula.Transform.Position = Entity.Transform.Position;
        Entity.Scene.Entities.Add(partícula);

        // Posterior borrado y descarga
        partículas.Add(partícula);
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

        // Removiendo entidades
        foreach(var partículas in partículas)
        {
            //Content.Unload(partículas);
            Entity.Scene.Entities.Remove(partículas);
        }
        //Content.Unload(Entity);
        Entity.Scene.Entities.Remove(Entity);
    }
}
