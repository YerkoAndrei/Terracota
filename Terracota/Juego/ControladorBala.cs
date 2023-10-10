using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota.Juego;

public class ControladorBala : AsyncScript
{
    public int maxColisiones;

    private int colisiones;

    public override async Task Execute()
    {
        var cuerpo = Entity.Get<RigidbodyComponent>();

        // PENDIENTE: destruir por tiempo

        while (Game.IsRunning)
        {
            await cuerpo.NewCollision();
            colisiones++;

            // Evita colisiones innesesarias
            if (colisiones >= maxColisiones)
                await Destruir();

            await Script.NextFrame();
        }
    }

    private async Task Destruir()
    {
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
        SceneSystem.SceneInstance.RootScene.Entities.Remove(Entity);
    }
}
