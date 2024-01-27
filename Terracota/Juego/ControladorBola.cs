using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Sistema;
using static Constantes;

public class ControladorBola : AsyncScript
{
    public TipoProyectil tipoProyectil;
    public TipoJugador tipoJugador;
    public int maxColisiones;
    public Prefab prefabPartículas;

    private RigidbodyComponent cuerpo;
    private Vector3 posiciónInicial;
    private Vector3 escalaInicial;
    private float masaInicial;
    private float duraciónGuardado;
    private int colisiones;
    private bool activo;
    private bool guardando;

    private float fuerzaSonido;

    public override async Task Execute()
    {
        cuerpo = Entity.Get<RigidbodyComponent>();
        duraciónGuardado = 0.7f;

        // Valores iniciales
        posiciónInicial = Entity.Transform.Position;
        escalaInicial = Entity.Transform.Scale;
        masaInicial = cuerpo.Mass;

        _ = Ocultar();

        while (Game.IsRunning)
        {
            if (activo)
            {
                var colisión = await cuerpo.NewCollision();

                // Metralla con metralla no cuenta como colisión
                if ((colisión.ColliderA == cuerpo && colisión.ColliderB.Entity.Get<ControladorBola>() == null) ||
                   (colisión.ColliderB == cuerpo && colisión.ColliderA.Entity.Get<ControladorBola>() == null))
                {
                    colisiones++;
                    MostrarEfectos();

                    // Solo suena cuando colisiona con estáticos
                    if (colisión.ColliderA.Entity.Get<ElementoSonido>() == null && colisión.ColliderB.Entity.Get<ElementoSonido>() == null)
                    {
                        fuerzaSonido = ObtenerMayorFuerzaLinearNormalizada();
                        SistemaSonido.SonarBola(fuerzaSonido);
                    }
                }

                // Evita colisiones innesesarias
                if (colisiones >= maxColisiones)
                    await Guardar();
            }
            await Script.NextFrame();
        }
    }

    public async void Disparar(Vector3 posición, Vector3 rotación, float aleatorio, Vector3 fuerza)
    {
        // Espera posible diferencia
        if(activo)
            await Ocultar();

        activo = true;
        guardando = false;
        colisiones = 0;

        Entity.Transform.Position = posiciónInicial + posición;
        Entity.Transform.RotationEulerXYZ = rotación;
        Entity.Transform.Scale = escalaInicial;

        cuerpo.Enabled = true;
        cuerpo.IsKinematic = false;
        cuerpo.UpdatePhysicsTransformation();

        cuerpo.Mass = masaInicial + aleatorio;
        cuerpo.ApplyForce(fuerza);

        // Tiempo de vida
        _ = ContarVida();
    }

    private void MostrarEfectos()
    {
        var partícula = prefabPartículas.Instantiate()[0];
        switch (tipoProyectil)
        {
            case TipoProyectil.bola:
                partícula.Transform.Scale *= 1.1f;
                break;
            case TipoProyectil.metralla:
                partícula.Transform.Scale *= 0.4f;
                break;
        }

        partícula.Transform.Position = Entity.Transform.Position;
        Entity.Scene.Entities.Add(partícula);

        // Posterior destrucción
        _ = ContarVidaPartícula(partícula);
    }

    private async Task ContarVidaPartícula(Entity partícula)
    {
        // Duración partícula
        await Task.Delay(1000);
        Entity.Scene.Entities.Remove(partícula);
    }

    private async Task ContarVida()
    {
        // Duración según tipo de juego
        // Remoto = (duraciónTurnoLocal - duraciónGuardado) - desface;
        var duración = duraciónTurnoLocal;
        if (SistemaRed.ObtenerJugando())
        {
            var tiempoGuardado = (int)(duraciónGuardado * 1000f);
            duración -= (tiempoGuardado + 200);
        }

        await Task.Delay(duración);
        await Guardar();
    }

    private async Task Guardar()
    {
        if (guardando) return;
        guardando = true;

        float duraciónLerp = duraciónGuardado;
        float tiempoLerp = 0;
        float tiempo = 0;

        var inicial = Entity.Transform.Scale;
        while (tiempoLerp < duraciónLerp && activo)
        {
            tiempo = SistemaAnimación.EvaluarRápido(tiempoLerp / duraciónLerp);

            Entity.Transform.Scale = Vector3.Lerp(inicial, Vector3.Zero, tiempo);
            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Task.Delay(1);
        }

        // Fin
        if (activo)
            await Ocultar();
    }

    private async Task Ocultar()
    {
        activo = false;
        cuerpo.IsKinematic = true;
        cuerpo.Enabled = false;

        Entity.Transform.Scale = Vector3.Zero;
        Entity.Transform.Position = posiciónInicial - Vector3.UnitY;
        await Task.Delay(2);
    }

    public float ObtenerMayorFuerzaLinearNormalizada()
    {
        // Encuentra mayor velocidad normalizada
        var velocidad = cuerpo.LinearVelocity;
        velocidad.Normalize();

        switch (tipoProyectil)
        {
            default:
            case TipoProyectil.bola:
                return ObtenerMayorValor(velocidad);
            case TipoProyectil.metralla:
                return ObtenerMayorValor(velocidad) * 0.4f;
        }
    }

    public void ActualizarFísicas(float[] matriz)
    {
        cuerpo.Entity.Transform.Position = new Vector3(matriz[1], matriz[2], matriz[3]);
        cuerpo.Entity.Transform.Rotation = new Quaternion(matriz[4], matriz[5], matriz[6], matriz[7]);
        cuerpo.Entity.Transform.Scale = new Vector3(matriz[8], matriz[9], matriz[10]);

        // Sonido
        if(matriz[0] > 0)
            SistemaSonido.SonarBola(fuerzaSonido);
    }

    public float[] ObtenerFísicas()
    {
        var sonido = fuerzaSonido;
        fuerzaSonido = 0;

        // Anfitrión obtiene físicas
        // Huésped actualiza sin física
        return
        [
            sonido,
            cuerpo.Entity.Transform.Position.X,
            cuerpo.Entity.Transform.Position.Y,
            cuerpo.Entity.Transform.Position.Z,
            cuerpo.Entity.Transform.Rotation.X,
            cuerpo.Entity.Transform.Rotation.Y,
            cuerpo.Entity.Transform.Rotation.Z,
            cuerpo.Entity.Transform.Rotation.W,
            cuerpo.Entity.Transform.Scale.X,
            cuerpo.Entity.Transform.Scale.Y,
            cuerpo.Entity.Transform.Scale.Z
        ];
    }
}
