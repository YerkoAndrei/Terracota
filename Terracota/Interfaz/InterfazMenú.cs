using Stride.Core.Serialization;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;
using System;

namespace Terracota;
using static Sistema;

public class InterfazMenú : StartupScript
{
    public UrlReference<Scene> escenaLocal;
    public UrlReference<Scene> escenaRed;
    public UrlReference<Scene> escenaP2P;

    public UrlReference<Scene> escenaCreación;

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
        Content.Unload(SceneSystem.SceneInstance.RootScene);
        SceneSystem.SceneInstance.RootScene = Content.Load(escenaLocal);
    }

    private void EnClicLAN()
    {
        // Abrir menú correspondiente
    }

    private void EnClicP2P()
    {
        // Abrir menú correspondiente
    }

    private void EnClicCrear()
    {
        Content.Unload(SceneSystem.SceneInstance.RootScene);
        SceneSystem.SceneInstance.RootScene = Content.Load(escenaCreación);
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
