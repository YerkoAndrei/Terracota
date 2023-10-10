using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;

namespace Terracota.Juego;
using static Constantes;

public class ControladorPartida : AsyncScript
{
    public TransformComponent cámara;

    public TransformComponent cámaraAnfitrión;
    public TransformComponent cámaraHuesped;

    public override async Task Execute()
    {
        while (Game.IsRunning)
        {
            if (Input.IsKeyPressed(Keys.C))
                await MoverCámara(TipoJugador.huesped);
            if (Input.IsKeyPressed(Keys.V))
                await MoverCámara(TipoJugador.anfitrión);

            await Script.NextFrame();
        }
    }

    private async Task MoverCámara(TipoJugador jugador)
    {
        float duraciónLerp = 2;
        float tiempoLerp = 0;
        float tiempo = 0;

        var posiciónObjetivo = Vector3.Zero;
        var rotaciónObjetivo = Quaternion.Identity;
        switch (jugador)
        {
            case TipoJugador.anfitrión:
                posiciónObjetivo = cámaraAnfitrión.Position;
                rotaciónObjetivo = cámaraAnfitrión.Rotation;
                break;
            case TipoJugador.huesped:
                posiciónObjetivo = cámaraHuesped.Position;
                rotaciónObjetivo = cámaraHuesped.Rotation;
                break;
        }

        var posiciónInicial = cámara.Position;
        var rotaciónInicial = cámara.Rotation;
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
}
