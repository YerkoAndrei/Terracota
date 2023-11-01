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
    public TransformComponent fortaleza;

    public List<ElementoCreación> estatuas = new List<ElementoCreación> { };
    public List<ElementoCreación> cortos = new List<ElementoCreación> { };
    public List<ElementoCreación> largos = new List<ElementoCreación> { };

    private ElementoCreación bloqueActual;
    private bool moviendoCámara;
    private Texture backBuffer;
    private Viewport viewport;

    private List<string> bloquesListos;
    private List<ElementoCreación> bloques;

    private float rotaciónClic;

    public override void Start()
    {
        backBuffer = GraphicsDevice.Presenter.BackBuffer;
        viewport = new Viewport(0, 0, backBuffer.Width, backBuffer.Height);

        bloquesListos = new List<string>();
        bloques = new List<ElementoCreación>();
        bloques.AddRange(estatuas);
        bloques.AddRange(cortos);
        bloques.AddRange(largos);

        rotaciónClic = -45;
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
            bloqueActual.ActualizarPosición(resultado.Point, sensor.ObtenerAltura(bloqueActual.Entity));
        }

        // Rotación
        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            EnClicGirarPieza();
        }

        // Guardar
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            if (bloqueActual.EsPosibleColocar() && sensor.EsPosibleColocar())
            {
                AgregarBloque();
                bloqueActual = null;
            }
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
            resultado.Collider.Entity.GetParent() != null && resultado.Collider.Entity.GetParent().Get<ElementoCreación>() != null)
        {
            var tipoBloque = resultado.Collider.Entity.GetParent().Get<ElementoCreación>().tipoBloque;
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

    public ElementoCreación ObtenerActual()
    {
        return bloqueActual;
    }

    public void EnClicReiniciarPosiciones()
    {
        foreach (var bloque in bloques)
        {
            bloque.ReiniciarTransform();
        }
    }

    private void AgregarBloque()
    {
        // Agrega tipo + número
        var código = bloqueActual.tipoBloque.ToString() + bloqueActual.ObtenerNúmero();
        if (!bloquesListos.Contains(código))
            bloquesListos.Add(código);
    }

    public void EnClicGuardar(int ranura)
    {
        if(bloquesListos.Count < bloques.Count)
        {
            // PENDIENTE: mensaje error
            Log.Warning("no se puedeee");
            return;
        }

        // PENDIENTE: crear o sacar miniatura

        // Posición respecto a la fortaleza
        foreach (var bloque in bloques)
        {
            bloque.PosicionarEnFortaleza(fortaleza.Position);
        }

        // Guardado
        if(SistemaMemoria.GuardarFortaleza(bloques.ToArray(), ranura, null))
        {
            // PENDIENTE: mensaje ok
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

    public void EnClicMoverCámara(bool derecha)
    {
        if (moviendoCámara)
            return;

        controladorCámara.RotarCámara(90, derecha, () =>
        {
            moviendoCámara = false;
        });
    }
}
