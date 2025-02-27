﻿using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Engine;
using Stride.Rendering.Compositing;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;

namespace Terracota;
using static Constantes;

public class SistemaEscenas : SyncScript
{
    public UrlReference<Scene> escenaMenú;
    public UrlReference<Scene> escenaCreación;
    public UrlReference<Scene> escenaLocal;
    public UrlReference<Scene> escenaRemota;

    public GraphicsCompositor compositorBajo;
    public GraphicsCompositor compositorMedio;
    public GraphicsCompositor compositorAlto;

    private static SistemaEscenas instancia;

    private static Escenas siguienteEscena;
    private static Scene escenaActual;
    private static bool ocultando;
    private static bool abriendo;

    // Lerp
    private float duraciónOcultar;
    private float duraciónAbrir;
    private float tiempoDelta;
    private float tiempo;

    private Grid panelOscuro;
    private ImageElement imgCursor;

    public override void Start()
    {
        instancia = this;

        // Pantalla completa
        Game.Window.AllowUserResizing = false;
        Game.Window.Title = SistemaTraducción.ObtenerTraducción("nombreJuego");
        
        // Resolución
        var resolución = SistemaMemoria.ObtenerConfiguración(Configuraciones.resolución).Split('x');
        var pantallaCompleta = bool.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.pantallaCompleta));

        var ancho = int.Parse(resolución[0]);
        var alto = int.Parse(resolución[1]);
        CambiarPantalla(pantallaCompleta, ancho, alto);

        // Predeterminado
        var página = Entity.Get<UIComponent>().Page.RootElement;
        panelOscuro = página.FindVisualChildOfType<Grid>("PanelOscuro");
        panelOscuro.Opacity = 0;
        duraciónOcultar = 0.2f;
        duraciónAbrir = 0.4f;

        escenaActual = Content.Load(escenaMenú);
        Entity.Scene.Children.Add(escenaActual);

        // Traduciones
        SistemaTraducción.ActualizarTextosEscena();

        // Cursor
        Game.Window.IsMouseVisible = false;
        imgCursor = página.FindVisualChildOfType<ImageElement>("imgCursor");
    }

    public override void Update()
    {
        // Cursor es una imagen
        PosicionarCursor();

        if (ocultando)
        {
            tiempoDelta += (float)Game.UpdateTime.WarpElapsed.TotalSeconds;
            tiempo = SistemaAnimación.EvaluarSuave(tiempoDelta / duraciónOcultar);
            panelOscuro.Opacity = MathUtil.Lerp(0f, 1f, tiempo);

            // Fin
            if (tiempoDelta >= duraciónOcultar)
                CargarEscena();
        }

        if (abriendo)
        {
            tiempoDelta += (float)Game.UpdateTime.WarpElapsed.TotalSeconds;
            tiempo = SistemaAnimación.EvaluarSuave(tiempoDelta / duraciónAbrir);
            panelOscuro.Opacity = MathUtil.Lerp(1f, 0f, tiempo);

            // Fin
            if (tiempoDelta >= duraciónAbrir)
                TerminarCarga();
        }
    }

    public void PosicionarCursor()
    {
        // Cursor es un ImageElement
        float left = 0;
        float top = 0;
        float right = 0;
        float bottom = 0;

        if (Input.MousePosition.X > 0.5f)
            left = (Input.MousePosition.X - 0.5f) * 2 * 1280;
        else
            right = (0.5f - Input.MousePosition.X) * 2 * 1280;

        if (Input.MousePosition.Y > 0.5f)
            top = (Input.MousePosition.Y - 0.5f) * 2 * 720;
        else
            bottom = (0.5f - Input.MousePosition.Y) * 2 * 720;

        imgCursor.Margin = new Thickness(left, top, right, bottom);
    }

    public static void CambiarPantalla(bool pantallaCompleta, int ancho, int alto)
    {
        instancia.Game.Window.Visible = false;

        instancia.Game.Window.SetSize(new Int2(ancho, alto));
        instancia.Game.Window.PreferredWindowedSize = new Int2(ancho, alto);
        instancia.Game.Window.PreferredFullscreenSize = new Int2(ancho, alto);
        instancia.Game.Window.IsFullscreen = pantallaCompleta;

        instancia.Game.Window.Visible = true;
    }

    public static void CambiarEscena(Escenas escena)
    {
        if (ocultando || abriendo)
            return;

        instancia.tiempo = 0;
        instancia.tiempoDelta = 0;
        instancia.panelOscuro.Opacity = 0;
        instancia.panelOscuro.CanBeHitByUser = true;

        siguienteEscena = escena;
        ocultando = true;
        abriendo = false;
    }

    private async void CargarEscena()
    {
        var conectar = false;
        ocultando = false;

        tiempo = 0;
        tiempoDelta = 0;
        instancia.panelOscuro.Opacity = 1;
        await Task.Delay(10);

        // Descarga
        Content.Unload(escenaActual);
        Entity.Scene.Children.Remove(escenaActual);

        switch (siguienteEscena)
        {
            case Escenas.menú:
                escenaActual = Content.Load(escenaMenú);
                break;
            case Escenas.creación:
                escenaActual = Content.Load(escenaCreación);
                break;
            case Escenas.local:
                escenaActual = Content.Load(escenaLocal);
                break;
            case Escenas.remoto:
                escenaActual = Content.Load(escenaRemota);
                conectar = true;
                break;
        }

        // Carga
        Entity.Scene.Children.Add(escenaActual);
        await Task.Delay(10);

        // Retraso predeterminado
        await Task.Delay(200);

        // Traduciones
        SistemaTraducción.ActualizarTextosEscena();

        // Red
        if(conectar)
            SistemaRed.Conectar();

        abriendo = true;
    }
    
    private void TerminarCarga()
    {
        tiempo = 0;
        tiempoDelta = 0;
        panelOscuro.Opacity = 0;
        panelOscuro.CanBeHitByUser = false;
        abriendo = false;
    }

    public static GraphicsCompositor ObtenerGráficos(Calidades nivel)
    {
        switch (nivel)
        {
            case Calidades.bajo:
                return instancia.compositorBajo;
            case Calidades.medio:
                return instancia.compositorMedio;
            default:
            case Calidades.alto:
                return instancia.compositorAlto;
        }
    }
}
