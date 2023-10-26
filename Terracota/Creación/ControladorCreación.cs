using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;
using Stride.Graphics;
using Stride.Input;

namespace Terracota;
using static Constantes;

public class ControladorCreación : SyncScript
{
    public ControladorCámara controladorCámara;
    public ControladorSensor sensor;
    public CameraComponent cámara;
    public InterfazCreación interfaz;

    public List<ElementoBloque> estatuas = new List<ElementoBloque> { };
    public List<ElementoBloque> cortos = new List<ElementoBloque> { };
    public List<ElementoBloque> largos = new List<ElementoBloque> { };

    private ElementoBloque bloqueActual;
    private bool moviendoCámara;
    private Texture backBuffer;
    private Viewport viewport;

    private List<ElementoBloque> bloques;
    private List<Vector3> pocisionesIniciales;

    private float rotaciónClic;

    public override void Start()
    {
        backBuffer = GraphicsDevice.Presenter.BackBuffer;
        viewport = new Viewport(0, 0, backBuffer.Width, backBuffer.Height);

        bloques = new List<ElementoBloque>();
        bloques.AddRange(estatuas);
        bloques.AddRange(cortos);
        bloques.AddRange(largos);

        rotaciónClic = -45;
        pocisionesIniciales = new List<Vector3>();
        foreach(var bloque in bloques)
        {
            pocisionesIniciales.Add(bloque.ObtenerPosición());
        }
    }

    public override void Update()
    {
        // Intenta encontrar bloque
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            if (BuscarBloque())
                return;
        }

        if (bloqueActual == null)
            return;

        var resultado = ObtienePosiciónCursor();
        if (resultado.Succeeded)
        {
            sensor.ActualizarPosición(resultado.Point);
            bloqueActual.ActualizarPosición(resultado.Point, sensor.ObtenerAltura());
        }

        // Rotación
        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            EnClicGirarPieza();
        }

        // Guardar
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            if (bloqueActual.EsPosibleColocar())
                bloqueActual = null;
        }
    }

    public HitResult ObtienePosiciónCursor()
    {
        var posiciónInicio = viewport.Unproject(new Vector3(Input.AbsoluteMousePosition, 0.0f), cámara.ProjectionMatrix, cámara.ViewMatrix, Matrix.Identity);
        var pocisiónLejos = viewport.Unproject(new Vector3(Input.AbsoluteMousePosition, 1.0f), cámara.ProjectionMatrix, cámara.ViewMatrix, Matrix.Identity);

        var resultado = this.GetSimulation().Raycast(posiciónInicio, pocisiónLejos, CollisionFilterGroups.StaticFilter, CollisionFilterGroupFlags.StaticFilter);
        return resultado;
    }

    public bool BuscarBloque()
    {
        var posiciónInicio = viewport.Unproject(new Vector3(Input.AbsoluteMousePosition, 0.0f), cámara.ProjectionMatrix, cámara.ViewMatrix, Matrix.Identity);
        var pocisiónLejos = viewport.Unproject(new Vector3(Input.AbsoluteMousePosition, 1.0f), cámara.ProjectionMatrix, cámara.ViewMatrix, Matrix.Identity);

        var resultado = this.GetSimulation().Raycast(posiciónInicio, pocisiónLejos, CollisionFilterGroups.SensorTrigger, CollisionFilterGroupFlags.DefaultFilter, true);
        if (resultado.Succeeded && bloqueActual == null &&
            resultado.Collider.Entity.GetParent() != null && resultado.Collider.Entity.GetParent().Get<ElementoBloque>() != null)
        {
            var tipoBloque = resultado.Collider.Entity.GetParent().Get<ElementoBloque>().tipoBloque;
            var númeroStr = resultado.Collider.Entity.GetParent().Name;
            int número = int.Parse(númeroStr[^1].ToString());

            AgregarBloque(tipoBloque, número);
            return true;
        }
        return false;
    }

    public void AgregarBloque(TipoBloque tipoBloque, int id)
    {
        switch(tipoBloque)
        {
            case TipoBloque.estatua:
                bloqueActual = estatuas[id];
                break;
            case TipoBloque.corto:
                bloqueActual = cortos[id];
                break;
            case TipoBloque.largo:
                bloqueActual = largos[id];
                break;
        }
        sensor.ReiniciarCuerpo(bloqueActual.tipoBloque, bloqueActual.ObtenerRotación());
    }

    public ElementoBloque ObtenerActual()
    {
        return bloqueActual;
    }

    public void EnClicReiniciarPosiciones()
    {
        for (int i=0; i < bloques.Count; i++)
        {
            bloques[i].ForzarPosición(pocisionesIniciales[i]);
        }
    }

    public void EnClicGuardar(int ranura)
    {
        // PENDIENTE: esperar a colocar todos los bloques
        // PENDIENTE: sacar miniatura

        if(SistemaMemoria.GuardarFortaleza(bloques.ToArray(), ranura, null))
        {
            interfaz.CerrarPanelGuardar();
            EnClicReiniciarPosiciones();
        }
    }

    public void EnClicGirarPieza()
    {
        if (bloqueActual == null)
            return;

        sensor.Rotar(rotaciónClic);
        bloqueActual.Entity.Transform.Rotation *= Quaternion.RotationY(MathUtil.DegreesToRadians(rotaciónClic));
    }

    public async void EnClicMoverCámara(bool derecha)
    {
        if (moviendoCámara)
            return;

        await controladorCámara.RotarCámara(90, derecha);
        moviendoCámara = false;
    }
}
