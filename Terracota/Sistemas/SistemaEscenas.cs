using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Engine;
using Stride.Graphics;
using Stride.Graphics.SDL;
using Stride.Rendering.Compositing;
using Stride.UI;
using Stride.UI.Panels;

namespace Terracota;
using static Constantes;

public class SistemaEscenas : SyncScript
{
    public UrlReference<Scene> escenaMenú;
    public UrlReference<Scene> escenaCreación;
    public UrlReference<Scene> escenaLocal;
    public UrlReference<Scene> escenaLAN;
    public UrlReference<Scene> escenaP2P;

    public GraphicsCompositor compositorBajo;
    public GraphicsCompositor compositorMedio;
    public GraphicsCompositor compositorAlto;

    public Texture cursor;

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

    public override void Start()
    {
        // Pantalla completa
        //Game.Window.PreferredFullscreenSize = new Int2(1920, 1080);
        //Game.Window.IsFullscreen = true;

        // Cursor
        var cursorBytes = Encoding.ASCII.GetBytes(cursor.ToString());
        var nuevoCursor = new Cursor(cursorBytes, cursorBytes, 0, 0, 0, 0);
        Cursor.SetCursor(nuevoCursor);

        // Predeterminado
        instancia = this;
        duraciónLerp = 0.2f;

        var página = Entity.Get<UIComponent>().Page.RootElement;
        panelOscuro = página.FindVisualChildOfType<Grid>("PanelOscuro");

        escenaActual = Content.Load(escenaMenú);
        Entity.Scene.Children.Add(escenaActual);

        // Traduciones
        SistemaTraducción.ActualizarTextosEscena();
    }

    public override void Update()
    {
        if(ocultando)
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
            case Escenas.LAN:
                escenaActual = Content.Load(escenaLAN);
                break;
            case Escenas.P2P:
                escenaActual = Content.Load(escenaP2P);
                break;
        }

        // Carga
        Entity.Scene.Children.Add(escenaActual);

        // Retraso predeterminado
        await Task.Delay(200);

        // Traduciones
        SistemaTraducción.ActualizarTextosEscena();

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
