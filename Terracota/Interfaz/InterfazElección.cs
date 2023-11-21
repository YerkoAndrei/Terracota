using System.Linq;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
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

    public Texture spriteAnfitrión;
    public Texture spriteHuesped;

    public Color colorAnfitrión;
    public Color colorHuesped;
    public Color colorRuletaVacía;
    public Color colorRuletaActiva;

    private ImageElement[] ruleta;

    private Grid gridRuleta;
    private Grid decoRuleta;
    private TextBlock txtAnfitrión;
    private TextBlock txtHuesped;

    private ScrollViewer visorAnfitrión;
    private ScrollViewer visorHuesped;

    private Grid gridGanador;
    private ImageElement fondoGanador;
    private ImageElement imgGanador;
    private ImageElement imgFlechaIzquierda;
    private ImageElement imgFlechaDerecha;

    private Grid btnComenzar;
    private Grid btnVolver;
    private Grid btnAleatorio;

    private bool anfitriónSeleccionado;
    private bool huespedSeleccionado;

    private bool esperandoRuleta;
    private bool ganaAnfitrión;

    public override void Start()
    {
        var página = Entity.Get<UIComponent>().Page.RootElement;

        // Ruleta
        gridRuleta = página.FindVisualChildOfType<Grid>("Ruleta");
        decoRuleta = página.FindVisualChildOfType<Grid>("DecoRuleta");
        gridRuleta.Visibility = Visibility.Hidden;
        decoRuleta.Visibility = Visibility.Hidden;
        ruleta = gridRuleta.FindVisualChildrenOfType<ImageElement>().ToArray();

        // Elecciones
        txtAnfitrión = página.FindVisualChildOfType<TextBlock>("txtAnfitrión");
        txtHuesped = página.FindVisualChildOfType<TextBlock>("txtHuesped");
        txtAnfitrión.Text = string.Empty;
        txtHuesped.Text = string.Empty;

        // Visor
        visorAnfitrión = página.FindVisualChildOfType<ScrollViewer>("VisorAnfitrión");
        visorHuesped = página.FindVisualChildOfType<ScrollViewer>("VisorHuesped");

        // Ganador
        gridGanador = página.FindVisualChildOfType<Grid>("Ganador");
        gridGanador.Visibility = Visibility.Hidden;

        fondoGanador = página.FindVisualChildOfType<ImageElement>("FondoGanador");
        imgGanador = página.FindVisualChildOfType<ImageElement>("imgGanador");
        imgFlechaIzquierda = página.FindVisualChildOfType<ImageElement>("imgFlechaIzquierda");
        imgFlechaDerecha = página.FindVisualChildOfType<ImageElement>("imgFlechaDerecha");
        imgFlechaIzquierda.Visibility = Visibility.Hidden;
        imgFlechaDerecha.Visibility = Visibility.Hidden;

        // Botones
        btnComenzar = página.FindVisualChildOfType<Grid>("btnComenzar");
        btnVolver = página.FindVisualChildOfType<Grid>("btnVolver");
        btnAleatorio = página.FindVisualChildOfType<Grid>("btnAleatorio");

        ConfigurarBotón(btnComenzar, EnClicComenzar);
        ConfigurarBotón(btnVolver, EnClicVolver);
        ConfigurarBotón(btnAleatorio, EnClicAleatorio);

        BloquearBotón(btnComenzar, true);

        // Fortalezas
        var padreRanurasAnfitrión = página.FindVisualChildOfType<UniformGrid>("RanurasAnfitrión");
        var padreRanurasHuesped = página.FindVisualChildOfType<UniformGrid>("RanurasHuesped");
        var fortalezas = SistemaMemoria.CargarFortalezas(true);

        padreRanurasAnfitrión.Children.Clear();
        padreRanurasHuesped.Children.Clear();

        padreRanurasAnfitrión.Rows = fortalezas.Count;
        padreRanurasHuesped.Rows = fortalezas.Count;

        padreRanurasAnfitrión.Height = 0;
        padreRanurasHuesped.Height = 0;

        for (int i = 0; i < fortalezas.Count; i++)
        {
            var fortalezaTemp = fortalezas[i];

            // Izquierda
            var nuevaRanuraAnfitrión = prefabRanura.InstantiateElement<Grid>("RanuraIzquierda");
             ConfigurarRanuraElección(nuevaRanuraAnfitrión, i, fortalezaTemp.Nombre, () => EnClicAnfitrión(fortalezaTemp.Nombre));
            padreRanurasAnfitrión.Height += (nuevaRanuraAnfitrión.Height + 10);
            padreRanurasAnfitrión.Children.Add(nuevaRanuraAnfitrión);

            // Derecha
            var nuevaRanuraHuesped = prefabRanura.InstantiateElement<Grid>("RanuraDerecha");
            ConfigurarRanuraElección(nuevaRanuraHuesped, i, fortalezaTemp.Nombre, () => EnClicHuesped(fortalezaTemp.Nombre));
            padreRanurasHuesped.Children.Add(nuevaRanuraHuesped);
            padreRanurasHuesped.Height += (nuevaRanuraHuesped.Height + 10);
        }
    }

    private void EnClicVolver()
    {
        esperandoRuleta = false;
        gridRuleta.Visibility = Visibility.Hidden;
        decoRuleta.Visibility = Visibility.Hidden;
        SistemaEscenas.CambiarEscena(Escenas.menú);
    }

    private async void EnClicAleatorio()
    {
        gridRuleta.Visibility = Visibility.Hidden;
        btnComenzar.Visibility = Visibility.Hidden;
        visorAnfitrión.Visibility = Visibility.Hidden;
        visorHuesped.Visibility = Visibility.Hidden;

        // Anfitrión
        var ranuraAnfitrión = ObtenerRanuraAleatoria();
        EnClicAnfitrión(ranuraAnfitrión);

        // Huesped
        var ranuraHuesped = ObtenerRanuraAleatoria(); 
        EnClicHuesped(ranuraHuesped);

        // Ganador
        ganaAnfitrión = RangoAleatorio(0,2) == 1;

        controladorPartida.RotarXCámara(1.5f);
        await FinalizarRuleta();
        controladorPartida.ComenzarPartida(ganaAnfitrión);
    }

    private string ObtenerRanuraAleatoria()
    {
        // Aleatorio no devuelve fortaleza vacía
        var fortalezas = SistemaMemoria.CargarFortalezas(false);
        if (fortalezas.Count <= 0)
            return string.Empty;

        var aleatorio = RangoAleatorio(0, fortalezas.Count);
        return fortalezas[aleatorio].Nombre;
    }

    private void EnClicAnfitrión(string nombre)
    {
        if (esperandoRuleta)
            return;

        if (huespedSeleccionado)
            BloquearBotón(btnComenzar, false);

        anfitriónSeleccionado = true;
        txtAnfitrión.Text = nombre;
        controladorPartida.CargarFortaleza(nombre, true);
    }

    private void EnClicHuesped(string nombre)
    {
        if (esperandoRuleta)
            return;

        if(anfitriónSeleccionado)
            BloquearBotón(btnComenzar, false);

        huespedSeleccionado = true;
        txtHuesped.Text = nombre;
        controladorPartida.CargarFortaleza(nombre, false);
    }

    private async void EnClicComenzar()
    {
        if (esperandoRuleta || !huespedSeleccionado || !anfitriónSeleccionado)
            return;

        ApagarRuleta();
        esperandoRuleta = true;
        gridRuleta.Visibility = Visibility.Visible;
        decoRuleta.Visibility = Visibility.Visible;

        btnVolver.HorizontalAlignment = HorizontalAlignment.Center;
        btnVolver.Margin = new Thickness(0,0,0,30);

        btnComenzar.Visibility = Visibility.Hidden;
        btnAleatorio.Visibility = Visibility.Hidden;
        visorAnfitrión.Visibility = Visibility.Hidden;
        visorHuesped.Visibility = Visibility.Hidden;

        var aleatorio = RangoAleatorio(40, 51);
        controladorPartida.RotarXCámara(4.5f);
        await MoverRuleta(aleatorio);
        await FinalizarRuleta();
        controladorPartida.ComenzarPartida(ganaAnfitrión);
    }

    private async Task MoverRuleta(int toques)
    {
        int diezAntes = toques - 10;
        int toqueActual = 0;
        int ruletaActual = 0;
        int delay = 45;

        while (toqueActual < toques && esperandoRuleta)
        {
            toqueActual++;
            ruletaActual++;

            if (ruletaActual >= ruleta.Length)
                ruletaActual = 0;

            // Cambia colores
            ApagarRuleta();
            ruleta[ruletaActual].Color = colorRuletaActiva;

            // Últimos van más lento
            if (toqueActual >= diezAntes)
                delay += 40;

            await Task.Delay(delay);
        }

        // Ganador
        ganaAnfitrión = (ruletaActual > ((ruleta.Length / 2) - 1));

        // Fin
        await Task.Delay(500);
    }

    private async Task FinalizarRuleta()
    {
        btnVolver.Visibility = Visibility.Hidden;
        btnAleatorio.Visibility = Visibility.Hidden;
        gridRuleta.Visibility = Visibility.Hidden;
        decoRuleta.Visibility = Visibility.Hidden;

        if (ganaAnfitrión)
        {
            // Gana anfitrión / izquierda
            fondoGanador.Color = colorAnfitrión;
            imgGanador.Source = ObtenerSprite(spriteAnfitrión);
            imgFlechaIzquierda.Visibility = Visibility.Visible;
        }
        else
        {
            // Gana huesped / derecha
            fondoGanador.Color = colorHuesped;
            imgGanador.Source = ObtenerSprite(spriteHuesped);
            imgFlechaDerecha.Visibility = Visibility.Visible;

        }
        gridGanador.Visibility = Visibility.Visible;

        // Fin
        await Task.Delay(2000);
    }

    private void ApagarRuleta()
    {
        for(int i=0; i < ruleta.Length; i++)
        {
            ruleta[i].Color = colorRuletaVacía;
        }
    }
}
