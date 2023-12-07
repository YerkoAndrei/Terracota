using System.Threading.Tasks;
using Stride.Engine;

namespace Terracota;

public class ElementoSonido : AsyncScript
{
    public override async Task Execute()
    {
        var elemento = Entity.Get<ElementoBloque>();
        var cuerpo = elemento.cuerpo;
        var primero = false;

        while (Game.IsRunning)
        {
            var colisión = await cuerpo.NewCollision();

            // Primero de posicionamiento no suena
            if (primero)
            {
                // Obtiene fuerza bola
                var controladorBola = colisión.ColliderA.Entity.Get<ControladorBola>();
                if (controladorBola == null)
                    controladorBola = colisión.ColliderB.Entity.Get<ControladorBola>();
                
                if(controladorBola != null)
                    SistemaSonido.SonarBloqueFísico(elemento.tipoBloque, controladorBola.ObtenerFuerza());
                else
                    SistemaSonido.SonarBloqueFísico(elemento.tipoBloque, 0.5f);
            }
            else
                primero = true;

            await Script.NextFrame();
        }
    }
}
