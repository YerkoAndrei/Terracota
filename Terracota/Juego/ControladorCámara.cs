using System;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace Terracota;

public class ControladorCámara : StartupScript
{
    public TransformComponent eje;
    public TransformComponent cámara;

    private float ZInicial;
    
    public override void Start()
    {
        cámara = Entity.Get<TransformComponent>();
        ZInicial = cámara.Position.Z;
    }
    
    public async Task RotarCámara(float YObjetivo, bool derecha, TransformComponent luzDireccional = null)
    {
        await RotarEjeCámara(YObjetivo, derecha, luzDireccional);
    }

    public async void ActivarEfectoDisparo()
    {
        await MoverCámara(0.8f);
    }

    private async Task RotarEjeCámara(float YObjetivo, bool derecha, TransformComponent luzDireccional = null)
    {
        float duraciónLerp = 1f;
        float tiempoLerp = 0;
        float tiempo = 0;

        var rotaciónInicial = eje.Rotation;
        var rotaciónObjetivo = rotaciónInicial * Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo));

        // Ajusta dirección de movimiento
        var direcciónObjetivo = rotaciónObjetivo;
        if (derecha)
            direcciónObjetivo = rotaciónInicial * Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo - 0.01f));
        else
            direcciónObjetivo = rotaciónInicial * Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo + 0.01f));

        while (tiempoLerp < duraciónLerp)
        {
            tiempo = tiempoLerp / duraciónLerp;
            eje.Rotation = Quaternion.Lerp(rotaciónInicial, direcciónObjetivo, tiempo);

            // Mueve sol 45º aprox.
            if (luzDireccional != null)
                luzDireccional.Rotation *= Quaternion.RotationY(0.005f);

            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Task.Delay(1);
            //await Script.NextFrame();
        }

        // Fin
        eje.Rotation = rotaciónObjetivo;
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
            await Script.NextFrame();
        }

        // Fin
        cámara.Position = posiciónObjetivo;
    }
}
