using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Constantes;

public class ElementoBloque : StartupScript
{
    public TipoBloque tipoBloque;
    public RigidbodyComponent cuerpo;

    // Posición hijo
    private Vector3 posiciónCuerpo;
    private string código;

    public override void Start()
    {
        posiciónCuerpo = cuerpo.Entity.Transform.Position;
        cuerpo.IsKinematic = true;
        cuerpo.Enabled = false;
    }

    public void CrearCódigo(string _código)
    {
        código = _código;
    }

    public string ObtenerCódigo()
    {
        return código;
    }

    public void Activar()
    {
        cuerpo.Entity.Transform.Position = posiciónCuerpo;
        cuerpo.IsKinematic = false;
        cuerpo.Enabled = true;
    }

    public void Posicionar(Vector3 posición, Quaternion rotación)
    {
        Entity.Transform.Position = posición;
        Entity.Transform.Rotation = rotación;
    }

    public void PosicionarFísica(float[] matriz)
    {
        cuerpo.Entity.Transform.Position = new Vector3(matriz[0], matriz[1], matriz[2]);
        cuerpo.Entity.Transform.Rotation = new Quaternion(matriz[3], matriz[4], matriz[5], matriz[6]);
    }

    public float[] ObtenerFísicas()
    {
        // Anfitrión obtiene físicas
        // Huesped actualiza sin física
        return new float[]
        {
            cuerpo.Entity.Transform.Position.X,
            cuerpo.Entity.Transform.Position.Y,
            cuerpo.Entity.Transform.Position.Z,
            cuerpo.Entity.Transform.Rotation.X,
            cuerpo.Entity.Transform.Rotation.Y,
            cuerpo.Entity.Transform.Rotation.Z,
            cuerpo.Entity.Transform.Rotation.W,
            cuerpo.Entity.Transform.Scale.X,
            cuerpo.Entity.Transform.Scale.Y,
            cuerpo.Entity.Transform.Scale.Z
        };
    }
}
