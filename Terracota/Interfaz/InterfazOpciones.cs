using Stride.Engine;
using Stride.Rendering.Compositing;
using Stride.UI.Controls;
using Stride.UI;
using Stride.UI.Panels;
using Stride.UI.Events;
using System;
using System.Globalization;

namespace Terracota;
using static Sistema;
using static Constantes;
using static Terracota.Constantes;

public class InterfazOpciones : StartupScript
{
    public GraphicsCompositor compositor;

    private UIElement página;
    private Grid Opciones;
    private Grid animOpciones;
    private bool animando;

    public override void Start()
    {
        //compositor.Game.

        // Botones y deslisadores
        página = Entity.Get<UIComponent>().Page.RootElement;
        Opciones = página.FindVisualChildOfType<Grid>("Opciones");
        Opciones.Visibility = Visibility.Hidden;
        animOpciones = página.FindVisualChildOfType<Grid>("animOpciones");

        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("PanelOscuroOpciones"), EnClicVolver);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnVolver"), EnClicVolver);

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnEspañol"), () => EnClicIdioma(Idiomas.español));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnInglés"), () => EnClicIdioma(Idiomas.inglés));

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btn30"), () => EnClicVelocidadRed(30));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btn60"), () => EnClicVelocidadRed(60));

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnGráficosBajos"), () => EnClicGráficos(NivelesConfiguración.bajo));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnGráficosMedios"), () => EnClicGráficos(NivelesConfiguración.medio));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnGráficosAltos"), () => EnClicGráficos(NivelesConfiguración.alto));

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSombrasBajas"), () => EnClicSombras(NivelesConfiguración.bajo));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSombrasMedias"), () => EnClicSombras(NivelesConfiguración.medio));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSombrasAltas"), () => EnClicSombras(NivelesConfiguración.alto));

        var sliderGeneral = página.FindVisualChildOfType<Slider>("SliderGeneral");
        var sliderMúsica = página.FindVisualChildOfType<Slider>("SliderMúsica");
        var sliderEfectos = página.FindVisualChildOfType<Slider>("SliderEfectos");

        // Carga
        BloquearIdioma(         (Idiomas)Enum.Parse(typeof(Idiomas), SistemaMemoria.ObtenerConfiguración(Configuraciones.idioma)));
        BloquearVelocidadRed(   int.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.velocidadRed)));
        BloquearGráficos(       (NivelesConfiguración)Enum.Parse(typeof(NivelesConfiguración), SistemaMemoria.ObtenerConfiguración(Configuraciones.gráficos)));
        BloquearSombras(        (NivelesConfiguración)Enum.Parse(typeof(NivelesConfiguración), SistemaMemoria.ObtenerConfiguración(Configuraciones.sombras)));

        sliderGeneral.Value =   float.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.volumenGeneral), CultureInfo.InvariantCulture);
        sliderMúsica.Value =    float.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.volumenMúsica), CultureInfo.InvariantCulture);
        sliderEfectos.Value =   float.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.volumenEfectos), CultureInfo.InvariantCulture);

        // Llamados slider
        sliderGeneral.ValueChanged += ConfigurarVolumenGeneral;
        sliderMúsica.ValueChanged += ConfigurarVolumenMúsica;
        sliderEfectos.ValueChanged += ConfigurarVolumenEfectos;
    }

    private void EnClicIdioma(Idiomas idioma)
    {
        if (animando)
            return;

        SistemaTraducción.CambiarIdioma(idioma);
        BloquearIdioma(idioma);
    }

    private void EnClicVelocidadRed(int velocidad)
    {
        if (animando)
            return;

        SistemaMemoria.GuardarConfiguración(Configuraciones.velocidadRed, velocidad.ToString());
        BloquearVelocidadRed(velocidad);
    }

    private void ConfigurarVolumenGeneral(object sender, RoutedEventArgs e)
    {
        if (animando)
            return;

        var slider = (Slider)sender;
        SistemaMemoria.GuardarConfiguración(Configuraciones.volumenGeneral, slider.Value.ToString("0.00", CultureInfo.InvariantCulture));
    }

    private void ConfigurarVolumenMúsica(object sender, RoutedEventArgs e)
    {
        if (animando)
            return;

        var slider = (Slider)sender;
        SistemaMemoria.GuardarConfiguración(Configuraciones.volumenMúsica, slider.Value.ToString("0.00", CultureInfo.InvariantCulture));
    }

    private void ConfigurarVolumenEfectos(object sender, RoutedEventArgs e)
    {
        if (animando)
            return;

        var slider = (Slider)sender;
        SistemaMemoria.GuardarConfiguración(Configuraciones.volumenEfectos, slider.Value.ToString("0.00", CultureInfo.InvariantCulture));
    }

    private void EnClicGráficos(NivelesConfiguración nivel)
    {
        if (animando)
            return;

        SistemaMemoria.GuardarConfiguración(Configuraciones.gráficos, nivel.ToString());
        BloquearGráficos(nivel);
    }

    private void EnClicSombras(NivelesConfiguración nivel)
    {
        if (animando)
            return;

        SistemaMemoria.GuardarConfiguración(Configuraciones.sombras, nivel.ToString());
        BloquearSombras(nivel);
    }

    private void EnClicPantallaCompleta(bool pantallaCompleta)
    {
        if (animando)
            return;

        SistemaMemoria.GuardarConfiguración(Configuraciones.pantallaCompleta, pantallaCompleta.ToString());
    }

    private void EnClicResolución(int ancho, int alto)
    {
        if (animando)
            return;

        var resolución = ancho.ToString() + "x" + alto.ToString();
        SistemaMemoria.GuardarConfiguración(Configuraciones.resolución, resolución);
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
                break;
            case 60:
                BloquearBotón(página.FindVisualChildOfType<Grid>("btn30"), false);
                BloquearBotón(página.FindVisualChildOfType<Grid>("btn60"), true);
                break;
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

    private void EnClicVolver()
    {
        if (animando)
            return;

        animando = true;
        SistemaAnimación.AnimarElemento(animOpciones, 0.2f, false, Direcciones.abajo, TipoCurva.rápida, () =>
        {
            Opciones.Visibility = Visibility.Hidden;
            animando = false;
        });
    }
}
