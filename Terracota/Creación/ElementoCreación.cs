using System.Linq;
using System.Collections.Generic;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;
using Stride.Rendering;

namespace Terracota;
using static Constantes;

public class ElementoCreación : StartupScript
{
    public TipoBloque tipoBloque;
    public RigidbodyComponent cuerpo;
    public Material materialNegativo;
    public List<ModelComponent> modelos = new List<ModelComponent> { };

    private bool moviendo;
    private string número;
    private Vector3 posiciónInicial;
    private Quaternion rotaciónInicial;
    private Vector3 posiciónFortaleza;

    private Dictionary<string, MaterialInstance> materiales;

    public override void Start()
    {
        // Obtiene último dígito de la entidad
        número = Entity.Name[^1].ToString();

        cuerpo.Collisions.CollectionChanged += CalcularColisiones;
        posiciónInicial = ObtenerPosición();
        rotaciónInicial = ObtenerRotación();
                
        // Obtiene materiales (Deben estar marcados en instancia, no modelos)
        materiales = new Dictionary<string, MaterialInstance>();
        foreach (var modelo in modelos)
        {
            foreach (var material in modelo.Materials)
            {
                // Código compuesto por nombre + nombre raíz
                var código = Entity.Name + "_" + modelo.Entity.Name + "_" + material.Key;
                materiales.Add(código, material.Value);
            }
        }
    }

    private void CalcularColisiones(object sender, TrackingCollectionChangedEventArgs args)
    {
        // Solo marca el bloque actual
        if(!moviendo)
            return;

        if (EsPosibleColocar())
        {
            foreach (var modelo in modelos)
            {
                // Foreach lee Materials como KeyValuePair, pero for lo lee como IndexingDictionary
                int i = 0;
                foreach (var material in modelo.Materials)
                {
                    // Código compuesto por nombre + nombre raíz
                    var código = Entity.Name + "_" + modelo.Entity.Name + "_" + material.Key;
                    modelo.Materials[i] = materiales.GetValueOrDefault(código).Material;
                    i++;
                }
            }
        }
        else
        {
            for (int i = 0; i < modelos.Count; i++)
            {
                for (int ii = 0; ii < modelos[i].Materials.Count; ii++)
                {
                    modelos[i].Materials[ii] = materialNegativo;
                }
            }
        }
    }

    public void ActualizarPosición(Vector3 nuevaPosición, int altura)
    {
        moviendo = true;
        Entity.Transform.Position = new Vector3(nuevaPosición.X, altura, nuevaPosición.Z);
    }

    public void ForzarPosición(Vector3 posición, Quaternion rotación)
    {
        moviendo = false;
        Entity.Transform.Position = posición;
        Entity.Transform.Rotation = rotación;
    }

    public void ReiniciarTransform()
    {
        moviendo = false;
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

    public void Colocar()
    {
        moviendo = false;
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
