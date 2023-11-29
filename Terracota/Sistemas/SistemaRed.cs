using System;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using Stride.Engine;
using System.Net.Http;

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
    private static UdpClient servidor;
    private static IPEndPoint remoto;
    private static int puertoServidor = 666;
    private static int puertoRemoto = 666;

    public override async Task Execute()
    {/*
        ObtenerIP();

        Log.Warning("Comenzando");
        EncontrarLAN();

        while (Game.IsRunning)
        {
            // Do stuff every new frame
            await Script.NextFrame();
        }*/
    }

    public static async void ObtenerIP()
    {
        // Local
        IPLocalActual = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();

        // Global
        try
        {
            var respuesta = await cliente.GetStringAsync("https://api.ipify.org/");
            IPPublicaActual = respuesta;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error");
        }
    }


    // LAN
    public static void MarcarAnfitrión()
    {
        tipoJugador = TipoJugador.anfitrión;
    }

    public static bool PreguntarAnfitrión()
    {
        return (tipoJugador == TipoJugador.anfitrión);
    }

    public static Dictionary<string, string> EncontrarLAN()
    {
        Ping ping;
        IPAddress ip;
        PingReply respuesta;
        string nombre;

        string ipLocal = "192.168.1.";
        var listaEncontrados = new Dictionary<string, string>();

        Parallel.For(0, 254, (i, loopState) =>
        {
            ping = new Ping();
            respuesta = ping.Send(ipLocal + i.ToString());

            if(respuesta.Status == IPStatus.Success)
            {
                try
                {
                    ip = IPAddress.Parse(ipLocal + i.ToString());
                    nombre = Dns.GetHostEntry(ip).HostName;
                    listaEncontrados.Add(ipLocal + i.ToString(), nombre);
                }
                catch (Exception e)
                {
                    listaEncontrados.Add(ipLocal + i.ToString(), i.ToString());
                }
            }
        });
        return listaEncontrados;
    }

    // Anfitrión
    public static string ConectarHuesped(bool local, string ip)
    {
        IPConectada = ip;
        if (local)
            return IPLocalActual;
        else
            return IPPublicaActual;
    }

    public static void EnviarData()
    {
        servidor = new UdpClient(puertoServidor);
        remoto = new IPEndPoint(IPAddress.Parse(IPConectada), puertoRemoto);
        servidor.BeginReceive(new AsyncCallback(EnRecibir), servidor);
    }

    private static async void EnRecibir(IAsyncResult resultado)
    {
        byte[] data = servidor.EndReceive(resultado, ref remoto);
        servidor.BeginReceive(new AsyncCallback(EnRecibir), servidor);
    }

    // Huesped
    public static async void ConectarConAnfitrión(bool local, string ipAnfitrión)
    {
        // PENDIENTE
        var respuesta = await cliente.GetStringAsync(ipAnfitrión);
        IPConectada = respuesta;
    }

    public static void EnviarEntrada()
    {

    }
}
