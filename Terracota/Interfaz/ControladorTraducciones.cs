using System.Collections.Generic;
using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI;

namespace Terracota;

public class ControladorTraducciones : StartupScript
{
    // TextBlock - Código
    public Dictionary<string, string> códigos = new Dictionary<string, string> { };

    private Dictionary<TextBlock, string> textos;

    public override void Start()
    {
        if (textos == null)
            CargarTextos();
    }

    private void CargarTextos()
    {
        var página = Entity.Get<UIComponent>().Page.RootElement;
        textos = new Dictionary<TextBlock, string>();

        foreach (var código in códigos)
        {
            textos.Add(página.FindVisualChildOfType<TextBlock>(código.Key), código.Value);
        }
    }

    public void Traducir()
    {
        // Traducción temprana por SistemaEscenas
        if (textos == null)
            CargarTextos();

        foreach (var texto in textos)
        {
            texto.Key.Text = SistemaTraducción.ObtenerTraducción(texto.Value);
        }
    }
}
