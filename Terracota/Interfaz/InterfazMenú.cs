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
using static Sistema;
using static Constantes;

public class InterfazMenú : StartupScript
{
    public Entity rotador;
    public UILibrary prefabHost;

    public Color colorCargaVacía;
    public Color colorCargaActiva;

    private static Quaternion últimaRotación = Quaternion.Identity;

    private UIElement página;
    private Grid Opciones;
    private Grid animOpciones;
    private Grid gridLAN;
    private Grid gridP2P;

    private Grid btnBuscar;
    private Grid gridCargando;
    private ImageElement imgCargando;
    private UniformGrid padreHosts;
    private EditText txtBaseIPActual;

    private bool cargando;
    private bool animando;

    public override void Start()
    {
        página = Entity.Get<UIComponent>().Page.RootElement;
        Opciones = página.FindVisualChildOfType<Grid>("Opciones");
        animOpciones = página.FindVisualChildOfType<Grid>("animOpciones");

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnLocal"), EnClicLocal);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnLAN"), EnClicLAN);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnP2P"), EnClicP2P);

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnCrear"), EnClicCrear);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnOpciones"), EnClicOpciones);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSalir"), EnClicSalir);

        BloquearBotón(página.FindVisualChildOfType<Grid>("btnP2P"), true);

        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("btnCréditos"), EnClicCréditos);

        // Conexiones
        gridLAN = página.FindVisualChildOfType<Grid>("ConexiónLAN");
        gridP2P = página.FindVisualChildOfType<Grid>("ConexiónP2P");

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnLANVolver"), CerrarPaneles);
        //ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnP2PVolver"), CerrarPaneles);

        gridLAN.Visibility = Visibility.Hidden;
        //gridP2P.Visibility = Visibility.Hidden;

        // LAN
        página.FindVisualChildOfType<TextBlock>("txtIPActual").Text = SistemaRed.ObtenerIP(TipoConexión.LAN);
        página.FindVisualChildOfType<TextBlock>("txtNombreHost").Text = SistemaRed.ObtenerNombreHost(TipoConexión.LAN);
        padreHosts = página.FindVisualChildOfType<UniformGrid>("Hosts");

        gridCargando = página.FindVisualChildOfType<Grid>("Cargando");
        gridCargando.Visibility = Visibility.Hidden;
        imgCargando = página.FindVisualChildOfType<ImageElement>("imgCargando");

        btnBuscar = página.FindVisualChildOfType<Grid>("btnBuscarLAN");
        ConfigurarBotón(btnBuscar, EnClicBuscarLAN);

        txtBaseIPActual = página.FindVisualChildOfType<EditText>("txtBaseIPActual");
        txtBaseIPActual.TextChanged += VerificarFuente;

        var partesIP = SistemaRed.ObtenerIP(TipoConexión.LAN).Split('.');
        txtBaseIPActual.Text = partesIP[0] + "." + partesIP[1] + "." + partesIP[2] + ".";

        // P2P

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

        gridLAN.Visibility = Visibility.Visible;
    }

    private void EnClicBuscarLAN()
    {
        if (animando)
            return;

        // Limpieza
        padreHosts.Children.Clear();
        padreHosts.Rows = 0;
        padreHosts.Height = 0;

        MostrarCargando(true);
        SistemaRed.BuscarLAN(txtBaseIPActual.Text, this);
    }

    public void AgregarHost(string nombreIP, string nombreHost)
    {
        var nuevoHost = prefabHost.InstantiateElement<Grid>("Host");
        ConfigurarHostLAN(nuevoHost, nombreIP, nombreHost,
            () => { SistemaRed.ConectarDispositivo(nombreIP, TipoConexión.LAN, TipoJugador.anfitrión); },
            () => { SistemaRed.ConectarDispositivo(nombreIP, TipoConexión.LAN, TipoJugador.huesped); });
        
        padreHosts.Children.Add(nuevoHost);

        // Re ordena lista cada nuevo y al final
        // Son ingresados en paralelo y el índice puede repetirse
        OrdenarHosts();
    }

    private void OrdenarHosts()
    {
        padreHosts.Rows = padreHosts.Children.Count;
        padreHosts.Height = 0;

        for (int i = 0; i < padreHosts.Children.Count; i++)
        {
            padreHosts.Children[i].SetGridRow(i);
            padreHosts.Height += (padreHosts.Children[i].Height + 10);
        }
    }

    public void MostrarCargando(bool _cargando)
    {
        cargando = _cargando;

        if (cargando)
        {
            gridCargando.Visibility = Visibility.Visible;
            BloquearBotón(btnBuscar, true);
            AnimarCarga();
        }
        else
        {
            gridCargando.Visibility = Visibility.Hidden;
            BloquearBotón(btnBuscar, false);
            OrdenarHosts();
        }
    }

    private async void AnimarCarga()
    {
        int delay = 200;
        bool activo = false;

        while (cargando)
        {
            if (activo)
            {
                imgCargando.Color = colorCargaVacía;
                activo = false;
            }
            else 
            {
                imgCargando.Color = colorCargaActiva;
                activo = true;
            }
            await Task.Delay(delay);
        }
    }

    private void VerificarFuente(object sender, RoutedEventArgs args)
    {
        // Solo números y puntos
        string regex = @"[^0-9.]";
        txtBaseIPActual.Text = Regex.Replace(txtBaseIPActual.Text, regex, "");

        var fuente = SistemaTraducción.VerificarFuente(txtBaseIPActual.Text);

        if (txtBaseIPActual.Font != fuente)
            txtBaseIPActual.Font = fuente;
    }

    private void EnClicP2P()
    {
        if (animando)
            return;

        //gridP2P.Visibility = Visibility.Hidden;
    }

    private void CerrarPaneles()
    {
        if (animando)
            return;

        gridLAN.Visibility = Visibility.Hidden;
        //gridP2P.Visibility = Visibility.Hidden;
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
