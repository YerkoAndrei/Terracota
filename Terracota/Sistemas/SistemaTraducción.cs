using Stride.Engine;
using Stride.Graphics;

namespace Terracota;

public class SistemaTraducción : StartupScript
{
    public SpriteFont fuentePrincipal;
    public SpriteFont fuenteRespaldo;

    private static SistemaTraducción instancia;

    public override void Start()
    {
        instancia = this;
    }

    public static SpriteFont VerificarFuente(string texto)
    {
        foreach(char c in texto)
        {
            if(!instancia.fuentePrincipal.IsCharPresent(c))
                return instancia.fuenteRespaldo;
        }
        return instancia.fuentePrincipal;
    }
}
