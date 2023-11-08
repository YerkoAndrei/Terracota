using System.Linq;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;

namespace Terracota;
using static Sistema;
using static Constantes;

public class InterfazElección : StartupScript
{
    public ControladorPartidaLocal controladorPartida;
    public UILibrary prefabRanura;

    private ImageElement[] ruleta;

    private Grid gridRuleta;
    private TextBlock txtIzquierda;
    private TextBlock txtDerecha;

    private Grid fondoIzquierda;
    private Grid fondoDerecha;

    private bool izquierdaSeleccionada;
    private bool derechaSeleccionada;

    private bool esperandoRuleta;
    private bool ganaIzquierda;

    public override void Start()
    {
        var página = Entity.Get<UIComponent>().Page.RootElement;

        gridRuleta = página.FindVisualChildOfType<Grid>("Ruleta");
        gridRuleta.Visibility = Visibility.Hidden;
        ruleta = gridRuleta.FindVisualChildrenOfType<ImageElement>().ToArray();

        txtIzquierda = página.FindVisualChildOfType<TextBlock>("SelecciónIzquierda");
        txtDerecha = página.FindVisualChildOfType<TextBlock>("SelecciónDerecha");

        fondoIzquierda = página.FindVisualChildOfType<Grid>("FondoIzquierda");
        fondoDerecha = página.FindVisualChildOfType<Grid>("FondoDerecha");

        fondoIzquierda.Opacity = 0;
        fondoDerecha.Opacity = 0;

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnComenzar"), EnClicComenzar);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnVolver"), EnClicVolver);

        // Fortalezas
        var padreRanurasIzquierda = página.FindVisualChildOfType<UniformGrid>("RanurasIzquierda");
        var padreRanurasDerecha = página.FindVisualChildOfType<UniformGrid>("RanurasDerecha");
        var fortalezas = SistemaMemoria.CargarFortalezas(true);

        padreRanurasIzquierda.Children.Clear();
        padreRanurasDerecha.Children.Clear();

        padreRanurasIzquierda.Rows = fortalezas.Count;
        padreRanurasDerecha.Rows = fortalezas.Count;

        padreRanurasIzquierda.Height = 0;
        padreRanurasDerecha.Height = 0;

        for (int i = 0; i < fortalezas.Count; i++)
        {
            var fortalezaTemp = fortalezas[i];

            // Izquierda
            var nuevaRanuraIzquierda = prefabRanura.InstantiateElement<Grid>("RanuraIzquierda");
             ConfigurarRanuraElección(nuevaRanuraIzquierda, i, fortalezaTemp.ranura, fortalezaTemp.miniatura, () => EnClicIzquierda(fortalezaTemp.ranura));
            padreRanurasIzquierda.Height += (nuevaRanuraIzquierda.Height + 10);
            padreRanurasIzquierda.Children.Add(nuevaRanuraIzquierda);

            // Derecha
            var nuevaRanuraDerecha = prefabRanura.InstantiateElement<Grid>("RanuraDerecha");
            ConfigurarRanuraElección(nuevaRanuraDerecha, i, fortalezaTemp.ranura, fortalezaTemp.miniatura, () => EnClicDerecha(fortalezaTemp.ranura));
            padreRanurasDerecha.Children.Add(nuevaRanuraDerecha);
            padreRanurasDerecha.Height += (nuevaRanuraDerecha.Height + 10);
        }
    }

    private void EnClicVolver()
    {
        if (esperandoRuleta)
            return;

        SistemaEscenas.CambiarEscena(Escenas.menú);
    }

    private void EnClicIzquierda(int ranura)
    {
        if (esperandoRuleta)
            return;

        izquierdaSeleccionada = true;
        txtIzquierda.Text = ranura.ToString();
        controladorPartida.CargarFortaleza(ranura, true);
    }

    private void EnClicDerecha(int ranura)
    {
        if (esperandoRuleta)
            return;

        derechaSeleccionada = true;
        txtDerecha.Text = ranura.ToString();
        controladorPartida.CargarFortaleza(ranura, false);
    }

    private async void EnClicComenzar()
    {
        if (esperandoRuleta || !derechaSeleccionada || !izquierdaSeleccionada)
            return;

        ApagarRuleta();
        esperandoRuleta = true;
        gridRuleta.Visibility = Visibility.Visible;

        var aleatorio = (int)RangoAleatorio(40, 48);
        await MoverRuleta(aleatorio);
        controladorPartida.ComenzarPartida(ganaIzquierda);
    }

    private async Task MoverRuleta(int toques)
    {
        int toqueActual = 0;
        int ruletaActual = 0;
        int delay = 60;

        while (toqueActual < toques)
        {
            toqueActual++;
            ruletaActual++;

            if (ruletaActual >= ruleta.Length)
                ruletaActual = 0;

            // Cambia colores
            ApagarRuleta();
            ruleta[ruletaActual].Color = Color.Green;

            // Últimos van más lento
            if (toqueActual >= 30)
                delay += 40;

            await Task.Delay(delay);
        }

        // Fin
        await Task.Delay(500);
        gridRuleta.Visibility = Visibility.Hidden;
        ganaIzquierda = (ruletaActual > 3);

        if (ganaIzquierda)
        {
            // Gana izquierda
            txtIzquierda.TextSize = 100;
            txtDerecha.TextSize = 30;

            txtIzquierda.TextColor = Color.White;
            txtDerecha.TextColor = Color.Red;

            fondoIzquierda.BackgroundColor = Color.Green;
            fondoDerecha.BackgroundColor = Color.Red;
        }
        else
        {
            // Gana derecha
            txtIzquierda.TextSize = 30;
            txtDerecha.TextSize = 100;

            txtIzquierda.TextColor = Color.Red;
            txtDerecha.TextColor = Color.White;

            fondoIzquierda.BackgroundColor = Color.Red;
            fondoDerecha.BackgroundColor = Color.Green;
        }

        fondoIzquierda.Opacity = 0.5f;
        fondoDerecha.Opacity = 0.5f;

        // Fin
        await Task.Delay(3000);
    }

    private void ApagarRuleta()
    {
        for(int i=0; i < ruleta.Length; i++)
        {
            ruleta[i].Color = Color.White;
        }
    }
}
