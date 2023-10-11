using System;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Physics;

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
    public Entity tubo;
    public Entity origenProyectil;
    public Prefab bala;
    public Prefab metralla;

    private bool activo;

    public override void Start()
    {

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

        if (Input.MouseDelta.X != 0)
            soporte.Transform.RotationEulerXYZ += new Vector3(0, -(Input.MouseDelta.X * sensibilidadX), 0);

        if (Input.MouseDelta.Y != 0)
            tubo.Transform.RotationEulerXYZ += new Vector3(-(Input.MouseDelta.Y * sensibilidadY), 0, 0);
    }

    private void MoverCañónMóvil()
    {

    }

    public void Disparar(TipoProyectil tipoProyectil)
    {
        var aleatorio = new Random();
        
        switch (tipoProyectil)
        {
            case TipoProyectil.bola:
                var nuevaBala = bala.Instantiate()[0];
                nuevaBala.Transform.Position = origenProyectil.Transform.WorldMatrix.TranslationVector;
                nuevaBala.Transform.RotationEulerXYZ = new Vector3(aleatorio.Next(0, 360), aleatorio.Next(0, 360), aleatorio.Next(0, 360));

                SceneSystem.SceneInstance.RootScene.Entities.Add(nuevaBala);

                // Impulso
                var cuerpo = nuevaBala.Get<RigidbodyComponent>();
                cuerpo.ApplyForce(origenProyectil.Transform.WorldMatrix.Up * fuerzaBala);
                break;
            case TipoProyectil.metralla:
                var nuevaMetralla = metralla.Instantiate()[0];
                nuevaMetralla.Transform.Position = origenProyectil.Transform.WorldMatrix.TranslationVector;
                nuevaMetralla.Transform.RotationEulerXYZ = new Vector3(aleatorio.Next(0, 360), aleatorio.Next(0, 360), aleatorio.Next(0, 360));

                SceneSystem.SceneInstance.RootScene.Entities.Add(nuevaMetralla);

                var cuerposMetralla = nuevaMetralla.GetChildren();
                foreach (var metralla in cuerposMetralla)
                {
                    var fuerzaAleatoria = RangoAleatorio(-1f, 0.5f);
                    var cuerpoMetralla = metralla.Get<RigidbodyComponent>();
                    cuerpoMetralla.Mass += fuerzaAleatoria;

                    // Impulso
                    cuerpoMetralla.ApplyForce(origenProyectil.Transform.WorldMatrix.Up * (fuerzaMetralla + fuerzaAleatoria));
                }
                break;
        }
    }
}
