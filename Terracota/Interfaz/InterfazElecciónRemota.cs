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

    private ImageElement imgAnfitriónListo;
    private ImageElement imgHuespedListo;

    private Grid btnComenzar;
    private Grid btnVolver;
    private Grid btnAleatorio;

    private bool iniciada;
    private bool esperandoRuleta;
    private bool ganaAnfitrión;
    private bool partidaCancelada;

    // Animación
    private Grid animSuperior;

    public override void Start()
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
        imgHuespedListo = página.FindVisualChildOfType<ImageElement>("imgHuespedListo");

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
                var nuevaRanuraHuesped = prefabRanura.InstantiateElement<Grid>("RanuraDerecha");
                ConfigurarRanuraElección(nuevaRanuraHuesped, i, fortalezaTemp.Nombre, () => EnClicHuesped(fortalezaTemp.Nombre));
                padreRanurasHuesped.Children.Add(nuevaRanuraHuesped);
                padreRanurasHuesped.Height += (nuevaRanuraHuesped.Height + 10);
            }
        }
        iniciada = true;

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
        visorHuesped.Visibility = Visibility.Hidden;

        // Carga fortaleza
        if (SistemaRed.ObtenerTipoJugador() == TipoJugador.anfitrión)
        {
            var ranuraAnfitrión = ObtenerRanuraAleatoria();
            EnClicAnfitrión(ranuraAnfitrión);
        }
        else
        {
            var ranuraHuesped = ObtenerRanuraAleatoria();
            EnClicHuesped(ranuraHuesped);
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

    private void EnClicHuesped(string nombre)
    {
        if (esperandoRuleta)
            return;

        BloquearBotón(btnComenzar, false);

        txtHuesped.Text = nombre;
        controladorPartida.CargarFortaleza(nombre, false);
    }

    private void EnClicComenzar()
    {
        if (esperandoRuleta)
            return;

        visorAnfitrión.Visibility = Visibility.Hidden;
        visorHuesped.Visibility = Visibility.Hidden;

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
            _ = SistemaRed.EnviarData(DataRed.anfitriónListo);

            // Anfitrión comprueba que estén listos
            controladorPartida.RevisarJugadoresListos(SistemaRed.ObtenerTipoJugador());
        }
        else
            _ = SistemaRed.EnviarData(DataRed.huespedListo);
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
            await MoverRuletaHuesped();
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

    private async Task MoverRuletaHuesped()
    {
        var apagado = true;
        var toques = 40;
        int toqueActual = 0;
        int delay = 30;

        while (toqueActual < toques && esperandoRuleta && !partidaCancelada)
        {
            // Cambia colores
            if (apagado)
                PrenderRuleta();
            else
                ApagarRuleta();

            toqueActual++;
            apagado = !apagado;
            delay += 10;

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
            // Gana huesped / derecha
            fondoGanador.Color = colorHuesped;
            imgGanador.Source = ObtenerSprite(spriteHuesped);
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

    public bool ObtenerIniciada()
    {
        return iniciada;
    }

    public void MostrarJugadorListo(TipoJugador tipoJugador)
    {
        if (tipoJugador == TipoJugador.anfitrión)
            imgAnfitriónListo.Color = colorRuletaActiva;
        else
            imgHuespedListo.Color = colorRuletaActiva;
    }

    public void MostrarNombreFortaleza(string nombre, TipoJugador tipoJugador)
    {
        if (tipoJugador == TipoJugador.anfitrión)
            txtAnfitrión.Text = nombre;
        else
            txtHuesped.Text = nombre;
    }
}
