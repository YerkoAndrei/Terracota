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
    private bool moviendoCámara;
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
            bloqueActual.ActualizarPosición(resultado.Point, bloqueBase.ObtenerAltura());
        }

        // Rotación
        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            GirarPieza();
        }

        // Guardar
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            if (bloqueActual.Colocar())
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

    public void AgregarCorto(int corto)
    {
        bloqueActual = cortos[corto];
        bloqueBase.ReiniciarCuerpo(bloqueActual.tipoBloque, bloqueActual.ObtenerRotación());
    }

    public void AgregarLargo(int largo)
    {
        bloqueActual = largos[largo];
        bloqueBase.ReiniciarCuerpo(bloqueActual.tipoBloque, bloqueActual.ObtenerRotación());
    }

    public void Reiniciar()
    {

    }

    public void Guardar()
    {
        // esperar a colocar todos los bloques
        var bloques = new List<ElementoBloque>();
        bloques.AddRange(cortos);
        bloques.AddRange(largos);

        SistemaMemoria.GuardarFortaleza(bloques.ToArray());
    }

    public void GirarPieza()
    {
        if (bloqueActual == null)
            return;

        bloqueBase.Rotar();
        bloqueActual.Entity.Transform.Rotation *= Quaternion.RotationY(MathUtil.DegreesToRadians(-45));
    }

    public async void MoverCámara(bool derecha)
    {
        if (moviendoCámara)
            return;

        await MoverCámara(90, derecha);
    }

    private async Task MoverCámara(float YObjetivo, bool derecha)
    {
        moviendoCámara = true;

        float duraciónLerp = 0.5f;
        float tiempoLerp = 0;
        float tiempo = 0;

        var rotaciónInicial = ejeCámara.Rotation;
        var rotaciónObjetivo = Quaternion.Identity;

        if (derecha)
            rotaciónObjetivo = ejeCámara.Rotation * Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo));
        else
            rotaciónObjetivo = ejeCámara.Rotation * Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo * -1));

        while (tiempoLerp < duraciónLerp)
        {
            tiempo = tiempoLerp / duraciónLerp;
            ejeCámara.Rotation = Quaternion.Lerp(rotaciónInicial, rotaciónObjetivo, tiempo);

            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Task.Delay(1);
            //await Script.NextFrame();
        }

        // Fin
        ejeCámara.Rotation = rotaciónObjetivo;
        moviendoCámara = false;
    }
}
