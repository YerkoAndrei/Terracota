using System.Linq;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Constantes;

public class ElementoCreación : StartupScript
{
    public TipoBloque tipoBloque;
    public RigidbodyComponent cuerpo;

    private string número;
    private Vector3 posiciónInicial;
    private Quaternion rotaciónInicial;
    private Vector3 posiciónFortaleza;

    public override void Start()
    {
        // Obtiene último dígito de la entidad
        número = Entity.Name[^1].ToString();

        cuerpo.Collisions.CollectionChanged += CalcularColisiones;
        posiciónInicial = ObtenerPosición();
        rotaciónInicial = ObtenerRotación();
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

    public void ActualizarPosición(Vector3 nuevaPosición, int altura)
    {
        Entity.Transform.Position = new Vector3(nuevaPosición.X, altura, nuevaPosición.Z);
    }

    public void ReiniciarTransform()
    {
        Entity.Transform.Position = posiciónInicial;
        Entity.Transform.Rotation = rotaciónInicial;
    }

    public bool EsPosibleColocar()
    {        
        var colisionesSinBase = cuerpo.Collisions.Where(o => !o.ColliderA.Entity.Name.Contains("Sensor") &&
                                                             !o.ColliderB.Entity.Name.Contains("Sensor") &&
                                                             !o.ColliderA.Entity.Name.Contains("Fortaleza") &&
                                                             !o.ColliderB.Entity.Name.Contains("Fortaleza")).ToArray();
        return (colisionesSinBase.Length <= 0);
    }

    public void AsignarFortaleza(Vector3 fortaleza)
    {
        posiciónFortaleza = fortaleza;
    }

    public string ObtenerNúmero()
    {
        return número;
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

    public Vector3 ObtenerPosiciónRelativa()
    {
        // Posición relativa a posición fortaleza
        return Entity.Transform.Position - posiciónFortaleza;
    }

    public Quaternion ObtenerRotación()
    {
        return Entity.Transform.Rotation;
    }
}
