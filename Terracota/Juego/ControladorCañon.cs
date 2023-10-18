using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Physics;
using Stride.Particles;
using Stride.Particles.Components;

namespace Terracota;
using static Constantes;
using static Sistema;

public class ControladorCañon : SyncScript
{
    public float fuerzaBala;
    public float fuerzaMetralla;

    public float sensibilidadX;
    public float sensibilidadY;

    public Entity soporte;
    public Entity cañón;
    public Entity origenProyectil;

    public Entity ruedaIzquierda;
    public Entity ruedaDerecha;

    public ParticleSystemComponent partículasHumo;

    public Prefab bala;
    public Prefab metralla;

    private bool activo;

    public override void Start()
    {
        fuerzaBala *= 1000;
        fuerzaMetralla *= 1000;
    }

    public override void Update()
    {
        if (!activo)
            return;

        if (Input.HasMouse)
            MoverCañónPC();
        else
            MoverCañónMóvil();
    }

    public void Activar(bool activar)
    {
        activo = activar;
    }

    private void MoverCañónPC()
    {
        if (!Input.IsMouseButtonDown(MouseButton.Left))
            return;

        // Restricción
        var ánguloX = MathUtil.RadiansToDegrees(cañón.Transform.RotationEulerXYZ.X + -(Input.MouseDelta.Y * sensibilidadY));
        var ánguloY = MathUtil.RadiansToDegrees(soporte.Transform.RotationEulerXYZ.Y + -(Input.MouseDelta.X * sensibilidadX));

        if (Input.MouseDelta.X != 0 && ánguloY < 60 && ánguloY > -60)
        {
            soporte.Transform.RotationEulerXYZ += new Vector3(0, -(Input.MouseDelta.X * sensibilidadX), 0);
            ruedaIzquierda.Transform.RotationEulerXYZ += new Vector3(-(Input.MouseDelta.X * sensibilidadX), 0, 0);
            ruedaDerecha.Transform.RotationEulerXYZ += new Vector3((Input.MouseDelta.X * sensibilidadX), 0, 0);
        }

        if (Input.MouseDelta.Y != 0 && ánguloX < 60 && ánguloX > -40)
            cañón.Transform.RotationEulerXYZ += new Vector3(-(Input.MouseDelta.Y * sensibilidadY), 0, 0);
    }

    private void MoverCañónMóvil()
    {

    }

    public void Disparar(TipoProyectil tipoProyectil, float multiplicador)
    {        
        switch (tipoProyectil)
        {
            case TipoProyectil.bola:
                var nuevaBala = bala.Instantiate()[0];
                nuevaBala.Transform.Position = origenProyectil.Transform.WorldMatrix.TranslationVector;
                nuevaBala.Transform.RotationEulerXYZ = EulerAleatorio();

                SceneSystem.SceneInstance.RootScene.Entities.Add(nuevaBala);

                // Impulso
                var cuerpo = nuevaBala.Get<RigidbodyComponent>();
                cuerpo.ApplyForce(origenProyectil.Transform.WorldMatrix.Up * (fuerzaBala * multiplicador));
                break;
            case TipoProyectil.metralla:
                var nuevaMetralla = metralla.Instantiate()[0];
                nuevaMetralla.Transform.Position = origenProyectil.Transform.WorldMatrix.TranslationVector;
                nuevaMetralla.Transform.RotationEulerXYZ = EulerAleatorio();

                SceneSystem.SceneInstance.RootScene.Entities.Add(nuevaMetralla);

                var cuerposMetralla = nuevaMetralla.GetChildren();
                foreach (var metralla in cuerposMetralla)
                {
                    var fuerzaAleatoria = RangoAleatorio(-1f, 0.5f);
                    var cuerpoMetralla = metralla.Get<RigidbodyComponent>();
                    cuerpoMetralla.Mass += fuerzaAleatoria;

                    // Impulso
                    cuerpoMetralla.ApplyForce(origenProyectil.Transform.WorldMatrix.Up * ((fuerzaMetralla * multiplicador) + fuerzaAleatoria));
                }
                break;
        }

        // Partículas
        if (!partículasHumo.Enabled)
            partículasHumo.Enabled = true;

        partículasHumo.ParticleSystem.ResetSimulation();
        partículasHumo.ParticleSystem.Play();
    }
}
