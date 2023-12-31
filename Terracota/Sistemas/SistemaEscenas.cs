using System.Threading.Tasks;
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
    private float duraciónLerp;
    private float tiempoDelta;
    private float tiempo;

    private Grid panelOscuro;
    private ImageElement imgCursor;

    public override void Start()
    {
        instancia = this;

        // Pantalla completa
        Game.Window.AllowUserResizing = true;
        Game.Window.Title = SistemaTraducción.ObtenerTraducción("nombreJuego");
        var pantallaCompleta = bool.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.pantallaCompleta));

        // Resolución
        var resolución = SistemaMemoria.ObtenerConfiguración(Configuraciones.resolución).Split('x');
        var ancho = int.Parse(resolución[0]);
        var alto = int.Parse(resolución[1]);

        CambiarPantalla(pantallaCompleta, ancho, alto);

        // Predeterminado
        duraciónLerp = 0.2f;

        var página = Entity.Get<UIComponent>().Page.RootElement;
        panelOscuro = página.FindVisualChildOfType<Grid>("PanelOscuro");

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
            tiempoDelta += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            tiempo = SistemaAnimación.EvaluarSuave(tiempoDelta / duraciónLerp);
            panelOscuro.BackgroundColor = Color.Lerp(Color.Transparent, Color.Black, tiempo);

            // Fin
            if (tiempoDelta >= duraciónLerp)
                CargarEscena();
        }

        if (abriendo)
        {
            tiempoDelta += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            tiempo = SistemaAnimación.EvaluarSuave(tiempoDelta / duraciónLerp);
            panelOscuro.BackgroundColor = Color.Lerp(Color.Black, Color.Transparent, tiempo);

            // Fin
            if (tiempoDelta >= duraciónLerp)
            {
                panelOscuro.BackgroundColor = Color.Transparent;
                panelOscuro.CanBeHitByUser = false;
                abriendo = false;
            }
        }
    }

    public void PosicionarCursor()
    {
        // Cursor es un ImageElement
        // ¿Optimizar?
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
        instancia.panelOscuro.BackgroundColor = Color.Transparent;
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
        panelOscuro.BackgroundColor = Color.Black;

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

        // Retraso predeterminado
        await Task.Delay(200);

        // Traduciones
        SistemaTraducción.ActualizarTextosEscena();

        // Red
        if(conectar)
            SistemaRed.Conectar();

        abriendo = true;
    }

    public static GraphicsCompositor ObtenerGráficos(NivelesConfiguración nivel)
    {
        switch (nivel)
        {
            case NivelesConfiguración.bajo:
                return instancia.compositorBajo;
            case NivelesConfiguración.medio:
                return instancia.compositorMedio;
            default:
            case NivelesConfiguración.alto:
                return instancia.compositorAlto;
        }
    }
}
