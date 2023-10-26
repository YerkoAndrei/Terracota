using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace Terracota;

public class ControladorCámara : StartupScript
{
    private TransformComponent cámara;
    private float ZInicial;

    public override void Start()
    {
        cámara = Entity.Get<TransformComponent>();
        ZInicial = cámara.Position.Z;
    }

    public async void ActivarEfectoDisparo()
    {
        await MoverCámara(0.8f);
    }

    private async Task MoverCámara(float retroceso)
    {
        float duraciónLerp = 0.03f;
        float tiempoLerp = 0;
        float tiempo = 0;

        var posiciónInicial = cámara.Position;
        var posiciónObjetivo = new Vector3(posiciónInicial.X, posiciónInicial.Y, (posiciónInicial.Z + retroceso));

        while (tiempoLerp < duraciónLerp)
        {
            tiempo = tiempoLerp / duraciónLerp;
            cámara.Position = Vector3.Lerp(posiciónInicial, posiciónObjetivo, tiempo);

            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Task.Delay(1);
            //await Script.NextFrame();
        }

        // Fin
        cámara.Position = posiciónObjetivo;

        // Retroceso
        duraciónLerp = 0.8f;
        tiempoLerp = 0;
        tiempo = 0;

        posiciónInicial = cámara.Position;
        posiciónObjetivo = new Vector3(posiciónInicial.X, posiciónInicial.Y, ZInicial);

        while (tiempoLerp < duraciónLerp)
        {
            tiempo = tiempoLerp / duraciónLerp;
            cámara.Position = Vector3.Lerp(posiciónInicial, posiciónObjetivo, tiempo);

            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Task.Delay(1);
            //await Script.NextFrame();
        }

        // Fin
        cámara.Position = posiciónObjetivo;
    }
}
