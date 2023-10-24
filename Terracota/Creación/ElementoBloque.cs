using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;
using System.Linq;

namespace Terracota;
using static Constantes;

public class ElementoBloque : StartupScript
{
    public TipoBloque tipoBloque;
    public RigidbodyComponent cuerpo;

    public override void Start()
    {
        cuerpo.Collisions.CollectionChanged += CalcularColisiones;
    }

    private void CalcularColisiones(object sender, TrackingCollectionChangedEventArgs args)
    {
        /*
        // PENDIENTE: cambiar contorno o algo
        if (EsPosibleColocar())
            //efecto bueno
        else
            //efecto malo
        */
    }

    public void ActualizarPosición(Vector3 nuevaPosición, float altura)
    {
        Entity.Transform.Position = new Vector3(nuevaPosición.X, altura, nuevaPosición.Z);
    }

    public void ForzarPosición(Vector3 nuevaPosición)
    {
        Entity.Transform.Rotation = Quaternion.Identity;
        Entity.Transform.Position = nuevaPosición;
    }

    public bool EsPosibleColocar()
    {
        var colisionesSinBase = cuerpo.Collisions.Where(o => !o.ColliderA.Entity.Name.Contains("Sensor") && !o.ColliderB.Entity.Name.Contains("Sensor")).ToArray();
        return (colisionesSinBase.Length <= 0);
    }

    // Guardado
    public TipoBloque ObtenerTipo()
    {
        return tipoBloque;
    }

    public Vector3 ObtenerPosición()
    {
        return Entity.Transform.Position;
    }

    public Quaternion ObtenerRotación()
    {
        return Entity.Transform.Rotation;
    }
}
