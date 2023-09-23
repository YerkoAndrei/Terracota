using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using System.Collections;
using Stride.Core.Diagnostics;
using Silk.NET.SDL;
using BulletSharp;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace Terracota;
using static Constantes;

public class ControladorCañon : SyncScript
{
    public float fuerzaBala;
    public float fuerzaMetralla;

    public Entity soporte;
    public Entity tubo;
    public Entity origenProyectil;
    public Prefab bala;
    public Prefab metralla;

    private float últimaPosiciónX;
    private float últimaPosiciónY;

    private float aaa;
    public override void Start()
    {

    }

    public override void Update()
    {
        Mirar();

        if (!Input.HasMouse)
            return;

        if (Input.IsKeyPressed(Keys.Z))
            Disparar(TipoProyectil.bala);

        if (Input.IsKeyPressed(Keys.X))
            Disparar(TipoProyectil.metralla);
    }
    
    private void Mirar()
    {
        Console.WriteLine(soporte.Transform.RotationEulerXYZ);

        if (últimaPosiciónX != Input.AbsoluteMousePosition.X)
            soporte.Transform.Rotation = Quaternion.RotationY((últimaPosiciónX - Input.AbsoluteMousePosition.X) / 10);

        if (últimaPosiciónY != Input.AbsoluteMousePosition.Y)
            tubo.Transform.Rotation = Quaternion.RotationX((últimaPosiciónY - Input.AbsoluteMousePosition.Y) / 10);

        últimaPosiciónX = Input.AbsoluteMousePosition.X;
        últimaPosiciónY = Input.AbsoluteMousePosition.Y;
    }
        
    private void Disparar(TipoProyectil tipoProyectil)
    {
        Console.WriteLine(tipoProyectil);
        /*
        switch (tipoProyectil)
        {
            case TipoProyectil.bala:
                var nuevaBala = Instantiate(bala, origenProyectil.position, origenProyectil.rotation);
                var cuerpoBala = nuevaBala.GetComponent<Rigidbody>();

                cuerpoBala.AddForce(origenProyectil.up * fuerzaBala, ForceMode.Impulse);
                break;
            case TipoProyectil.metralla:
                var nuevaMetralla = Instantiate(metralla, origenProyectil.position, origenProyectil.rotation);
                var cuerposMetralla = nuevaMetralla.GetComponentsInChildren<Rigidbody>();

                foreach (var metralla in cuerposMetralla)
                {
                    var aleatorio = Random.Range(-1f, 0.5f);
                    metralla.mass += aleatorio;
                    metralla.AddForce(origenProyectil.up * (fuerzaMetralla + aleatorio), ForceMode.Impulse);
                }
                break;
        }*/
    }
}
