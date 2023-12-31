﻿using System.Threading.Tasks;
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

    public override async Task Execute()
    {
        var elemento = Entity.Get<ElementoBloque>();

        cuerpo = elemento.cuerpo;
        instanciaSonido = SistemaSonido.CrearInstancia(elemento.tipoBloque);

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
                SonarBloqueFísico(elemento.tipoBloque, ObtenerMayorFuerzaLinearNormalizada());
            }
            else if (ObtenerMayorFuerzaLinear() < sensiblidadLinear && ObtenerMayorFuerzaAngular() >= sensiblidadAngular && controladorBola == null)
            {
                // Volumen base velocidad angular
                SonarBloqueFísico(elemento.tipoBloque, ObtenerMayorFuerzaAngularNormalizada());
            }
            else if (controladorBola != null)
            {
                // Volumen base velocidad bola
                SonarBloqueFísico(elemento.tipoBloque, controladorBola.ObtenerMayorFuerzaLinearNormalizada());
            }
            await Script.NextFrame();
        }
    }

    public void SonarBloqueFísico(TipoBloque bloque, float fuerza)
    {
        if (instanciaSonido.PlayState == Stride.Media.PlayState.Playing)
            return;

        // Rango aleatorio da un poco de vida a los sonidos
        instanciaSonido.Volume = SistemaSonido.ObtenerVolumen(Configuraciones.volumenEfectos) * (fuerza - RangoAleatorio(0.0f, 0.8f));
        instanciaSonido.PlayExclusive();
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
