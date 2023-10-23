using System.Collections.Specialized;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Constantes;

public class ElementoBloqueBase : StartupScript
{
    private ControladorCreación controladorCreación;
    private RigidbodyComponent cuerpo;
    private bool seleccionando;
    private float altura;

    public override void Start()
    {
        cuerpo = Entity.Get<RigidbodyComponent>();
        cuerpo.Collisions.CollectionChanged += CalcularColisiones;
    }

    public void Inicializar(ControladorCreación _controladorCreación)
    {
        controladorCreación = _controladorCreación;
    }

    private void CalcularColisiones(object sender, TrackingCollectionChangedEventArgs args)
    {
        // No cambia si acaba de seleccionar
        if (seleccionando)
        {
            seleccionando = false;
            return;
        }

        // PENDIENTE: Verifica altura
        var a = controladorCreación.ObtenerActual().EsPosibleColocar();

        // Ajusta trigger
        if (args.Action == NotifyCollectionChangedAction.Add)
            CambiarCuerpo(true);
        else if (args.Action == NotifyCollectionChangedAction.Remove)
            CambiarCuerpo(false);
    }

    private void CambiarCuerpo(bool agrandar)
    {
        if (agrandar && altura < 3)
            altura += 1;
        else if(!agrandar && altura > 0)
            altura -= 1;

        Entity.Transform.Scale = new Vector3(Entity.Transform.Scale.X, (altura + 1), Entity.Transform.Scale.Z);
    }

    public void Rotar()
    {
        Entity.Transform.Rotation *= Quaternion.RotationY(MathUtil.DegreesToRadians(-45));
    }

    public void ReiniciarCuerpo(TipoBloque tipoBloque, Quaternion rotación)
    {
        altura = 0;
        seleccionando = true;

        Entity.Transform.Rotation = rotación;
        switch (tipoBloque)
        {
            case TipoBloque.estatua:
                Entity.Transform.Scale = new Vector3(1.2f, 3, 1.2f);
                break;
            case TipoBloque.corto:
                Entity.Transform.Scale = new Vector3(1, 1, 1);
                break;
            case TipoBloque.largo:
                Entity.Transform.Scale = new Vector3(2, 1, 1);
                break;
        }
    }

    public void ActualizarPosición(Vector3 nuevaPosición)
    {
        nuevaPosición.Y = 0;
        Entity.Transform.Position = nuevaPosición;
    }

    public float ObtenerAltura()
    {
        return altura;
    }
}
