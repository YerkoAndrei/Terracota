using Stride.Engine;
using System.Threading.Tasks;
using Stride.Core.Mathematics;

namespace Terracota;

public class ControladorCreación : SyncScript
{
    public TransformComponent ejeCámara;

    public Entity[] cortos;
    public Entity[] largos;

    public override void Start()
    {

    }

    public override void Update()
    {

    }

    public void AgregaCorto(int corto)
    {
        //cortos[corto].Transform.Position = mouse
    }

    public void AgregarLargo(int largo)
    {
        //largos[largo].Transform.Position = 
    }

    public async void MoverCámara(bool derecha)
    {
        await MoverCámara(90, derecha);
    }

    public void SalirGuardar()
    {

    }

    private async Task MoverCámara(float YObjetivo, bool derecha)
    {
        float duraciónLerp = 1f;
        float tiempoLerp = 0;
        float tiempo = 0;

        var rotaciónInicial = ejeCámara.Rotation;
        var rotaciónObjetivo = Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo));

        // Ajusta dirección de movimiento
        var direcciónObjetivo = rotaciónObjetivo;
        if (derecha)
            direcciónObjetivo = Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo - 0.01f));
        else
            direcciónObjetivo = Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo + 0.01f));

        while (tiempoLerp < duraciónLerp)
        {
            tiempo = tiempoLerp / duraciónLerp;
            ejeCámara.Rotation = Quaternion.Lerp(rotaciónInicial, direcciónObjetivo, tiempo);

            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Script.NextFrame();
        }

        // Fin
        ejeCámara.Rotation = rotaciónObjetivo;
    }
}
