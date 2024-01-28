using System.Threading.Tasks;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;

namespace Terracota;

public class ControladorInfo : AsyncScript
{
    private TextBlock txtFPS;
    private TextBlock txtPing;

    public override async Task Execute()
    {
        var página = Entity.Get<UIComponent>().Page.RootElement;
        txtFPS = página.FindVisualChildOfType<TextBlock>("txtFPS");
        txtPing = página.FindVisualChildOfType<TextBlock>("txtPing");

        txtFPS.Text = string.Empty;
        txtPing.Text = string.Empty;

        var contador = 0;
        var cuadros = 0f;

        // Promedio de FPS cada 60 frames
        while (Game.IsRunning)
        {
            // FPS
            if (contador >= 60)
            {
                var fps = contador / cuadros;
                txtFPS.Text = string.Format("FPS: {0}", fps.ToString("00"));
                cuadros = 0;
                contador = 0;
            }
            else
            {
                cuadros += (float)Game.UpdateTime.Elapsed.TotalSeconds;
                contador++;
            }

            // PING
            if (SistemaRed.ObtenerJugando())
                txtPing.Text = string.Format("Ping: {0}", SistemaRed.ObtenerPing());
            else
                txtPing.Text = string.Empty;

            await Script.NextFrame();
        }
    }
}
