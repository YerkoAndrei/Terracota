using System;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;

public class ControladorEstatua : AsyncScript
{
    private TransformComponent estatua;
    private Vector3 posiciónAnterior;

    private bool activo;
    private float contador;
    private float tiempoCaída;

    public override async Task Execute()
    {
        var cuerpo = Entity.Get<RigidbodyComponent>();

        estatua = Entity.Transform;
        posiciónAnterior = estatua.WorldMatrix.TranslationVector;

        tiempoCaída = 3;
        contador = tiempoCaída;
        activo = true;

        while (Game.IsRunning && activo)
        {
            // Verifica ángulo después de una colisión
            await cuerpo.NewCollision();
            await VerificarÁngulo();

            await Script.NextFrame();
        }
    }

    // PENDIENTE: nada de esto funciona
    private async Task VerificarÁngulo()
    {
        while (estatua.WorldMatrix.TranslationVector != posiciónAnterior)
        {
            // Verifica rotación respecto al suelo
            estatua.UpdateLocalMatrix();
            var rotación = estatua.RotationEulerXYZ;

            if (CalcularRotación(rotación.X) > 70 || CalcularRotación(rotación.Z) > 70)
            {
                // Debe estar cierto tiempo en el suelo para que tome en cuenta
                contador -= (float)Game.UpdateTime.Elapsed.TotalSeconds;
                if (contador <= tiempoCaída)
                    DesactivarEstatua();
            }

            posiciónAnterior = estatua.WorldMatrix.TranslationVector;
            await Script.NextFrame();
        }
        contador = tiempoCaída;
    }

    private float CalcularRotación(float ángulo)
    {
        var absoluto = MathF.Abs(ángulo);

        if (absoluto > 270)
            return MathF.Abs(absoluto - 360);
        else
            return absoluto;
    }

    private void DesactivarEstatua()
    {
        activo = false;
        Console.WriteLine("me morí");
    }
}
