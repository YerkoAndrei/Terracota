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
    private static TipoJugador tipoJugador;

    private static string IPLocalActual;
    private static string IPPublicaActual;
    private static string IPConectada;

    private static readonly HttpClient cliente = new HttpClient();

    // Envío de data
    private static UdpClient udp;
    private static IPEndPoint remoto;
    private static int puerto = 666;

    private static bool conectado;

    public override async Task Execute()
    {
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
            foreach (var ip in IPSv4)
            {
                if(ip.ToString().Contains("192.168."))
                {
                    IPLocalActual = ip.ToString();
                    break;
                }
            }

            // Si no, la última
            if(string.IsNullOrEmpty(IPLocalActual))
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

    public static string ObtenerNombreHost()
    {
        return Dns.GetHostName();
    }

    public static void MarcarDispositivo(TipoJugador _tipoJugador)
    {
        tipoJugador = _tipoJugador;
    }

    public static string ConectarDispositivo(string ip, TipoConexión tipoConexión, TipoJugador conectarComo)
    {
        MarcarDispositivo(conectarComo);

        IPConectada = ip;
        udp = new UdpClient(puerto);
        remoto = new IPEndPoint(IPAddress.Parse(IPConectada), puerto);
        var marcarComo = EntradasRed.nada;

        switch (conectarComo)
        {
            case TipoJugador.anfitrión:
                marcarComo = EntradasRed.marcarComoHuesped;
                break;
            case TipoJugador.huesped:
                marcarComo = EntradasRed.marcarComoAnfitrión;
                break;
        }

        try
        {
            // Esperar respuesta
            EnviarData(marcarComo);
            // en respuesta

            CambiarEscena();
        }
        catch (Exception e)
        {
            // Verificar y/o traducir errores
            return e.Message;
        }

        return string.Empty;
    }

    private static void CambiarEscena()
    {
        SistemaEscenas.CambiarEscena(Escenas.remoto);
    }

    public static void Conectar()
    {
        conectado = true;
    }

    public static void EnviarData(EntradasRed entrada, string data = null)
    {
        var json = JsonConvert.SerializeObject(data);
        var bytes = Encoding.ASCII.GetBytes(json);

        //udp.BeginSend();
    }

    private static void RecibirData()
    {
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
        await Parallel.ForAsync(2, 255, async (i, loopState) =>
        {
            var ping = new Ping();
            var nombreIP = ipLocal + i.ToString();
            var nombreHost = string.Empty;
            var ip = IPAddress.Parse(nombreIP);
            var respuesta = await ping.SendPingAsync(ip, 100);

            if (respuesta.Status == IPStatus.Success)
            {
                try
                {
                    // Intenta obtener nombre de host
                    var buscarNombre = await Dns.GetHostEntryAsync(ip);
                    nombreHost = buscarNombre.HostName;
                }
                catch { }

                // Se agrega a la lista
                if (nombreIP != IPLocalActual)
                    interfazMenú.AgregarHost(nombreIP, nombreHost);
            }
        });
        
        // Visual
        interfazMenú.MostrarCargando(false);
    }
}
