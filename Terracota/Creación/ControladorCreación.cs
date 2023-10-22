using System.Threading.Tasks;
using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;
using Stride.Graphics;
using Stride.Input;

namespace Terracota;

public class ControladorCreación : SyncScript
{
    public TransformComponent ejeCámara;
    public ElementoBloqueBase bloqueBase;
    public CameraComponent cámara;

    public List<ElementoBloque> cortos = new List<ElementoBloque> { };
    public List<ElementoBloque> largos = new List<ElementoBloque> { };

    private ElementoBloque bloqueActual;

    private Texture backBuffer;
    private Viewport viewport;

    public override void Start()
    {
        backBuffer = GraphicsDevice.Presenter.BackBuffer;
        viewport = new Viewport(0, 0, backBuffer.Width, backBuffer.Height);
    }

    public override void Update()
    {
        if (bloqueActual == null)
            return;

        var resultado = ObtienePosiciónCursor();
        if (resultado.Succeeded)
        {
            bloqueBase.ActualizarPosición(resultado.Point);
            bloqueActual.ActualizarPosición(resultado.Point, bloqueBase.ObtenerColisión());
        }

        // Rotación
        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            bloqueActual.Entity.Transform.Rotation *= Quaternion.RotationY(MathUtil.DegreesToRadians(-45));
        }

        // Guardar
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        { 
            if(bloqueActual.Colocar())
                bloqueActual = null;
        }
    }

    public HitResult ObtienePosiciónCursor()
    {
        var posiciónCerca = viewport.Unproject(new Vector3(Input.AbsoluteMousePosition, 0.0f), cámara.ProjectionMatrix, cámara.ViewMatrix, Matrix.Identity);
        var pocisiónLejos = viewport.Unproject(new Vector3(Input.AbsoluteMousePosition, 1.0f), cámara.ProjectionMatrix, cámara.ViewMatrix, Matrix.Identity);

        var resultado = this.GetSimulation().Raycast(posiciónCerca, pocisiónLejos);
        return resultado;
    }

    public void AgregaCorto(int corto)
    {
        bloqueActual = cortos[corto];
    }

    public void AgregarLargo(int largo)
    {
        bloqueActual = largos[largo];
    }

    public async void MoverCámara(bool derecha)
    {
        await MoverCámara(90, derecha);
    }

    public void Guardar()
    {
        // esperar a colocar todos los bloques
        var bloques = new List<ElementoBloque>();
        bloques.AddRange(cortos);
        bloques.AddRange(largos);

        SistemaMemoria.GuardarFortaleza(bloques.ToArray());
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
