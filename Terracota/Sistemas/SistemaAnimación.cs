using Stride.Engine;
using Stride.Animations;

namespace Terracota;
using static Constantes;

public class SistemaAnimación : StartupScript
{
    public AnimationCurve curvaEstandar;

    public override void Start()
    {
        //Log.Warning("Log activado:");
    }

    /*
    public static float EvaluarCurva(float tiempo)
    {
        return curvaEstandar.Evaluate(tiempo);
    }

    private static async Task Animar(Entity entidad, float duraciónLerp, bool conCurva, Color colorInicio, Color colorFinal, Action alFinal, Stride.Games.IGame game)
    {
        entidad.color = colorInicio;

        float tiempoLerp = 0;
        float tiempo = 0;
        while (tiempoLerp < duraciónLerp)
        {
            if (conCurva)
                tiempo = EvaluarCurva(tiempoLerp / duraciónLerp);
            else
                tiempo = tiempoLerp / duraciónLerp;

            entidad.color = Color.Lerp(colorInicio, colorFinal, tiempo);
            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Task.Delay(0);
        }

        // Fin
        entidad.color = colorFinal;

        if (alFinal != null)
            alFinal.Invoke();
    }

    private async Task MoverCámara(TransformComponent objetivo)
    {
        float duraciónLerp = 1.5f;
        float tiempoLerp = 0;
        float tiempo = 0;

        var posiciónInicial = cámara.Position;
        var rotaciónInicial = cámara.Rotation;

        var posiciónObjetivo = objetivo.Position;
        var rotaciónObjetivo = objetivo.Rotation;

        while (tiempoLerp < duraciónLerp)
        {
            tiempo = tiempoLerp / duraciónLerp;

            cámara.Position = Vector3.Lerp(posiciónInicial, posiciónObjetivo, tiempo);
            cámara.Rotation = Quaternion.Lerp(rotaciónInicial, rotaciónObjetivo, tiempo);

            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Script.NextFrame();
        }

        // Fin
        cámara.Position = posiciónObjetivo;
        cámara.Rotation = rotaciónObjetivo;
    }
    */
}
