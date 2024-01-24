using System;
using System.Linq;
using System.Globalization;
using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI;
using Stride.UI.Panels;
using Stride.UI.Events;
using System.Text.RegularExpressions;

namespace Terracota;
using static Sistema;
using static Constantes;

public class InterfazOpciones : StartupScript
{
    private UIElement página;
    private Grid Opciones;
    private Grid animOpciones;
    private UniformGrid resoluciones;
    private TextBlock resoluciónActual;
    private EditText txtPuerto;
    private bool animando;

    public override void Start()
    {
        // Botones y deslisadores
        página = Entity.Get<UIComponent>().Page.RootElement;
        Opciones = página.FindVisualChildOfType<Grid>("Opciones");
        Opciones.Visibility = Visibility.Hidden;
        animOpciones = página.FindVisualChildOfType<Grid>("animOpciones");

        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("PanelOscuroOpciones"), EnClicVolver);
        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("btnPanel"), () => MostrarResoluciones(false));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnVolver"), EnClicVolver);

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnEspañol"), () => EnClicIdioma(Idiomas.español));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnInglés"), () => EnClicIdioma(Idiomas.inglés));

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btn30"), () => EnClicVelocidadRed(30));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btn60"), () => EnClicVelocidadRed(60));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btn120"), () => EnClicVelocidadRed(120));

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnGráficosBajos"), () => EnClicGráficos(NivelesConfiguración.bajo));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnGráficosMedios"), () => EnClicGráficos(NivelesConfiguración.medio));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnGráficosAltos"), () => EnClicGráficos(NivelesConfiguración.alto));

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSombrasBajas"), () => EnClicSombras(NivelesConfiguración.bajo));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSombrasMedias"), () => EnClicSombras(NivelesConfiguración.medio));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSombrasAltas"), () => EnClicSombras(NivelesConfiguración.alto));

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnCompleta"), () => EnClicPantallaCompleta(true));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnVentana"), () => EnClicPantallaCompleta(false));

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnResoluciones"), () => MostrarResoluciones(true));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnR0"), () => EnClicResolución(960, 540, "btnR0"));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnR1"), () => EnClicResolución(1280, 720, "btnR1"));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnR2"), () => EnClicResolución(1366, 768, "btnR2"));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnR3"), () => EnClicResolución(1600, 900, "btnR3"));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnR4"), () => EnClicResolución(1920, 1080, "btnR4"));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnR5"), () => EnClicResolución(2560, 1440, "btnR5"));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnR6"), () => EnClicResolución(3840, 2160, "btnR6"));

        txtPuerto = página.FindVisualChildOfType<EditText>("txtPuerto");
        txtPuerto.Text = SistemaMemoria.ObtenerConfiguración(Configuraciones.puertoRed);
        txtPuerto.TextChanged += ConfigurarPuerto;

        resoluciónActual = página.FindVisualChildOfType<TextBlock>("txtResoluciónActual");
        resoluciónActual.Text = SistemaMemoria.ObtenerConfiguración(Configuraciones.resolución).Replace("x", " x ");

        resoluciones = página.FindVisualChildOfType<UniformGrid>("ListaResoluciones");
        resoluciones.Visibility = Visibility.Hidden;

        var sliderGeneral = página.FindVisualChildOfType<Slider>("SliderGeneral");
        var sliderMúsica = página.FindVisualChildOfType<Slider>("SliderMúsica");
        var sliderEfectos = página.FindVisualChildOfType<Slider>("SliderEfectos");

        // Carga
        BloquearIdioma(         (Idiomas)Enum.Parse(typeof(Idiomas), SistemaMemoria.ObtenerConfiguración(Configuraciones.idioma)));
        BloquearVelocidadRed(   int.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.velocidadRed)));
        BloquearGráficos(       (NivelesConfiguración)Enum.Parse(typeof(NivelesConfiguración), SistemaMemoria.ObtenerConfiguración(Configuraciones.gráficos)));
        BloquearSombras(        (NivelesConfiguración)Enum.Parse(typeof(NivelesConfiguración), SistemaMemoria.ObtenerConfiguración(Configuraciones.sombras)));
        BloquearPantallaCompleta(bool.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.pantallaCompleta)));

        DesbloquearBotonesResolución();
        switch(SistemaMemoria.ObtenerConfiguración(Configuraciones.resolución))
        {
            case "960x540":
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnR0"), true);
                break;
            case "1280x720":
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnR1"), true);
                break;
            case "1366x768":
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnR2"), true);
                break;
            case "1600x900":
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnR3"), true);
                break;
            case "1920x1080":
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnR4"), true);
                break;
            case "2560x1440":
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnR5"), true);
                break;
            case "3840x2160":
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnR6"), true);
                break;
        }

        sliderGeneral.Value =   float.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.volumenGeneral), CultureInfo.InvariantCulture);
        sliderMúsica.Value =    float.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.volumenMúsica), CultureInfo.InvariantCulture);
        sliderEfectos.Value =   float.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.volumenEfectos), CultureInfo.InvariantCulture);

        // Actualiza gráficos
        ActualizaGráficos(      (NivelesConfiguración)Enum.Parse(typeof(NivelesConfiguración), SistemaMemoria.ObtenerConfiguración(Configuraciones.gráficos)));
        ActualizaSombras();

        // Llamados slider
        sliderGeneral.ValueChanged += ConfigurarVolumenGeneral;
        sliderMúsica.ValueChanged += ConfigurarVolumenMúsica;
        sliderEfectos.ValueChanged += ConfigurarVolumenEfectos;

        // Sonidos
        sliderGeneral.TouchDown += (s, a) => { SistemaSonido.SonarBotónEntra(); };
        sliderGeneral.TouchUp += (s, a) => { SistemaSonido.SonarBotónSale(); };

        sliderMúsica.TouchDown += (s, a) => { SistemaSonido.SonarBotónEntra(); };
        sliderMúsica.TouchUp += (s, a) => { SistemaSonido.SonarBotónSale(); };

        sliderEfectos.TouchDown += (s, a) => { SistemaSonido.SonarBotónEntra(); };
        sliderEfectos.TouchUp += (s, a) => { SistemaSonido.SonarBotónSale(); };
    }

    private void EnClicIdioma(Idiomas idioma)
    {
        if (animando)
            return;
        SistemaTraducción.CambiarIdioma(idioma);
        BloquearIdioma(idioma);
        MostrarResoluciones(false);
    }

    private void EnClicVelocidadRed(int velocidad)
    {
        if (animando)
            return;

        SistemaMemoria.GuardarConfiguración(Configuraciones.velocidadRed, velocidad.ToString());
        SistemaRed.ActualizarConfiguración();
        BloquearVelocidadRed(velocidad);
        MostrarResoluciones(false);
    }

    private void ConfigurarPuerto(object sender, RoutedEventArgs e)
    {
        if (animando)
            return;

        MostrarResoluciones(false);
        if (string.IsNullOrEmpty(txtPuerto.Text))
        {
            txtPuerto.Text = "0";
            return;
        }

        // Solo números
        string regex = @"[^0-9]";
        var número = int.Parse(Regex.Replace(txtPuerto.Text, regex, ""));

        // Excepciones y límites
        if (número < 1 || número > 65535)
            número = 0;

        txtPuerto.Text = número.ToString();

        SistemaMemoria.GuardarConfiguración(Configuraciones.puertoRed, txtPuerto.Text);
        if (!SistemaRed.ActualizarConfiguración())
            txtPuerto.Text = "0";
    }
    
    private void ConfigurarVolumenGeneral(object sender, RoutedEventArgs e)
    {
        if (animando)
            return;

        var slider = (Slider)sender;
        SistemaMemoria.GuardarConfiguración(Configuraciones.volumenGeneral, slider.Value.ToString("0.00", CultureInfo.InvariantCulture));
        MostrarResoluciones(false);
        SistemaSonido.ActualizarVolumenMúsica();
    }

    private void ConfigurarVolumenMúsica(object sender, RoutedEventArgs e)
    {
        if (animando)
            return;

        var slider = (Slider)sender;
        SistemaMemoria.GuardarConfiguración(Configuraciones.volumenMúsica, slider.Value.ToString("0.00", CultureInfo.InvariantCulture));
        MostrarResoluciones(false);
        SistemaSonido.ActualizarVolumenMúsica();
    }

    private void ConfigurarVolumenEfectos(object sender, RoutedEventArgs e)
    {
        if (animando)
            return;

        var slider = (Slider)sender;
        SistemaMemoria.GuardarConfiguración(Configuraciones.volumenEfectos, slider.Value.ToString("0.00", CultureInfo.InvariantCulture));
        MostrarResoluciones(false);
    }

    private void EnClicGráficos(NivelesConfiguración nivel)
    {
        if (animando)
            return;

        SistemaMemoria.GuardarConfiguración(Configuraciones.gráficos, nivel.ToString());
        BloquearGráficos(nivel);
        ActualizaGráficos(nivel);
        MostrarResoluciones(false);
    }

    private void ActualizaGráficos(NivelesConfiguración nivel)
    {
        Services.GetService<SceneSystem>().GraphicsCompositor = SistemaEscenas.ObtenerGráficos(nivel);
    }

    private void EnClicSombras(NivelesConfiguración nivel)
    {
        if (animando)
            return;

        SistemaMemoria.GuardarConfiguración(Configuraciones.sombras, nivel.ToString());
        BloquearSombras(nivel);
        ActualizaSombras();
        MostrarResoluciones(false);
    }

    private void ActualizaSombras()
    {
        var controladorLuces = SceneSystem.SceneInstance.RootScene.Children[0].Entities.Where(o => o.Get<ControladorSombras>() != null).FirstOrDefault();
        controladorLuces.Get<ControladorSombras>().ActualizarSombras();
    }

    private void MostrarResoluciones(bool mostrar)
    {
        if (animando)
            return;

        if(mostrar)
        {
            if (resoluciones.Visibility == Visibility.Visible)
                resoluciones.Visibility = Visibility.Hidden;
            else
                resoluciones.Visibility = Visibility.Visible;
        }
        else
            resoluciones.Visibility = Visibility.Hidden;
    }

    private void EnClicResolución(int ancho, int alto, string botón)
    {
        if (animando)
            return;

        // Resolución
        var guardado = SistemaMemoria.ObtenerConfiguración(Configuraciones.resolución);
        var resolución = ancho.ToString() + "x" + alto.ToString();

        if (resolución == guardado)
        {
            MostrarResoluciones(false);
            return;
        }

        resoluciónActual.Text = resolución.Replace("x", " x ");
        SistemaMemoria.GuardarConfiguración(Configuraciones.resolución, resolución);
        ActualizarResolución(ancho, alto);
        MostrarResoluciones(false);

        DesbloquearBotonesResolución();
        BloquearBotón(página.FindVisualChildOfType<Grid>(botón), true);
    }

    private void ActualizarResolución(int ancho, int alto)
    {
        // Pantalla completa
        var pantallaCompleta = bool.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.pantallaCompleta));
        SistemaEscenas.CambiarPantalla(pantallaCompleta, ancho, alto);
    }

    private void EnClicPantallaCompleta(bool pantallaCompleta)
    {
        if (animando)
            return;

        var guardado = bool.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.pantallaCompleta));
        if (pantallaCompleta == guardado)
            return;

        SistemaMemoria.GuardarConfiguración(Configuraciones.pantallaCompleta, pantallaCompleta.ToString());
        BloquearPantallaCompleta(pantallaCompleta);
        ActualizaPantalla(pantallaCompleta);
        MostrarResoluciones(false);
    }

    private void ActualizaPantalla(bool pantallaCompleta)
    {
        // Resolución
        var resolución = SistemaMemoria.ObtenerConfiguración(Configuraciones.resolución).Split('x');
        var ancho = int.Parse(resolución[0]);
        var alto = int.Parse(resolución[1]);

        SistemaEscenas.CambiarPantalla(pantallaCompleta, ancho, alto);
    }

    // Bloqueos
    private void BloquearIdioma(Idiomas idioma)
    {
        switch (idioma)
        {
            case Idiomas.español:
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnEspañol"), true);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnInglés"), false);
                break;
            case Idiomas.inglés:
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnEspañol"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnInglés"), true);
                break;
        }
    }

    private void BloquearVelocidadRed(int velocidad)
    {
        switch (velocidad)
        {
            case 30:
                BloquearBotón(página.FindVisualChildOfType<Grid>("btn30"), true);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btn60"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btn120"), false);
                break;
            case 60:
                BloquearBotón(página.FindVisualChildOfType<Grid>("btn30"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btn60"), true);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btn120"), false);
                break;
            case 120:
                BloquearBotón(página.FindVisualChildOfType<Grid>("btn30"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btn60"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btn120"), true);
                break;
        }
    }

    private void BloquearPantallaCompleta(bool pantallaCompleta)
    {
        if (pantallaCompleta)
        {
            BloquearBotón(página.FindVisualChildOfType<Grid>("btnCompleta"), true);
            BloquearBotón(página.FindVisualChildOfType<Grid>("btnVentana"), false);
        }
        else
        {
            BloquearBotón(página.FindVisualChildOfType<Grid>("btnCompleta"), false);
            BloquearBotón(página.FindVisualChildOfType<Grid>("btnVentana"), true);
        }
    }

    private void BloquearGráficos(NivelesConfiguración nivel)
    {
        switch (nivel)
        {
            case NivelesConfiguración.bajo:
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnGráficosBajos"), true);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnGráficosMedios"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnGráficosAltos"), false);
                break;
            case NivelesConfiguración.medio:
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnGráficosBajos"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnGráficosMedios"), true);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnGráficosAltos"), false);
                break;
            case NivelesConfiguración.alto:
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnGráficosBajos"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnGráficosMedios"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnGráficosAltos"), true);
                break;
        }
    }

    private void BloquearSombras(NivelesConfiguración nivel)
    {
        switch (nivel)
        {
            case NivelesConfiguración.bajo:
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnSombrasBajas"), true);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnSombrasMedias"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnSombrasAltas"), false);
                break;
            case NivelesConfiguración.medio:
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnSombrasBajas"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnSombrasMedias"), true);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnSombrasAltas"), false);
                break;
            case NivelesConfiguración.alto:
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnSombrasBajas"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnSombrasMedias"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btnSombrasAltas"), true);
                break;
        }
    }

    private void DesbloquearBotonesResolución()
    {
        BloquearBotón(página.FindVisualChildOfType<Grid>("btnR0"), false);
        BloquearBotón(página.FindVisualChildOfType<Grid>("btnR1"), false);
        BloquearBotón(página.FindVisualChildOfType<Grid>("btnR2"), false);
        BloquearBotón(página.FindVisualChildOfType<Grid>("btnR3"), false);
        BloquearBotón(página.FindVisualChildOfType<Grid>("btnR4"), false);
        BloquearBotón(página.FindVisualChildOfType<Grid>("btnR5"), false);
        BloquearBotón(página.FindVisualChildOfType<Grid>("btnR6"), false);
    }

    private void EnClicVolver()
    {
        if (animando)
            return;

        animando = true;
        resoluciones.Visibility = Visibility.Hidden;
        SistemaAnimación.AnimarElemento(animOpciones, 0.2f, false, Direcciones.abajo, TipoCurva.rápida, () =>
        {
            Opciones.Visibility = Visibility.Hidden;
            animando = false;
        });
    }
}
