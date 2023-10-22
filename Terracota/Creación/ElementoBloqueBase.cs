using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;

public class ElementoBloqueBase : AsyncScript
{
    private RigidbodyComponent cuerpo;
    private float altura;

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

            if (!bloque.ObtenerMoviendo())
            {
                CambiarCuerpo(true);

                // Verificar nuevas colisiones
                //await colisión.Ended();
                await cuerpo.CollisionEnded();
                CambiarCuerpo(false);
            }
            await Script.NextFrame();
        }
    }

    private void CambiarCuerpo(bool agrandar)
    {
        if (agrandar && altura < 3)
            altura += 1;
        else if(!agrandar && altura > 0)
            altura -= 1;

        Entity.Transform.Scale = new Vector3(1, altura, 1);
    }

    public void ReiniciarCuerpo()
    {
        altura = 0;
        Entity.Transform.Scale = Vector3.One;
    }

    public void ActualizarPosición(Vector3 nuevaPosición)
    {
        nuevaPosición.Y = 0;
        Entity.Transform.Position = nuevaPosición;
    }

    public float ObtenerAltura()
    {
        return altura;
    }
}
