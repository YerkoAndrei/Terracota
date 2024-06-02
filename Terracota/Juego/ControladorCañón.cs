using System;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Particles.Components;
using System.Collections.Generic;
using System.Linq;

namespace Terracota;
using static Utilidades;
using static Constantes;

public class ControladorCañón : SyncScript
{
    public float fuerzaBola;
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

    private ControladorBola bola;
    private List<ControladorBola> metralla;

    private bool activo;
    private InterfazJuego interfaz;

    private float deltaXRotado;
    private float deltaYRotado;

    private bool deltaXPositivo;
    private bool deltaYPositivo;

    public override void Start() { }

    public void Inicializar(InterfazJuego _controladorInterfaz, TipoJugador tipoJugador)
    {
        fuerzaBola *= 1000;
        fuerzaMetralla *= 1000;

        interfaz = _controladorInterfaz;

        // Encuentra proyectiles en raíz escena
        bola = Entity.Scene.Entities.Where(o => o.Get<ControladorBola>() != null && o.Get<ControladorBola>().tipoProyectil == TipoProyectil.bola && o.Get<ControladorBola>().tipoJugador == tipoJugador).FirstOrDefault().Get<ControladorBola>();

        metralla = new List<ControladorBola>();
        var metrallaEncontrada = Entity.Scene.Entities.Where(o => o.Get<ControladorBola>() != null && o.Get<ControladorBola>().tipoProyectil == TipoProyectil.metralla && o.Get<ControladorBola>().tipoJugador == tipoJugador).ToArray();
        foreach(var m in metrallaEncontrada)
        {
            metralla.Add(m.Get<ControladorBola>());
        }
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
            SonarX(Input.MouseDelta.X);
        }

        if (Input.MouseDelta.Y != 0 && ánguloX < 60 && ánguloX > -40)
        {
            cañón.Transform.RotationEulerXYZ += new Vector3(-(Input.MouseDelta.Y * sensibilidadY), 0, 0);
            SonarY(Input.MouseDelta.Y);
        }
    }

    private void MoverCañónMóvil()
    {
        // PENDIENTE 2.0: móvil
    }

    public void Disparar(TipoProyectil tipoProyectil, float multiplicador)
    {
        switch (tipoProyectil)
        {
            case TipoProyectil.bola:
                // Impulso
                var aleatoriedadBola = RangoAleatorio(-0.5f, 0.5f);
                var fuerza = origenProyectil.Transform.WorldMatrix.Up * ((fuerzaBola * multiplicador) + aleatoriedadBola);

                bola.Disparar(origenProyectil.Transform.WorldMatrix.TranslationVector, EulerAleatorio(), aleatoriedadBola, fuerza);
                break;
            case TipoProyectil.metralla:
                foreach (var bola in metralla)
                {
                    // Impulso
                    var aleatoriedadMetralla = RangoAleatorio(-1f, 0.5f);
                    var posiciónAleatoria = RangoAleatorio(-0.02f, 0.02f);
                    var fuerzaIndividual = (origenProyectil.Transform.WorldMatrix.Up + posiciónAleatoria) * ((fuerzaMetralla * multiplicador) + aleatoriedadMetralla);

                    bola.Disparar(origenProyectil.Transform.WorldMatrix.TranslationVector, EulerAleatorio(), aleatoriedadMetralla, fuerzaIndividual);
                }
                break;
        }

        // Partículas
        ActivarPartículas();
    }

    public void ActivarPartículas()
    {
        if (!partículasFuego.Enabled)
            partículasFuego.Enabled = true;

        partículasFuego.ParticleSystem.ResetSimulation();
        partículasFuego.ParticleSystem.Play();

        if (!partículasHumo.Enabled)
            partículasHumo.Enabled = true;

        partículasHumo.ParticleSystem.ResetSimulation();
        partículasHumo.ParticleSystem.Play();
    }

    private void SonarX(float delta)
    {
        // Reinicia si cambia dirección
        var positivo = delta > 0;
        if (deltaXPositivo != positivo)
        {
            deltaXPositivo = positivo;
            deltaXRotado = 0;
        }

        // Suena según sensibilidad
        deltaXRotado += MathF.Abs(delta);
        if (deltaXRotado >= 0.01f)
        {
            deltaXRotado = 0;
            SistemaSonido.SonarCañónHorizontal();
        }
    }

    private void SonarY(float delta)
    {
        // Reinicia si cambia dirección
        var positivo = delta > 0;
        if (deltaYPositivo != positivo)
        {
            deltaYPositivo = positivo;
            deltaYRotado = 0;
        }

        // Suena según sensibilidad
        deltaYRotado += MathF.Abs(delta);
        if (deltaYRotado >= 0.01f)
        {
            deltaYRotado = 0;
            SistemaSonido.SonarCañónVertical();
        }
    }

    // Red
    public void ActualizarRotación(float[] matriz)
    {
        cañón.Transform.Rotation = new Quaternion(matriz[0], matriz[1], matriz[2], matriz[3]);
        soporte.Transform.Rotation = new Quaternion(matriz[4], matriz[5], matriz[6], matriz[7]);
    }

    public float[] ObtenerMatriz()
    {
        return new float[]
        {
            cañón.Transform.Rotation.X,
            cañón.Transform.Rotation.Y,
            cañón.Transform.Rotation.Z,
            cañón.Transform.Rotation.W,
            soporte.Transform.Rotation.X,
            soporte.Transform.Rotation.Y,
            soporte.Transform.Rotation.Z,
            soporte.Transform.Rotation.W
        };
    }
}
