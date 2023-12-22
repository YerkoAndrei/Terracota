using System;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Sistema;
using static Constantes;

public class ControladorBola : AsyncScript
{
    public int maxColisiones;
    public Prefab prefabPartículas;

    private TipoProyectil tipoProyectil;
    private RigidbodyComponent cuerpo;
    private Scene escena;
    private bool destruyendo;
    private bool removerDeEscena;
    private int colisiones;

    public void Inicialización(TipoProyectil _tipoProyectil)
    {
        tipoProyectil = _tipoProyectil;
        removerDeEscena = (tipoProyectil != TipoProyectil.metralla);
    }

    public override async Task Execute()
    {
        escena = Entity.Scene;
        cuerpo = Entity.Get<RigidbodyComponent>();

        // Tiempo de vida
        if (removerDeEscena)
            _ = ContarVida();

        while (Game.IsRunning)
        {
            var colisión = await cuerpo.NewCollision();

            // Metralla con metralla no cuenta como colisión
            if ((colisión.ColliderA == cuerpo && colisión.ColliderB.Entity.Get<ControladorBola>() == null) ||
               (colisión.ColliderB == cuerpo && colisión.ColliderA.Entity.Get<ControladorBola>() == null))
            {
                colisiones++;
                MostrarEfectos();
                SistemaSonido.SonarBola(ObtenerMayorFuerzaLinearNormalizada());
            }
            
            // Evita colisiones innesesarias
            if (colisiones >= maxColisiones)
                await Destruir();

            await Script.NextFrame();
        }
    }

    private void MostrarEfectos()
    {
        var partícula = prefabPartículas.Instantiate()[0];
        switch(tipoProyectil)
        {
            case TipoProyectil.bola:
                partícula.Transform.Scale *= 1.1f;
                break;
            case TipoProyectil.metralla:
                partícula.Transform.Scale *= 0.4f;
                break;
        }

        partícula.Transform.Position = Entity.Transform.Position;
        escena.Entities.Add(partícula);

        // Posterior borrado y descarga
        _ = ContarVidaPartícula(partícula);
    }

    private async Task ContarVidaPartícula(Entity entidad)
    {
        // Duración partícula
        await Task.Delay(1000);
        escena.Entities.Remove(entidad);
    }

    private async Task ContarVida()
    {
        await Task.Delay(duraciónTurno);
        await Destruir();
    }

    private async Task Destruir()
    {
        if (destruyendo) return;
        destruyendo = true;

        float duraciónLerp = 1;
        float tiempoLerp = 0;
        float tiempo = 0;

        var inicial = Entity.Transform.Scale;
        while (tiempoLerp < duraciónLerp)
        {
            tiempo = SistemaAnimación.EvaluarRápido(tiempoLerp / duraciónLerp);

            Entity.Transform.Scale = Vector3.Lerp(inicial, Vector3.Zero, tiempo);
            tiempoLerp += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            await Script.NextFrame();
        }

        // Fin
        cuerpo.Enabled = false;

        // Removiendo entidad
        if (removerDeEscena)
            escena.Entities.Remove(Entity);
    }

    public float ObtenerMayorFuerzaLinearNormalizada()
    {
        // Encuentra mayor velocidad normalizada
        var velocidad = cuerpo.LinearVelocity;
        velocidad.Normalize();

        return ObtenerMayorValor(velocidad);
    }

    // Metralla es destruida en orden por ControladorCañón
    public void DestruirInmediato()
    {
        escena.Entities.Remove(Entity);
    }
}
