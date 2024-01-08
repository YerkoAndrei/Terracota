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
    private Vector3 escalaInicial;
    private float masaInicial;
    private bool activo;
    private bool destruyendo;
    private int colisiones;

    public void Disparar(Vector3 posición, Vector3 rotación, float aleatorio, Vector3 fuerza)
    {
        activo = true;
        destruyendo = false;

        Entity.Transform.Position = posición;
        Entity.Transform.RotationEulerXYZ = rotación;
        Entity.Transform.Scale = escalaInicial;

        cuerpo.Enabled = true;
        cuerpo.IsKinematic = false;

        cuerpo.Mass = masaInicial + aleatorio;
        cuerpo.ApplyForce(fuerza);

        // Tiempo de vida
        _ = ContarVida();
    }

    public override async Task Execute()
    {
        activo = false;

        cuerpo = Entity.Get<RigidbodyComponent>();
        cuerpo.IsKinematic = true;
        cuerpo.Enabled = false;

        escalaInicial = Entity.Transform.Scale;
        masaInicial = cuerpo.Mass;

        Entity.Transform.Scale = Vector3.Zero;

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

                    // Solo suena cuando está sola
                    if (colisión.ColliderA.Entity.Get<ElementoSonido>() == null && colisión.ColliderB.Entity.Get<ElementoSonido>() == null)
                        SistemaSonido.SonarBola(ObtenerMayorFuerzaLinearNormalizada());
                }

                // Evita colisiones innesesarias
                if (colisiones >= maxColisiones)
                    await Destruir();
            }
            await Script.NextFrame();
        }
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
        await Task.Delay(duraciónTurnoLocal);
        await Destruir();
    }

    private async Task Destruir()
    {
        if (destruyendo) return;
        destruyendo = true;

        float duraciónLerp = 0.7f;
        float tiempoLerp = 0;
        float tiempo = 0;

        var inicial = Entity.Transform.Scale;
        while (tiempoLerp < duraciónLerp)
        {
            tiempo = SistemaAnimación.EvaluarRápido(tiempoLerp / duraciónLerp);

            Entity.Transform.Scale = Vector3.Lerp(inicial, Vector3.Zero, tiempo);
            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Task.Delay(1);
        }

        // Fin
        Entity.Transform.Scale = Vector3.One;
        activo = false;

        // Guardando entidad
        cuerpo.IsKinematic = true;
        cuerpo.Enabled = false;
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
                return ObtenerMayorValor(velocidad) * 0.6f;
        }
    }
}
