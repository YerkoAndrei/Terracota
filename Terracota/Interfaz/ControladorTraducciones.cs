using System.Collections.Generic;
using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI;

namespace Terracota;

public class ControladorTraducciones : StartupScript
{
    // Código - TextBlock
    public Dictionary<string, string> códigos = new Dictionary<string, string> { };

    private Dictionary<string, TextBlock> textos;

    public override void Start()
    {
        if (textos == null)
            CargarTextos();
    }

    private void CargarTextos()
    {
        var página = Entity.Get<UIComponent>().Page.RootElement;
        textos = new Dictionary<string, TextBlock>();

        foreach (var código in códigos)
        {
            textos.Add(código.Key, página.FindVisualChildOfType<TextBlock>(código.Value));
        }
    }

    public void Traducir()
    {
        // Traducción temprana por SistemaEscenas
        if (textos == null)
            CargarTextos();

        foreach (var texto in textos)
        {
            texto.Value.Text = SistemaTraducción.ObtenerTraducción(texto.Key);
        }
    }
}
