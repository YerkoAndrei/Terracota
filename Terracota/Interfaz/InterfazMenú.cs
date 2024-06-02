using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Events;
using Stride.UI.Panels;

namespace Terracota;
using static Utilidades;
using static Constantes;

public class InterfazMenú : StartupScript
{
    public Entity rotador;
    public Color colorEspera;
    public Color colorActivo;

    private static Quaternion últimaRotación = Quaternion.Identity;

    private UIElement página;
    private Grid Opciones;
    private Grid animOpciones;
    private Grid animLAN;
    private Grid gridLAN;

    private Grid popupInvitación;
    private Grid animInvitación;
    private Grid btnSí;
    private Grid btnNo;

    private Grid popupEsperando;
    private Grid animEsperando;
    private ImageElement imgEsperando;
    private TextBlock txtEsperando;

    private Conexión conexiónPendiente;
    private TextBlock txtDatosInvitación;

    private EditText txtConexiónLAN;
    private TextBlock txtErrorConexión;
    private Grid btnComoAnfitrión;
    private Grid btnComoHuésped;

    private bool animando;
    private bool animandoEspera;

    public override void Start()
    {
        página = Entity.Get<UIComponent>().Page.RootElement;

        // Versión
        página.FindVisualChildOfType<TextBlock>("txtVersión").Text = "1.0";

        Opciones = página.FindVisualChildOfType<Grid>("Opciones");
        animOpciones = página.FindVisualChildOfType<Grid>("animOpciones");

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnLocal"), EnClicLocal);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnLAN"), EnClicLAN);

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnCrear"), EnClicCrear);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnOpciones"), EnClicOpciones);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSalir"), EnClicSalir);

        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("btnCréditos"), EnClicCréditos);
        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("PanelOscuroLAN"), CerrarPaneles);

        // Conexión
        gridLAN = página.FindVisualChildOfType<Grid>("ConexiónLAN");
        animLAN = página.FindVisualChildOfType<Grid>("animLAN");
        gridLAN.Visibility = Visibility.Hidden;

        txtConexiónLAN = página.FindVisualChildOfType<EditText>("txtConexiónLAN");
        txtConexiónLAN.TextChanged += VerificarFuente;

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnLANVolver"), CerrarPaneles);

        // LAN
        txtConexiónLAN = página.FindVisualChildOfType<EditText>("txtConexiónLAN");
        txtConexiónLAN.Text = string.Empty;

        txtErrorConexión = página.FindVisualChildOfType<TextBlock>("txtErrorConexión");
        txtErrorConexión.Text = string.Empty;

        btnComoAnfitrión = página.FindVisualChildOfType<Grid>("btnComoAnfitrión");
        btnComoHuésped = página.FindVisualChildOfType<Grid>("btnComoHuésped");
        ConfigurarBotón(btnComoAnfitrión, () => { EnClicConectarLAN(TipoJugador.anfitrión); });
        ConfigurarBotón(btnComoHuésped, () => { EnClicConectarLAN(TipoJugador.huésped); });

        // Invitación
        popupInvitación = página.FindVisualChildOfType<Grid>("PopupInvitación");
        animInvitación = página.FindVisualChildOfType<Grid>("animInvitación");
        txtDatosInvitación = página.FindVisualChildOfType<TextBlock>("txtDatosInvitación");
        btnSí = página.FindVisualChildOfType<Grid>("btnSí");
        btnNo = página.FindVisualChildOfType<Grid>("btnNo");

        ConfigurarBotón(btnSí, () => { CerrarInvitación(true); });
        ConfigurarBotón(btnNo, () => { CerrarInvitación(false); });
        popupInvitación.Visibility = Visibility.Hidden;

        // Esperando
        popupEsperando = página.FindVisualChildOfType<Grid>("PopupEsperando");
        animEsperando = página.FindVisualChildOfType<Grid>("animEsperando");
        txtEsperando = página.FindVisualChildOfType<TextBlock>("txtEsperando");
        imgEsperando = página.FindVisualChildOfType<ImageElement>("imgEsperando");

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnCancelarEsperando"), () => { SistemaRed.CerrarConexión(true); });

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
        txtConexiónLAN.Text = Regex.Replace(txtConexiónLAN.Text, regex, "");

        var fuente = SistemaTraducción.VerificarFuente(txtConexiónLAN.Text);

        if (txtConexiónLAN.Font != fuente)
            txtConexiónLAN.Font = fuente;
    }

    private void EnClicLAN()
    {
        if (animando)
            return;

        página.FindVisualChildOfType<TextBlock>("txtLocalActual").Text = SistemaRed.ObtenerIPLocal();

        animando = true;
        gridLAN.Visibility = Visibility.Visible;
        SistemaAnimación.AnimarElemento(animLAN, 0.2f, true, Direcciones.arriba, TipoCurva.rápida, () =>
        {
            animando = false;
        });
    }

    private async void EnClicConectarLAN(TipoJugador tipoJugador)
    {
        if (animando)
            return;

        txtErrorConexión.Text = string.Empty;
        BloquearBotón(btnComoAnfitrión, true);
        BloquearBotón(btnComoHuésped, true);

        // Reconoce si es dirección ip
        if(!SistemaRed.ValidarDirección(txtConexiónLAN.Text))
        {
            txtErrorConexión.Text = SistemaTraducción.ObtenerTraducción("errorIp");
            BloquearBotón(btnComoAnfitrión, false);
            BloquearBotón(btnComoHuésped, false);
            return;
        }

        // Reconoce que no sea la misma
        if (txtConexiónLAN.Text == SistemaRed.ObtenerIPLocal())
        {
            txtErrorConexión.Text = SistemaTraducción.ObtenerTraducción("errorIp");
            BloquearBotón(btnComoAnfitrión, false);
            BloquearBotón(btnComoHuésped, false);
            return;
        }

        // Conexión
        var resultado = await SistemaRed.ConectarDispositivo(txtConexiónLAN.Text, tipoJugador, true);

        if (string.IsNullOrEmpty(resultado))
        {
            // Esperando
            txtEsperando.Text = string.Format(SistemaTraducción.ObtenerTraducción("esperando"), txtConexiónLAN.Text);

            SistemaAnimación.AnimarElemento(animInvitación, 0.2f, false, Direcciones.arriba, TipoCurva.rápida, () =>
            {
                popupInvitación.Visibility = Visibility.Hidden;
            });

            animandoEspera = true;
            popupEsperando.Visibility = Visibility.Visible;
            SistemaAnimación.AnimarElemento(animEsperando, 0.2f, true, Direcciones.abajo, TipoCurva.rápida,  AnimarEspera);
        }
        else
        {
            txtErrorConexión.Text = SistemaTraducción.ObtenerTraducción(resultado);
            BloquearBotón(btnComoAnfitrión, false);
            BloquearBotón(btnComoHuésped, false);
        }
    }

    public async void AnimarEspera()
    {
        var espera = true;
        while(!SistemaRed.ObtenerJugando() || !animandoEspera)
        {
            if (espera)
                imgEsperando.Color = colorActivo;
            else
                imgEsperando.Color = colorEspera;

            await Task.Delay(500);
            espera = !espera;
        }
    }

    public void MostrarInvitación(Conexión conexión)
    {
        if (animando || conexiónPendiente != null)
            return;

        var solicitud = string.Empty;
        switch(conexión.ConectarComo)
        {
            case TipoJugador.anfitrión:
                solicitud = SistemaTraducción.ObtenerTraducción("anfitrión");
                break;
            case TipoJugador.huésped:
                solicitud = SistemaTraducción.ObtenerTraducción("huésped");
                break;
        }

        conexiónPendiente = conexión;
        txtDatosInvitación.Text = string.Format(SistemaTraducción.ObtenerTraducción("datosConexión"), conexión.IP, solicitud);
        popupInvitación.Visibility = Visibility.Visible;

        SistemaAnimación.AnimarElemento(animInvitación, 0.2f, true, Direcciones.arriba, TipoCurva.rápida, null);
    }

    public async void CerrarInvitación(bool conectarse)
    {
        if (animando)
            return;

        if (conectarse)
        {
            BloquearBotón(btnSí, true);
            BloquearBotón(btnNo, true);

            var resultado =  await SistemaRed.ConectarDispositivo(conexiónPendiente.IP, conexiónPendiente.ConectarComo, false);

            // Abre panel y muestra error
            if (string.IsNullOrEmpty(resultado))
                SistemaRed.IniciarPartida(true);
            else
            {
                txtErrorConexión.Text = SistemaTraducción.ObtenerTraducción(resultado);
                txtConexiónLAN.Text = conexiónPendiente.IP;
                CerrarInvitación(false);
            }
        }
        else
            SistemaRed.CerrarConexión(true);
    }

    private void CerrarPaneles()
    {
        if (animando)
            return;

        animando = true;
        txtErrorConexión.Text = string.Empty;

        if (gridLAN.Visibility == Visibility.Visible)
        {
            SistemaAnimación.AnimarElemento(animLAN, 0.2f, false, Direcciones.arriba, TipoCurva.rápida, () =>
            {
                animando = false;
                gridLAN.Visibility = Visibility.Hidden;
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

        SistemaRed.ForzarCierreConexión();
        Environment.Exit(0);
    }
}
