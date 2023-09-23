using System;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Physics;
using Stride.Core;

namespace Terracota;
using static Constantes;

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

    private bool active;

    public override void Start()
    {
        // Turnos
        active = true;
    }

    public override void Update()
    {
        if (!active)
            return;

        if (Input.HasMouse)
            MoverCañónPC();
        else
            MoverCañónMóvil();

        // pruebas
        if (Input.IsKeyPressed(Keys.Z))
            Disparar(TipoProyectil.bala);

        if (Input.IsKeyPressed(Keys.X))
            Disparar(TipoProyectil.metralla);
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

    private void Disparar(TipoProyectil tipoProyectil)
    {
        Console.WriteLine(tipoProyectil);
        var aleatorio = new Random();
        
        switch (tipoProyectil)
        {
            case TipoProyectil.bala:
                var nuevaBala = bala.Instantiate()[0];
                nuevaBala.Transform.Position = origenProyectil.Transform.WorldMatrix.TranslationVector;
                nuevaBala.Transform.RotationEulerXYZ = new Vector3(aleatorio.Next(0, 360), aleatorio.Next(0, 360), aleatorio.Next(0, 360));

                SceneSystem.SceneInstance.RootScene.Entities.Add(nuevaBala);

                // Impulso
                var cuerpo = nuevaBala.Get<RigidbodyComponent>();
                cuerpo.ApplyForce(origenProyectil.Transform.WorldMatrix.Up * fuerzaBala);
                break;
            case TipoProyectil.metralla:/*
                var nuevaMetralla = Instantiate(metralla, origenProyectil.position, origenProyectil.rotation);
                var cuerposMetralla = nuevaMetralla.GetComponentsInChildren<Rigidbody>();

                foreach (var metralla in cuerposMetralla)
                {
                    var aleatorio = Random.Range(-1f, 0.5f);
                    metralla.mass += aleatorio;
                    metralla.AddForce(origenProyectil.up * (fuerzaMetralla + aleatorio), ForceMode.Impulse);
                }*/
                break;
        }
    }
}
