using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using Stride.Engine;
using Newtonsoft.Json;

namespace Terracota;
using static Constantes;

public class SistemaRed : AsyncScript
{
    private static SistemaRed instancia;
    private static TipoJugador tipoJugador;

    private static string IPLocalActual;
    private static string IPPublicaActual;
    private static string IPConectada;

    private static readonly HttpClient cliente = new HttpClient();

    // Envío de data
    private static UdpClient udp;
    private static IPEndPoint remoto;
    private static int puerto = 666;

    private bool conectado;

    public override async Task Execute()
    {
        instancia = this;
        ObtenerIPs();
                
        int cuadroActual = 0;
        int velocidadRed = int.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.velocidadRed));

        while (Game.IsRunning)
        {
            if (conectado && tipoJugador == TipoJugador.anfitrión)
            {
                cuadroActual++;
                if (cuadroActual >= velocidadRed)
                {
                    cuadroActual = 0;
                    ActualizarFísica();
                }
            }
            await Script.NextFrame();
        }
    }

    private void ActualizarFísica()
    {
        var data = "PENDIENE";
        EnviarData(EntradasRed.data, data);
    }

    private static async void ObtenerIPs()
    {
        // Local, última encontrada
        var IPSv4 = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(o => o.AddressFamily == AddressFamily.InterNetwork).ToArray();

        if (IPSv4.Length > 0)
        {
            // Usa la primera que comience con 192.168.
            var encontrada = false;
            foreach (var ip in IPSv4)
            {
                if(ip.ToString().Contains("192.168."))
                {
                    IPLocalActual = ip.ToString();
                    encontrada = true;
                    break;
                }
            }

            // Si no, la última
            if(!encontrada)
                IPLocalActual = IPSv4[^1].ToString();
        }
        else
            IPLocalActual = "NO IP";

        // Global
        string[] APIs = { "https://api.seeip.org/", "https://api.ipify.org/" };
        foreach (var ip in APIs)
        {
            try
            {
                var respuesta = await cliente.GetStringAsync(ip);
                IPPublicaActual = respuesta;
                break;
            }
            catch
            {
                IPPublicaActual = "NO IP";
            }
        }
    }

    public static string ObtenerIP(TipoConexión tipoConexión)
    {
        switch(tipoConexión)
        {
            case TipoConexión.LAN:
                return IPLocalActual;
            case TipoConexión.P2P:
                return IPPublicaActual;
            default:
                return string.Empty;
        }
    }

    public static void MarcarDispositivo(TipoJugador _tipoJugador)
    {
        tipoJugador = _tipoJugador;
    }

    public static void ConectarDispositivo(string ip, TipoConexión tipoConexión, TipoJugador conectarComo)
    {
        MarcarDispositivo(conectarComo);

        IPConectada = ip;
        udp = new UdpClient(puerto);
        remoto = new IPEndPoint(IPAddress.Parse(IPConectada), puerto);

        // Esperar respuesta
        switch (conectarComo)
        {
            case TipoJugador.anfitrión:
                EnviarData(EntradasRed.marcarComoAnfitrión);
                break;
            case TipoJugador.huesped:
                EnviarData(EntradasRed.marcarComoHuesped);
                break;
        }
        // en respuesta
        CambiarEscena();
    }

    private static void CambiarEscena()
    {
        SistemaEscenas.CambiarEscena(Escenas.remoto);
    }

    public static void EnviarData(EntradasRed entrada, string data = null)
    {
        var json = JsonConvert.SerializeObject(data);
        var bytes = Encoding.ASCII.GetBytes(json);

        udp.BeginReceive(new AsyncCallback(EnRecibir), udp);
    }

    private static async void EnRecibir(IAsyncResult resultado)
    {
        byte[] data = udp.EndReceive(resultado, ref remoto);
        udp.BeginReceive(new AsyncCallback(EnRecibir), udp);
    }

    // LAN
    public static async void BuscarLAN(string ipLocal, InterfazMenú interfazMenú)
    {
        Ping ping;
        IPAddress ip;
        PingReply respuesta;
        string nombreIP;
        string nombreHost;

        await Parallel.ForAsync(1, 255, async (i, loopState) =>
        {
            ping = new Ping();
            nombreIP = ipLocal + i.ToString();
            ip = IPAddress.Parse(nombreIP);
            respuesta = await ping.SendPingAsync(ip, 100);

            if (respuesta.Status == IPStatus.Success)
            {
                // Intenta obtener nombre de host
                try     { nombreHost = Dns.GetHostEntry(ip).HostName; }
                catch   { nombreHost = string.Empty; }

                // Se agrega a la lista
                if (nombreIP != IPLocalActual)
                    interfazMenú.AgregarHost(nombreIP, nombreHost);
            }
        });
        
        // Visual
        interfazMenú.MostrarCargando(false);
    }

    // P2P
    public static void EncontrarP2P()
    {

    }
}
