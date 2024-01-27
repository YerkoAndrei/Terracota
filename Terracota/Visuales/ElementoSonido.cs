using System.Linq;
using System.Threading.Tasks;
using Stride.Audio;
using Stride.Engine;
using Stride.Physics;

namespace Terracota;
using static Sistema;
using static Constantes;

public class ElementoSonido : AsyncScript
{
    private RigidbodyComponent cuerpo;
    private SoundInstance instanciaSonido;
    private IPartida iPartida;

    private float fuerzaSonido;

    public override async Task Execute()
    {
        var controlador = Entity.Scene.Entities.FirstOrDefault(e => e.Name == "ControladorPartida");
        foreach (var componente in controlador.Components)
        {
            if (componente is IPartida)
            {
                iPartida = (IPartida)componente;
                break;
            }
        }

        var elemento = Entity.Get<ElementoBloque>();

        cuerpo = elemento.cuerpo;
        instanciaSonido = SistemaSonido.CrearInstancia(elemento.tipoBloque);

        // Sensibilidades para que suene
        var sensiblidadLinear = 0.025;
        var sensiblidadAngular = 0.02f;

        while (Game.IsRunning)
        {
            var colisión = await cuerpo.NewCollision();
            fuerzaSonido = 0;

            // Obtiene controlador bola
            var controladorBola = colisión.ColliderA.Entity.Get<ControladorBola>();
            if (controladorBola == null)
                controladorBola = colisión.ColliderB.Entity.Get<ControladorBola>();

            // Solo suena con cierta fuerza
            if (ObtenerMayorFuerzaLinear() >= sensiblidadLinear && controladorBola == null)
            {
                // Volumen base velocidad linear
                fuerzaSonido = ObtenerMayorFuerzaLinearNormalizada();
            }
            else if (ObtenerMayorFuerzaLinear() < sensiblidadLinear && ObtenerMayorFuerzaAngular() >= sensiblidadAngular && controladorBola == null)
            {
                // Volumen base velocidad angular
                fuerzaSonido = ObtenerMayorFuerzaAngularNormalizada();
            }
            else if (controladorBola != null)
            {
                // Volumen base velocidad bola
                var multiplicador = 1f;
                if (controladorBola.tipoProyectil == TipoProyectil.metralla)
                    multiplicador = 0.4f;

                fuerzaSonido = controladorBola.ObtenerMayorFuerzaLinearNormalizada() * multiplicador;
            }

            if(fuerzaSonido > 0)
                SonarBloqueFísico(fuerzaSonido);

            await Script.NextFrame();
        }
    }

    public void SonarBloqueFísico(float fuerza)
    {
        if (instanciaSonido.PlayState == Stride.Media.PlayState.Playing || !iPartida.ObtenerActivo())
            return;

        // Volumen y pitch aleatorio da más vida a los sonidos
        instanciaSonido.Volume = (SistemaSonido.ObtenerVolumen(Configuraciones.volumenEfectos) * fuerza) - RangoAleatorio(0, 0.6f);
        instanciaSonido.Pitch = RangoAleatorio(0.8f, 1.2f);
        instanciaSonido.Play();
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

    public float ObtenerFuerzaSonido()
    {
        var sonido = fuerzaSonido;
        fuerzaSonido = 0;
        return sonido;
    }
}
