using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Physics;
using Stride.Particles.Components;

namespace Terracota;
using static Constantes;
using static Sistema;

public class ControladorCañón : SyncScript
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

    public ParticleSystemComponent partículasFuego;
    public ParticleSystemComponent partículasHumo;

    public Prefab bala;
    public Prefab metralla;

    private bool activo;
    private InterfazJuego interfaz;

    public override void Start() { }

    public void Iniciar(InterfazJuego _controladorInterfaz)
    {
        fuerzaBala *= 1000;
        fuerzaMetralla *= 1000;

        interfaz = _controladorInterfaz;
    }

    public override void Update()
    {
        if (!activo || interfaz.ObtenerPausa())
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
        // PENDIENTE: móvil
    }

    public void Disparar(TipoProyectil tipoProyectil, float multiplicador)
    {        
        switch (tipoProyectil)
        {
            case TipoProyectil.bola:
                var nuevaBala = bala.Instantiate()[0];
                nuevaBala.Transform.Position = origenProyectil.Transform.WorldMatrix.TranslationVector;
                nuevaBala.Transform.RotationEulerXYZ = EulerAleatorio();

                var cuerpo = nuevaBala.Get<RigidbodyComponent>();
                Entity.Scene.Entities.Add(nuevaBala);

                // Impulso
                var aleatoriedadBala = RangoAleatorio(-0.5f, 0.5f);
                cuerpo.Mass += aleatoriedadBala;
                cuerpo.ApplyForce(origenProyectil.Transform.WorldMatrix.Up * ((fuerzaBala * multiplicador) + aleatoriedadBala));
                break;
            case TipoProyectil.metralla:
                var nuevaMetralla = metralla.Instantiate();
                foreach(var metralla in nuevaMetralla)
                {
                    metralla.Transform.Position += origenProyectil.Transform.WorldMatrix.TranslationVector;
                    metralla.Transform.RotationEulerXYZ = EulerAleatorio();

                    var cuerpoMetralla = metralla.Get<RigidbodyComponent>();
                    Entity.Scene.Entities.Add(metralla);

                    // Impulso
                    var aleatoriedadMetralla = RangoAleatorio(-1f, 0.5f);
                    var posiciónAleatoria = RangoAleatorio(-0.02f, 0.02f);
                    cuerpoMetralla.Mass += aleatoriedadMetralla;
                    cuerpoMetralla.ApplyForce((origenProyectil.Transform.WorldMatrix.Up + posiciónAleatoria) * ((fuerzaMetralla * multiplicador) + aleatoriedadMetralla));
                }
                break;
        }

        // Partículas
        if (!partículasFuego.Enabled)
            partículasFuego.Enabled = true;

        partículasFuego.ParticleSystem.ResetSimulation();
        partículasFuego.ParticleSystem.Play();

        if (!partículasHumo.Enabled)
            partículasHumo.Enabled = true;

        partículasHumo.ParticleSystem.ResetSimulation();
        partículasHumo.ParticleSystem.Play();        
    }
}
