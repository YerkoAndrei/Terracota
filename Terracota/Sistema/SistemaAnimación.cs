using Stride.Engine;
using Stride.Animations;

namespace Terracota;
using static Constantes;

public class SistemaAnimación : StartupScript
{
    public AnimationCurve curvaEstandar;

    public override void Start()
    {
        Log.Warning("Log activado:");
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
    */
}
