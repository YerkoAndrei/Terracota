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
            await cuerpo.NewCollision();

            // Primero de posicionamiento no suena
            if (primero)
                SistemaSonido.SonarBloqueFísico(elemento.tipoBloque);
            else
                primero = true;

            await Script.NextFrame();
        }
    }
}
