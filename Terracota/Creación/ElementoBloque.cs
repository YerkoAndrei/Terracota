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
            await cuerpo.NewCollision();
            tocandoBloque = true;

            await cuerpo.CollisionEnded();
            tocandoBloque = false;

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
