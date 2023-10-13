using System;
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

    private ControladorPartidaLocal controladorPartida;

    private TransformComponent estatua;
    private Vector3 rotaciónAnterior;

    private bool activo;

    public override async Task Execute()
    {
        controladorPartida = SceneSystem.SceneInstance.RootScene.Entities.FirstOrDefault(e => e.Name == "ControladorPartida").Get<ControladorPartidaLocal>();
        var cuerpo = Entity.Get<RigidbodyComponent>();

        estatua = Entity.Transform;
        rotaciónAnterior = estatua.RotationEulerXYZ;

        activo = true;
        while (Game.IsRunning)
        {
            // Verifica ángulo después de una colisión
            if (activo)
            {
                await cuerpo.NewCollision();
                await VerificarÁngulo();
            }

            await Script.NextFrame();
        }
    }

    // PENDIENTE: inestable
    private async Task VerificarÁngulo()
    {
        // Espera a que se quede quieto
        while (estatua.RotationEulerXYZ != rotaciónAnterior)
        {
            rotaciónAnterior = estatua.RotationEulerXYZ;
            await Script.NextFrame();
        }

        // Verifica rotación
        estatua.UpdateLocalMatrix();
        if (CalcularRotación(estatua.RotationEulerXYZ.X) > 80 || CalcularRotación(estatua.RotationEulerXYZ.Z) > 80)
            DesactivarEstatua();
    }

    private float CalcularRotación(float ángulo)
    {
        var absoluto = MathF.Abs(MathUtil.RadiansToDegrees(ángulo));

        if (absoluto > 180)
            return MathF.Abs(absoluto - 180);
        else
            return absoluto;
    }

    private void DesactivarEstatua()
    {
        activo = false;
        controladorPartida.DesactivarEstatua(jugador);
    }
}
