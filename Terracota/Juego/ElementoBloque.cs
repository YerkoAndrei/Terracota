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

    private ElementoSonido elementoSonido;

    public override void Start()
    {
        elementoSonido = Entity.Get<ElementoSonido>();
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

    public void ActualizarFísicas(float[] matriz)
    {
        cuerpo.Entity.Transform.Position = new Vector3(matriz[1], matriz[2], matriz[3]);
        cuerpo.Entity.Transform.Rotation = new Quaternion(matriz[4], matriz[5], matriz[6], matriz[7]);

        // Sonido
        if (matriz[0] > 0)
            elementoSonido.SonarBloqueFísico(matriz[0]);
    }

    public float[] ObtenerFísicas()
    {
        // Anfitrión obtiene físicas
        // Huésped actualiza sin física
        return
        [
            elementoSonido.ObtenerFuerzaSonido(),
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
        ];
    }
}
