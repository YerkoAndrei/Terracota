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

    public void PosicionarFísica(BloqueFísico bloque)
    {
        cuerpo.Entity.Transform.Position = new Vector3 (bloque.Posición[0], bloque.Posición[1], bloque.Posición[2]);
        cuerpo.Entity.Transform.Rotation = bloque.Rotación;
    }

    public BloqueFísico ObtenerFísicas()
    {
        // Anfitrión obtiene físicas
        // Huesped actualiza sin física
        return new BloqueFísico
        {
            Posición = new float[] { cuerpo.Entity.Transform.Position.X, cuerpo.Entity.Transform.Position.Y, cuerpo.Entity.Transform.Position.Z },
            Rotación = cuerpo.Entity.Transform.Rotation,
        };
    }
}
