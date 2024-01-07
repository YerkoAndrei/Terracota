using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Events;
using Stride.UI.Panels;

namespace Terracota;
using static Sistema;
using static Constantes;

public class InterfazMenú : StartupScript
{
    public Entity rotador;

    private static Quaternion últimaRotación = Quaternion.Identity;

    private UIElement página;
    private Grid Opciones;
    private Grid animOpciones;
    private Grid animRemoto;
    private Grid gridRemoto;

    private EditText txtConexiónRemota;
    private TextBlock txtErrorConexión;
    private Grid btnComoAnfitrión;
    private Grid btnComoHuesped;

    private bool animando;

    public override void Start()
    {
        página = Entity.Get<UIComponent>().Page.RootElement;
        Opciones = página.FindVisualChildOfType<Grid>("Opciones");
        animOpciones = página.FindVisualChildOfType<Grid>("animOpciones");

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnLocal"), EnClicLocal);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnRemoto"), EnClicRemoto);

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnCrear"), EnClicCrear);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnOpciones"), EnClicOpciones);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSalir"), EnClicSalir);

        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("btnCréditos"), EnClicCréditos);
        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("PanelOscuroRemoto"), CerrarPaneles);

        // Conexiones
        gridRemoto = página.FindVisualChildOfType<Grid>("ConexiónRemota");
        animRemoto = página.FindVisualChildOfType<Grid>("animRemoto");
        gridRemoto.Visibility = Visibility.Hidden;

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnRemotoVolver"), CerrarPaneles);

        // P2P
        txtConexiónRemota = página.FindVisualChildOfType<EditText>("txtConexiónRemota");
        txtConexiónRemota.Text = string.Empty;

        txtErrorConexión = página.FindVisualChildOfType<TextBlock>("txtErrorConexión");
        txtErrorConexión.Text = string.Empty;

        btnComoAnfitrión = página.FindVisualChildOfType<Grid>("btnComoAnfitrión");
        btnComoHuesped = página.FindVisualChildOfType<Grid>("btnComoHuesped");
        ConfigurarBotón(btnComoAnfitrión, () => { EnClicConectarRemoto(TipoJugador.anfitrión); });
        ConfigurarBotón(btnComoHuesped, () => { EnClicConectarRemoto(TipoJugador.huesped); });

        txtConexiónRemota = página.FindVisualChildOfType<EditText>("txtConexiónRemota");
        txtConexiónRemota.TextChanged += VerificarFuente;

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

    private void VerificarFuente(object sender, RoutedEventArgs args)
    {
        // Solo números y puntos
        string regex = @"[^0-9.]";
        txtConexiónRemota.Text = Regex.Replace(txtConexiónRemota.Text, regex, "");

        var fuente = SistemaTraducción.VerificarFuente(txtConexiónRemota.Text);

        if (txtConexiónRemota.Font != fuente)
            txtConexiónRemota.Font = fuente;
    }

    private void EnClicRemoto()
    {
        if (animando)
            return;

        página.FindVisualChildOfType<TextBlock>("txtLocalActual").Text = SistemaRed.ObtenerIP(TipoConexión.local);
        página.FindVisualChildOfType<TextBlock>("txtGlobalActual").Text = SistemaRed.ObtenerIP(TipoConexión.global);

        animando = true;
        gridRemoto.Visibility = Visibility.Visible;
        SistemaAnimación.AnimarElemento(animRemoto, 0.2f, true, Direcciones.arriba, TipoCurva.rápida, () =>
        {
            animando = false;
        });
    }

    private async void EnClicConectarRemoto(TipoJugador tipoJugador)
    {
        if (animando)
            return;

        txtErrorConexión.Text = string.Empty;

        // PENDIENTE: identificar LAN o P2P
        var resultado = await SistemaRed.ConectarDispositivo(txtConexiónRemota.Text, TipoConexión.global, tipoJugador, true);

        if (!string.IsNullOrEmpty(resultado))
            txtErrorConexión.Text = resultado;
    }

    private void CerrarPaneles()
    {
        if (animando)
            return;

        animando = true;
        txtErrorConexión.Text = string.Empty;

        if (gridRemoto.Visibility == Visibility.Visible)
        {
            SistemaAnimación.AnimarElemento(animRemoto, 0.2f, false, Direcciones.arriba, TipoCurva.rápida, () =>
            {
                animando = false;
                gridRemoto.Visibility = Visibility.Hidden;
            });
        }
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

        try
        {
            // Abre navegador
            Process.Start(new ProcessStartInfo { FileName = "https://yerkoandrei.itch.io", UseShellExecute = true });
        }
        catch { }
    }

    private void EnClicSalir()
    {
        if (animando)
            return;

        Environment.Exit(0);
    }
}
