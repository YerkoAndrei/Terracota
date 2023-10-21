using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Events;
using System;

namespace Terracota;

public class ControladorMenú : StartupScript
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

        var btnLocal = página.FindVisualChildOfType<Button>("btnLocal");
        var btnRed = página.FindVisualChildOfType<Button>("btnRed");
        var btnP2P = página.FindVisualChildOfType<Button>("btnP2P");

        var btnCrear = página.FindVisualChildOfType<Button>("btnCrear");
        var btnOpciones = página.FindVisualChildOfType<Button>("btnOpciones");
        var btnSalir = página.FindVisualChildOfType<Button>("btnSalir");

        var btnCréditos = página.FindVisualChildOfType<Button>("btnCréditos");

        btnLocal.Click += EnClicLocal;
        btnRed.Click += EnClicRed;
        btnP2P.Click += EnClicP2P;

        btnCrear.Click += EnClicCrear;
        btnOpciones.Click += EnClicOpciones;
        btnSalir.Click += EnClicSalir;
    }

    private void EnClicLocal(object sender, RoutedEventArgs e)
    {
        Content.Unload(SceneSystem.SceneInstance.RootScene);
        SceneSystem.SceneInstance.RootScene = Content.Load(escenaLocal);
    }

    private void EnClicRed(object sender, RoutedEventArgs e)
    {
        // Abrir menú correspondiente
    }

    private void EnClicP2P(object sender, RoutedEventArgs e)
    {
        // Abrir menú correspondiente
    }

    private void EnClicCrear(object sender, RoutedEventArgs e)
    {/*
        Content.Unload(SceneSystem.SceneInstance.RootScene);
        SceneSystem.SceneInstance.RootScene = Content.Load(escenaCreación);*/
    }

    private void EnClicOpciones(object sender, RoutedEventArgs e)
    {
        // Abrir menú correspondiente
    }

    private void EnClicSalir(object sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }
}
