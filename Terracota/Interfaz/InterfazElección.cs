using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Terracota;
using static Sistema;
using static Constantes;

public class InterfazElección : StartupScript
{
    public ControladorPartidaLocal controladorPartida;

    private List<ImageElement> ruleta;

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

        txtIzquierda = página.FindVisualChildOfType<TextBlock>("SelecciónIzquierda");
        txtDerecha = página.FindVisualChildOfType<TextBlock>("SelecciónDerecha");

        fondoIzquierda = página.FindVisualChildOfType<Grid>("FondoIzquierda");
        fondoDerecha = página.FindVisualChildOfType<Grid>("FondoDerecha");

        fondoIzquierda.Opacity = 0;
        fondoDerecha.Opacity = 0;

        página.FindVisualChildOfType<Button>("btnComenzar").Click += (sender, e) => { EnClicComenzar(); };

        ruleta = new List<ImageElement>
        {
            página.FindVisualChildOfType<ImageElement>("tope_0"),
            página.FindVisualChildOfType<ImageElement>("tope_1"),
            página.FindVisualChildOfType<ImageElement>("tope_2"),
            página.FindVisualChildOfType<ImageElement>("tope_3"),
            página.FindVisualChildOfType<ImageElement>("tope_4"),
            página.FindVisualChildOfType<ImageElement>("tope_5"),
            página.FindVisualChildOfType<ImageElement>("tope_6"),
            página.FindVisualChildOfType<ImageElement>("tope_7"),
        };

        // Botones
        página.FindVisualChildOfType<Button>("btnRanuraIzquierda_1").Click += (sender, e) => { EnClicIzquierda(1); };
        página.FindVisualChildOfType<Button>("btnRanuraIzquierda_2").Click += (sender, e) => { EnClicIzquierda(2); };
        página.FindVisualChildOfType<Button>("btnRanuraIzquierda_3").Click += (sender, e) => { EnClicIzquierda(3); };
        página.FindVisualChildOfType<Button>("btnRanuraIzquierda_4").Click += (sender, e) => { EnClicIzquierda(4); };
        página.FindVisualChildOfType<Button>("btnRanuraIzquierda_5").Click += (sender, e) => { EnClicIzquierda(5); };
        página.FindVisualChildOfType<Button>("btnRanuraIzquierda_6").Click += (sender, e) => { EnClicIzquierda(6); };
        página.FindVisualChildOfType<Button>("btnRanuraIzquierda_7").Click += (sender, e) => { EnClicIzquierda(7); };
        página.FindVisualChildOfType<Button>("btnRanuraIzquierda_8").Click += (sender, e) => { EnClicIzquierda(8); };

        página.FindVisualChildOfType<Button>("btnRanuraDerecha_1").Click += (sender, e) => { EnClicDerecha(1); };
        página.FindVisualChildOfType<Button>("btnRanuraDerecha_2").Click += (sender, e) => { EnClicDerecha(2); };
        página.FindVisualChildOfType<Button>("btnRanuraDerecha_3").Click += (sender, e) => { EnClicDerecha(3); };
        página.FindVisualChildOfType<Button>("btnRanuraDerecha_4").Click += (sender, e) => { EnClicDerecha(4); };
        página.FindVisualChildOfType<Button>("btnRanuraDerecha_5").Click += (sender, e) => { EnClicDerecha(5); };
        página.FindVisualChildOfType<Button>("btnRanuraDerecha_6").Click += (sender, e) => { EnClicDerecha(6); };
        página.FindVisualChildOfType<Button>("btnRanuraDerecha_7").Click += (sender, e) => { EnClicDerecha(7); };
        página.FindVisualChildOfType<Button>("btnRanuraDerecha_8").Click += (sender, e) => { EnClicDerecha(8); };
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
        if (!derechaSeleccionada || !izquierdaSeleccionada)
            return;
        
        int aleatorio = (int)RangoAleatorio(40, 50);

        esperandoRuleta = true;
        ApagarRuleta();
        gridRuleta.Visibility = Visibility.Visible;

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

            if (ruletaActual >= ruleta.Count)
                ruletaActual = 0;

            // Cambia colores
            ApagarRuleta();
            ruleta[ruletaActual].Color = Color.Green;

            // Últimos van más lento
            if (toqueActual >= 30)
                delay += 30;

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
        for(int i=0; i < ruleta.Count; i++)
        {
            ruleta[i].Color = Color.White;
        }
    }
}
