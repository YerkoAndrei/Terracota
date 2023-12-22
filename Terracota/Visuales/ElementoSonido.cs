using System.Threading.Tasks;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Sistema;

public class ElementoSonido : AsyncScript
{
    private RigidbodyComponent cuerpo;

    public override async Task Execute()
    {
        var elemento = Entity.Get<ElementoBloque>();
        cuerpo = elemento.cuerpo;

        // Sensibilidades para que suene
        var sensiblidadLinear = 0.025;
        var sensiblidadAngular = 0.02f;

        while (Game.IsRunning)
        {
            var colisión = await cuerpo.NewCollision();

            // Obtiene controlador bola
            var controladorBola = colisión.ColliderA.Entity.Get<ControladorBola>();
            if (controladorBola == null)
                controladorBola = colisión.ColliderB.Entity.Get<ControladorBola>();

            // Solo suena con cierta fuerza
            if (ObtenerMayorFuerzaLinear() >= sensiblidadLinear && controladorBola == null)
            {
                // Volumen base velocidad linear
                SistemaSonido.SonarBloqueFísico(elemento.tipoBloque, ObtenerMayorFuerzaLinearNormalizada());
            }
            else if (ObtenerMayorFuerzaLinear() < sensiblidadLinear && ObtenerMayorFuerzaAngular() >= sensiblidadAngular && controladorBola == null)
            {
                // Volumen base velocidad angular
                SistemaSonido.SonarBloqueFísico(elemento.tipoBloque, ObtenerMayorFuerzaAngularNormalizada());
            }
            else if (controladorBola != null)
            {
                // Volumen base velocidad bola
                SistemaSonido.SonarBloqueFísico(elemento.tipoBloque, controladorBola.ObtenerMayorFuerzaLinearNormalizada());
            }
            await Script.NextFrame();
        }
    }

    // Encuentra mayor velocidad sin normalizar
    public float ObtenerMayorFuerzaLinear()
    {
        var velocidad = cuerpo.LinearVelocity;
        return ObtenerMayorValor(velocidad);
    }

    public float ObtenerMayorFuerzaAngular()
    {
        var velocidad = cuerpo.AngularVelocity;
        return ObtenerMayorValor(velocidad);
    }

    // Encuentra mayor velocidad normalizada
    public float ObtenerMayorFuerzaLinearNormalizada()
    {
        var velocidad = cuerpo.LinearVelocity;
        velocidad.Normalize();
        return ObtenerMayorValor(velocidad);
    }

    public float ObtenerMayorFuerzaAngularNormalizada()
    {
        var velocidad = cuerpo.AngularVelocity;
        velocidad.Normalize();
        return ObtenerMayorValor(velocidad);
    }
}
