using System.Linq;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Constantes;

public class ControladorEstatua : AsyncScript
{
    public TipoJugador jugador;
    public RigidbodyComponent cuerpo;
    public RigidbodyComponent cabeza;
    public RigidbodyComponent lanza;

    private IPartida iPartida;
    private bool activo;

    public void Iniciar()
    {
        activo = true;
    }

    public override async Task Execute()
    {
        // Encuentra interface
        var controlador = Entity.Scene.Entities.FirstOrDefault(e => e.Name == "ControladorPartida");
        foreach (var componente in controlador.Components)
        {
            if (componente is IPartida)
            {
                iPartida = (IPartida)componente;
                break;
            }
        }

        while (Game.IsRunning)
        {
            if (activo)
            {
                // Cabeza toca el suelo
                await cabeza.NewCollision();
                DesactivarEstatua();
            }
            await Script.NextFrame();
        }
    }

    private void DesactivarEstatua()
    {
        cuerpo.Restitution = 0;
        lanza.Restitution = 0;
        cuerpo.Mass = 1000;

        activo = false;

        iPartida.DesactivarEstatua(jugador);
    }
}
