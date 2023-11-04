using Stride.Core.Serialization;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;
using System;

namespace Terracota;
using static Sistema;
using static Constantes;

public class InterfazMenú : StartupScript
{
    public override void Start()
    {
        //Game.Window.PreferredFullscreenSize = new Int2(1920, 1080);
        //Game.Window.IsFullscreen = true;

        var página = Entity.Get<UIComponent>().Page.RootElement;

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnLocal"), EnClicLocal);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnLAN"), EnClicLAN);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnP2P"), EnClicP2P);

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnCrear"), EnClicCrear);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnOpciones"), EnClicOpciones);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSalir"), EnClicSalir);

        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("btnCréditos"), EnClicCréditos);
    }

    private void EnClicLocal()
    {
        SistemaEscenas.CambiarEscena(Escenas.local);
    }

    private void EnClicLAN()
    {
        // Abrir menú correspondiente
        //SistemaEscenas.CambiarEscena(Escenas.LAN);
    }

    private void EnClicP2P()
    {
        // Abrir menú correspondiente
        //SistemaEscenas.CambiarEscena(Escenas.P2P);
    }

    private void EnClicCrear()
    {
        SistemaEscenas.CambiarEscena(Escenas.creación);
    }

    private void EnClicOpciones()
    {
        // Abrir panel
    }

    private void EnClicCréditos()
    {
        // Abrir panel
    }

    private void EnClicSalir()
    {
        Environment.Exit(0);
    }
}
