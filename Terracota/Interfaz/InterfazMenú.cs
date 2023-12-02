using System;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;

namespace Terracota;
using static Sistema;
using static Constantes;

public class InterfazMenú : StartupScript
{
    public Entity rotador;

    private static Quaternion últimaRotación = Quaternion.Identity;
    private Grid Opciones;
    private Grid animOpciones;
    private bool animando;

    public override void Start()
    {
        var página = Entity.Get<UIComponent>().Page.RootElement;
        Opciones = página.FindVisualChildOfType<Grid>("Opciones");
        animOpciones = página.FindVisualChildOfType<Grid>("animOpciones");

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnLocal"), EnClicLocal);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnLAN"), EnClicLAN);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnP2P"), EnClicP2P);

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnCrear"), EnClicCrear);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnOpciones"), EnClicOpciones);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSalir"), EnClicSalir);

        BloquearBotón(página.FindVisualChildOfType<Grid>("btnLAN"), true);
        BloquearBotón(página.FindVisualChildOfType<Grid>("btnP2P"), true);

        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("btnCréditos"), EnClicCréditos);

        // Recuerda última rotación
        rotador.Transform.Rotation = últimaRotación;
    }

    private void EnClicLocal()
    {
        if (animando)
            return;

        últimaRotación = rotador.Transform.Rotation;
        SistemaEscenas.CambiarEscena(Escenas.local);
    }

    private void EnClicLAN()
    {
        if (animando)
            return;

        // Abrir menú correspondiente
        //SistemaEscenas.CambiarEscena(Escenas.LAN);
    }

    private void EnClicP2P()
    {
        if (animando)
            return;

        // Abrir menú correspondiente
        //SistemaEscenas.CambiarEscena(Escenas.P2P);
    }

    private void EnClicCrear()
    {
        if (animando)
            return;

        últimaRotación = rotador.Transform.Rotation;
        SistemaEscenas.CambiarEscena(Escenas.creación);
    }

    private void EnClicOpciones()
    {
        if (animando)
            return;

        animando = true;
        Opciones.Visibility = Visibility.Visible;
        SistemaAnimación.AnimarElemento(animOpciones, 0.2f, true, Direcciones.abajo, TipoCurva.rápida, () =>
        {
            animando = false;
        });
    }

    private void EnClicCréditos()
    {
        if (animando)
            return;

        // Abrir panel
    }

    private void EnClicSalir()
    {
        if (animando)
            return;

        Environment.Exit(0);
    }
}
