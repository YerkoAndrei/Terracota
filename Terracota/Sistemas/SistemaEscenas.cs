using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Engine;
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
    public UrlReference<Scene> escenaLAN;
    public UrlReference<Scene> escenaP2P;

    private static SistemaEscenas instancia;

    private static Escenas siguienteEscena;
    private static Scene escenaActual;
    private static bool ocultando;
    private static bool abriendo;

    // Lerp
    private float duraciónLerp;
    private float tiempoDelta;
    private float tiempo;

    private ImageElement panelOscuro;

    public override void Start()
    {
        instancia = this;
        duraciónLerp = 1.2f;

        var página = Entity.Get<UIComponent>().Page.RootElement;
        panelOscuro = página.FindVisualChildOfType<ImageElement>("PanelOscuro");

        escenaActual = Content.Load(escenaMenú);
        Entity.Scene.Children.Add(escenaActual);
    }

    public override void Update()
    {
        if(ocultando)
        {
            tiempoDelta += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            tiempo = tiempoDelta / duraciónLerp;

            //Log.Warning(panelOscuro.Opacity.ToString());
            //panelOscuro.Opacity = (float)MathUtil.Lerp(0, 1, tiempo);
            panelOscuro.Opacity = 1;
            panelOscuro.Color = Color.Lerp(Color.White, Color.Black, tiempo);

            // Fin
            if (tiempoDelta >= duraciónLerp)
            {
                panelOscuro.Opacity = 1;
                CambiarEscena();
                ocultando = false;
            }
        }

        if (abriendo)
        {
            tiempoDelta += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            tiempo = tiempoDelta / duraciónLerp;

            //panelOscuro.Opacity = MathUtil.Lerp(1, 0, tiempo);
            panelOscuro.Opacity = 1;
            panelOscuro.Color = Color.Lerp(Color.White, Color.Black, tiempo);

            // Fin
            if (tiempoDelta >= duraciónLerp)
            {
                panelOscuro.Opacity = 0;
                panelOscuro.IsEnabled = false;
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
        instancia.panelOscuro.IsEnabled = true;

        siguienteEscena = escena;
        ocultando = true;
        abriendo = false;
    }

    private void CambiarEscena()
    {
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

        Entity.Scene.Children.Add(escenaActual);
        ocultando = false;
        abriendo = true;
    }
}
