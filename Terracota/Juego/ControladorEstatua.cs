using System.Linq;
using System.Threading.Tasks;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Constantes;

public class ControladorEstatua : AsyncScript
{
    public TipoJugador jugador;
    public RigidbodyComponent cuerpo;
    public RigidbodyComponent cabeza;

    private ControladorPartidaLocal controladorPartida;
    private bool activo;

    public void Iniciar()
    {
        activo = true;
    }

    public override async Task Execute()
    {
        controladorPartida = Entity.Scene.Entities.FirstOrDefault(e => e.Name == "ControladorPartida").Get<ControladorPartidaLocal>();

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
        cuerpo.Mass = 1000;
        activo = false;

        controladorPartida.DesactivarEstatua(jugador);
    }
}
