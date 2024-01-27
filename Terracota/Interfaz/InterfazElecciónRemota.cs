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

public class InterfazElecciónRemota : StartupScript
{
    public ControladorPartidaRemota controladorPartida;
    public UILibrary prefabRanura;

    public Texture spriteAnfitrión;
    public Texture spriteHuésped;

    public Color colorAnfitrión;
    public Color colorHuésped;
    public Color colorRuletaVacía;
    public Color colorRuletaActiva;

    private ImageElement[] ruleta;

    private Grid gridRuleta;
    private Grid decoRuleta;
    private TextBlock txtAnfitrión;
    private TextBlock txtHuésped;

    private ScrollViewer visorAnfitrión;
    private ScrollViewer visorHuésped;

    private Grid gridGanador;
    private ImageElement fondoGanador;
    private ImageElement imgGanador;
    private ImageElement imgFlechaIzquierda;
    private ImageElement imgFlechaDerecha;

    private ImageElement imgAnfitriónListo;
    private ImageElement imgHuéspedListo;

    private Grid btnComenzar;
    private Grid btnVolver;
    private Grid btnAleatorio;

    private bool esperandoRuleta;
    private bool ganaAnfitrión;
    private bool partidaCancelada;

    // Animación
    private Grid animSuperior;

    public void Inicializar()
    {
        partidaCancelada = false;

        var página = Entity.Get<UIComponent>().Page.RootElement;

        // Ruleta
        gridRuleta = página.FindVisualChildOfType<Grid>("Ruleta");
        decoRuleta = página.FindVisualChildOfType<Grid>("DecoRuleta");
        gridRuleta.Visibility = Visibility.Hidden;
        decoRuleta.Visibility = Visibility.Hidden;
        ruleta = gridRuleta.FindVisualChildrenOfType<ImageElement>().ToArray();

        // Elecciones
        imgAnfitriónListo = página.FindVisualChildOfType<ImageElement>("imgAnfitriónListo");
        imgHuéspedListo = página.FindVisualChildOfType<ImageElement>("imgHuéspedListo");

        txtAnfitrión = página.FindVisualChildOfType<TextBlock>("txtAnfitrión");
        txtHuésped = página.FindVisualChildOfType<TextBlock>("txtHuésped");
        txtAnfitrión.Text = string.Empty;
        txtHuésped.Text = string.Empty;

        // Visor
        visorAnfitrión = página.FindVisualChildOfType<ScrollViewer>("VisorAnfitrión");
        visorHuésped = página.FindVisualChildOfType<ScrollViewer>("VisorHuésped");

        // Ganador
        gridGanador = página.FindVisualChildOfType<Grid>("Ganador");
        gridGanador.Visibility = Visibility.Hidden;

        fondoGanador = página.FindVisualChildOfType<ImageElement>("FondoGanador");
        imgGanador = página.FindVisualChildOfType<ImageElement>("imgGanador");
        imgFlechaIzquierda = página.FindVisualChildOfType<ImageElement>("imgFlechaIzquierda");
        imgFlechaDerecha = página.FindVisualChildOfType<ImageElement>("imgFlechaDerecha");
        imgFlechaIzquierda.Visibility = Visibility.Hidden;
        imgFlechaDerecha.Visibility = Visibility.Hidden;

        // Animación
        animSuperior = página.FindVisualChildOfType<Grid>("Superior");

        // Botones
        btnComenzar = página.FindVisualChildOfType<Grid>("btnComenzar");
        btnVolver = página.FindVisualChildOfType<Grid>("btnVolver");
        btnAleatorio = página.FindVisualChildOfType<Grid>("btnAleatorio");

        ConfigurarBotón(btnComenzar, EnClicComenzar);
        ConfigurarBotón(btnVolver, EnClicVolver);
        ConfigurarBotón(btnAleatorio, EnClicAleatorio);

        // Fortalezas
        var padreRanurasAnfitrión = página.FindVisualChildOfType<UniformGrid>("RanurasAnfitrión");
        var padreRanurasHuésped = página.FindVisualChildOfType<UniformGrid>("RanurasHuésped");
        var fortalezas = SistemaMemoria.CargarFortalezas(true);

        padreRanurasAnfitrión.Children.Clear();
        padreRanurasHuésped.Children.Clear();

        padreRanurasAnfitrión.Rows = fortalezas.Count;
        padreRanurasHuésped.Rows = fortalezas.Count;

        padreRanurasAnfitrión.Height = 0;
        padreRanurasHuésped.Height = 0;

        for (int i = 0; i < fortalezas.Count; i++)
        {
            var fortalezaTemp = fortalezas[i];

            // Solo muestra fortalezas locales
            if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
            {
                // Izquierda
                var nuevaRanuraAnfitrión = prefabRanura.InstantiateElement<Grid>("RanuraIzquierda");
                ConfigurarRanuraElección(nuevaRanuraAnfitrión, i, fortalezaTemp.Nombre, () => EnClicAnfitrión(fortalezaTemp.Nombre));
                padreRanurasAnfitrión.Height += (nuevaRanuraAnfitrión.Height + 10);
                padreRanurasAnfitrión.Children.Add(nuevaRanuraAnfitrión);
            }
            else
            {
                // Derecha
                var nuevaRanuraHuésped = prefabRanura.InstantiateElement<Grid>("RanuraDerecha");
                ConfigurarRanuraElección(nuevaRanuraHuésped, i, fortalezaTemp.Nombre, () => EnClicHuésped(fortalezaTemp.Nombre));
                padreRanurasHuésped.Children.Add(nuevaRanuraHuésped);
                padreRanurasHuésped.Height += (nuevaRanuraHuésped.Height + 10);
            }
        }

        // Bloqueo botones
        BloquearBotón(btnComenzar, true);

        if (fortalezas.Count <= 1)
            BloquearBotón(btnAleatorio, true);
    }

    private void EnClicVolver()
    {
        esperandoRuleta = false;
        partidaCancelada = true;
        gridRuleta.Visibility = Visibility.Hidden;
        decoRuleta.Visibility = Visibility.Hidden;

        // Cierra conexión
        SistemaRed.CerrarConexión(true);
    }

    private void EnClicAleatorio()
    {
        btnComenzar.Visibility = Visibility.Hidden;
        btnAleatorio.Visibility = Visibility.Hidden;

        gridRuleta.Visibility = Visibility.Hidden;
        visorAnfitrión.Visibility = Visibility.Hidden;
        visorHuésped.Visibility = Visibility.Hidden;

        // Carga fortaleza
        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
        {
            var ranuraAnfitrión = ObtenerRanuraAleatoria();
            EnClicAnfitrión(ranuraAnfitrión);
        }
        else
        {
            var ranuraHuésped = ObtenerRanuraAleatoria();
            EnClicHuésped(ranuraHuésped);
        }

        // Intenta comenzar partida
        IntentarComenzarPartida();
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

        BloquearBotón(btnComenzar, false);

        txtAnfitrión.Text = nombre;
        controladorPartida.CargarFortaleza(nombre, true);
    }

    private void EnClicHuésped(string nombre)
    {
        if (esperandoRuleta)
            return;

        BloquearBotón(btnComenzar, false);

        txtHuésped.Text = nombre;
        controladorPartida.CargarFortaleza(nombre, false);
    }

    private void EnClicComenzar()
    {
        if (esperandoRuleta)
            return;

        visorAnfitrión.Visibility = Visibility.Hidden;
        visorHuésped.Visibility = Visibility.Hidden;

        btnComenzar.Visibility = Visibility.Hidden;
        btnAleatorio.Visibility = Visibility.Hidden;

        btnVolver.HorizontalAlignment = HorizontalAlignment.Center;
        btnVolver.Margin = new Thickness(0, 0, 0, 120);

        if (partidaCancelada)
            return;

        // Intenta comenzar partida
        IntentarComenzarPartida();
    }

    private void IntentarComenzarPartida()
    {
        MostrarJugadorListo(SistemaRed.ObtenerTipoJugador());

        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
        {
            _ = SistemaRed.EnviarData(DataRed.jugadorListo, TipoJugador.anfitrión);

            // Anfitrión comprueba que estén listos
            controladorPartida.RevisarJugadoresListos(SistemaRed.ObtenerTipoJugador());
        }
        else
            _ = SistemaRed.EnviarData(DataRed.jugadorListo, TipoJugador.huésped);
    }

    public async void ComenzarRuleta()
    {
        ApagarRuleta();
        esperandoRuleta = true;
        gridRuleta.Visibility = Visibility.Visible;
        decoRuleta.Visibility = Visibility.Visible;

        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
        {
            _ = SistemaRed.EnviarData(DataRed.comenzarRuleta);

            controladorPartida.RotarXCámara(4.5f);

            // Toques aleatorios deciden quién empeiza
            await MoverRuletaAnfitrión(RangoAleatorio(40, 51));

            _ = SistemaRed.EnviarData(DataRed.finalizarRuleta, ganaAnfitrión);
            ComenzarPartida(ganaAnfitrión);
        }
        else
        {
            controladorPartida.RotarXCámara(4.5f);
            await MoverRuletaHuésped();
        }
    }

    public async void ComenzarPartida(bool _ganaAnfitrión)
    {
        ganaAnfitrión = _ganaAnfitrión;
        esperandoRuleta = false;

        SistemaSonido.SonarInicio();
        await FinalizarRuleta();
        SistemaSonido.SonarInicio();
        SistemaSonido.CambiarMúsica(true);

        SistemaAnimación.AnimarElemento(animSuperior, 0.2f, false, Direcciones.arriba, TipoCurva.rápida, () =>
        {
            controladorPartida.ComenzarPartida(ganaAnfitrión);
        });
    }

    private async Task MoverRuletaAnfitrión(int toques)
    {
        int diezAntes = toques - 10;
        int toqueActual = 0;
        int ruletaActual = 0;
        int delay = 45;

        while (toqueActual < toques && esperandoRuleta && !partidaCancelada)
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

            SistemaSonido.SonarRuleta();
            await Task.Delay(delay);
        }

        // Ganador
        ganaAnfitrión = (ruletaActual > ((ruleta.Length / 2) - 1));

        // Fin
        await Task.Delay(500);
    }

    private async Task MoverRuletaHuésped()
    {
        var apagado = true;
        var toques = 40;
        int toqueActual = 0;
        int delay = 45;

        while (toqueActual < toques && esperandoRuleta && !partidaCancelada)
        {
            // Cambia colores
            if (apagado)
                PrenderRuleta();
            else
                ApagarRuleta();

            toqueActual++;
            apagado = !apagado;

            if (toqueActual >= 20)
                delay += 20;

            SistemaSonido.SonarRuleta();
            await Task.Delay(delay);
        }
    }

    private async Task FinalizarRuleta()
    {
        btnVolver.Visibility = Visibility.Hidden;
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
            // Gana huésped / derecha
            fondoGanador.Color = colorHuésped;
            imgGanador.Source = ObtenerSprite(spriteHuésped);
            imgFlechaDerecha.Visibility = Visibility.Visible;

        }
        gridGanador.Visibility = Visibility.Visible;

        // Fin
        await Task.Delay(1800);
    }

    private void PrenderRuleta()
    {
        for (int i = 0; i < ruleta.Length; i++)
        {
            ruleta[i].Color = colorRuletaActiva;
        }
    }

    private void ApagarRuleta()
    {
        for (int i = 0; i < ruleta.Length; i++)
        {
            ruleta[i].Color = colorRuletaVacía;
        }
    }

    public void MostrarJugadorListo(TipoJugador tipoJugador)
    {
        if (tipoJugador == TipoJugador.anfitrión)
            imgAnfitriónListo.Color = colorRuletaActiva;
        else
            imgHuéspedListo.Color = colorRuletaActiva;
    }

    public void MostrarNombreFortaleza(string nombre, TipoJugador tipoJugador)
    {
        if (tipoJugador == TipoJugador.anfitrión)
            txtAnfitrión.Text = nombre;
        else
            txtHuésped.Text = nombre;
    }
}
