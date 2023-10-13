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
    private float contador;
    private float tiempoCaída;

    public override async Task Execute()
    {
        controladorPartida = SceneSystem.SceneInstance.RootScene.Entities.FirstOrDefault(e => e.Name == "ControladorPartida").Get<ControladorPartidaLocal>();
        var cuerpo = Entity.Get<RigidbodyComponent>();

        estatua = Entity.Transform;
        rotaciónAnterior = estatua.RotationEulerXYZ;

        tiempoCaída = 3;
        contador = tiempoCaída;
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

    private async Task VerificarÁngulo()
    {
        while (estatua.RotationEulerXYZ != rotaciónAnterior && contador > 0 && activo)
        {
            // Verifica rotación respecto al suelo
            estatua.UpdateLocalMatrix();
            var rotación = estatua.RotationEulerXYZ;

            // PENDIENTE: falsos positivos, no se cumplen los 3 segundos
            if (CalcularRotación(rotación.X) > 80 || CalcularRotación(rotación.Z) > 80)
            {
                // Debe estar cierto tiempo en el suelo para que tome en cuenta
                contador -= (float)Game.UpdateTime.Elapsed.TotalSeconds;
                if (contador <= tiempoCaída)
                    DesactivarEstatua();
            }
            
            rotaciónAnterior = estatua.RotationEulerXYZ;
            await Script.NextFrame();
        }
        contador = tiempoCaída;
    }
    
    private float CalcularRotación(float ángulo)
    {
        var absoluto = MathF.Abs(MathUtil.RadiansToDegrees(ángulo));

        if (absoluto > 270)
            return MathF.Abs(absoluto - 360);
        else
            return absoluto;
    }

    private void DesactivarEstatua()
    {
        activo = false;
        controladorPartida.DesactivarEstatua(jugador);
    }
}
