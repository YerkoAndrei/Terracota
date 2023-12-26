using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using Stride.Engine;

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

        if(IPSv4.Length > 0)
            IPLocalActual = IPSv4[^1].ToString();
        else
            IPLocalActual = "NO IP";

        // Global
        try
        {
            var respuesta = await cliente.GetStringAsync("https://api.ipify.org/");
            IPPublicaActual = respuesta;
        }
        catch (Exception e)
        {
            IPPublicaActual = "NO IP";
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
        udp.BeginReceive(new AsyncCallback(EnRecibir), udp);
    }

    private static async void EnRecibir(IAsyncResult resultado)
    {
        byte[] data = udp.EndReceive(resultado, ref remoto);
        udp.BeginReceive(new AsyncCallback(EnRecibir), udp);
    }

    // LAN
    public static async void BuscarLAN(string ipLocal)
    {
        var interfazMenú =  instancia.SceneSystem.SceneInstance.RootScene.Children[0].Entities.Where(o => o.Get<InterfazMenú>() != null).FirstOrDefault().Get<InterfazMenú>();
        
        Ping ping;
        IPAddress ip;
        PingReply respuesta;
        string nombre;
        int índice = 0;

        await Parallel.ForAsync(0, 254, async (i, loopState) =>
        {
            ping = new Ping();
            respuesta = await ping.SendPingAsync(ipLocal + i.ToString());

            if(respuesta.Status == IPStatus.Success)
            {
                try
                {
                    ip = IPAddress.Parse(ipLocal + i.ToString());
                    nombre = Dns.GetHostEntry(ip).HostName;
                    var nuevoHost = new Host
                    {
                        IP = ipLocal + i.ToString(),
                        Nombre = nombre
                    };

                    // Se agrega con nombre
                    if (nuevoHost.IP != IPLocalActual)
                    {
                        interfazMenú.AgregarHost(nuevoHost, índice);
                        índice++;
                    }
                }
                catch (Exception e)
                {
                    ip = IPAddress.Parse(ipLocal + i.ToString());
                    var nuevoHost = new Host
                    {
                        IP = ipLocal + i.ToString(),
                        Nombre = string.Empty
                    };

                    // Se agrega sin nombre
                    if (nuevoHost.IP != IPLocalActual)
                    {
                        interfazMenú.AgregarHost(nuevoHost, índice);
                        índice++;
                    }
                }
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
