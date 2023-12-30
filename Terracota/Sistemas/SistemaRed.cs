using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
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
    private static string IPPúblicaActual;
    private static string IPConectada;

    // GLobal
    private static HttpClient cliente = new HttpClient();

    // UDP P2P
    private static UdpClient udp;
    private static IPEndPoint remoto;

    private static bool conectado;
    private static int velocidadRed;
    private static int puerto;

    // Partida remota
    private static SistemaRed instancia;
    private static ControladorPartidaRemota controlador;

    public override async Task Execute()
    {
        instancia = this;
        int cuadroActual = 0;

        ObtenerIPs();
        ActualizarConfiguración();

        while (Game.IsRunning)
        {
            if (conectado && tipoJugador == TipoJugador.anfitrión)
            {
                cuadroActual++;
                if (cuadroActual >= velocidadRed)
                {
                    cuadroActual = 0;
                    await ActualizarFísica();
                }
            }

            await RecibirData();

            await Script.NextFrame();
        }
    }

    private async Task ActualizarFísica()
    {
        var data = controlador.ObtenerFísicas();
        await EnviarData(EntradasRed.físicas, data);
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
                IPPúblicaActual = respuesta;
                break;
            }
            catch
            {
                IPPúblicaActual = "NO IP";
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
                return IPPúblicaActual;
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

    public static async Task<string> ConectarDispositivo(string ip, TipoConexión tipoConexión, TipoJugador conectarComo, bool iniciar)
    {
        try
        {
            var correcto = false;

            // Conexión
            if (udp != null)
                udp.Close();

            udp = new UdpClient(puerto);
            remoto = new IPEndPoint(IPAddress.Parse(ip), puerto);
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            
            // Solo anfitrión
            //if(conectarComo == TipoJugador.anfitrión)
            //    udp.Client.Bind(remoto);

            udp.Connect(ip, puerto);

            if (iniciar)
            {
                var miIP = string.Empty;
                switch (tipoConexión)
                {
                    case TipoConexión.LAN:
                        miIP = IPLocalActual;
                        break;
                    case TipoConexión.P2P:
                        miIP = IPPúblicaActual;
                        break;
                }

                var conectarRemotoComo = TipoJugador.nada;
                switch (conectarComo)
                {
                    case TipoJugador.anfitrión:
                        conectarRemotoComo = TipoJugador.huesped;
                        break;
                    case TipoJugador.huesped:
                        conectarRemotoComo = TipoJugador.anfitrión;
                        break;
                }

                var data = new Conexión
                {
                    IP = miIP,
                    TipoConexión = tipoConexión,
                    ConectarComo = conectarRemotoComo
                };
                correcto = await EnviarData(EntradasRed.conexión, data);
            }

            if (correcto || !iniciar)
            {
                MarcarDispositivo(conectarComo);
                IPConectada = ip;

                CambiarEscena();

                // Obtención controlador
                controlador = instancia.SceneSystem.SceneInstance.RootScene.Children[0].Entities.Where(o => o.Get<ControladorPartidaRemota>() != null).FirstOrDefault().Get<ControladorPartidaRemota>();
                return string.Empty;
            }
            else
                return "error udp";
        }
        catch (Exception e)
        {
            // PENDIENTE: Controlar y traducir errores
            return e.Message;
        }
    }

    private static void CambiarEscena()
    {
        SistemaEscenas.CambiarEscena(Escenas.remoto);
    }

    public static void Conectar()
    {
        conectado = true;
    }

    public static TipoJugador ObtenerTipoJugador()
    {
        return tipoJugador;
    }

    public static async Task<bool> EnviarData(EntradasRed entrada, dynamic data = null)
    {
        // Serializa data
        var json = string.Empty;
        if(data != null)
            json = JsonConvert.SerializeObject(data);

        var diccionario = new Dictionary<int, string>()
        {
            { (int)entrada, json }
        };

        // Agrega encabezado
        var dataFinal = JsonConvert.SerializeObject(diccionario);
        var buffer = Encoding.ASCII.GetBytes(dataFinal);

        try
        {
            await udp.SendAsync(buffer);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static async Task RecibirData()
    {
        string buffer = string.Empty;
        try
        {
            var resultado = await udp.ReceiveAsync();
            buffer = Encoding.ASCII.GetString(resultado.Buffer);
        }
        catch
        {
            return;
        }

        if(string.IsNullOrEmpty(buffer))
            return;

        var data = JsonConvert.DeserializeObject<Dictionary<int, string>>(buffer);
        var entrada = (EntradasRed)data.Keys.Single();

        switch(entrada)
        {
            case EntradasRed.conexión:
                var conexión = JsonConvert.DeserializeObject<Conexión>(data.Values.Single());
                await ConectarDispositivo(conexión.IP, conexión.TipoConexión, conexión.ConectarComo, false);
                break;
            case EntradasRed.turnoAnfitrión:
                controlador.CambiarTurno(TipoJugador.anfitrión);
                break;
            case EntradasRed.turnoHuesped:
                controlador.CambiarTurno(TipoJugador.huesped);
                break;
            case EntradasRed.cargarFortaleza:
                var fortaleza = JsonConvert.DeserializeObject<Fortaleza>(data.Values.Single());
                switch(tipoJugador)
                {
                    case TipoJugador.anfitrión:
                        controlador.CargarFortaleza(fortaleza, TipoJugador.huesped);
                        break;
                    case TipoJugador.huesped:
                        controlador.CargarFortaleza(fortaleza, TipoJugador.anfitrión);
                        break;
                }
                break;
            case EntradasRed.pausa:
                switch (tipoJugador)
                {
                    case TipoJugador.anfitrión:
                        controlador.Pausar(TipoJugador.huesped);
                        break;
                    case TipoJugador.huesped:
                        controlador.Pausar(TipoJugador.anfitrión);
                        break;
                }
                break;
            case EntradasRed.disparo:
                switch (tipoJugador)
                {
                    case TipoJugador.anfitrión:
                        controlador.ActivarDisparo(TipoJugador.huesped);
                        break;
                    case TipoJugador.huesped:
                        controlador.ActivarDisparo(TipoJugador.anfitrión);
                        break;
                }
                break;
            case EntradasRed.texto:
                var texto = JsonConvert.DeserializeObject<Texto>(data.Values.Single());
                controlador.ActualizarTexto(texto);
                break;
            case EntradasRed.físicas:
                var físicas = JsonConvert.DeserializeObject<Físicas>(data.Values.Single());
                controlador.ActualizarFísicas(físicas);
                break;
        }
    }

    // LAN
    public static async void BuscarLAN(string ipLocal, InterfazMenú interfazMenú)
    {
        await Parallel.ForAsync(1, 255, async (i, loopState) =>
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

    public static bool ActualizarConfiguración()
    {
        if (conectado)
            return false;

        velocidadRed = int.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.velocidadRed));
        puerto = int.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.puertoRed));

        try
        {
            if(udp != null)
                udp.Close();

            udp = new UdpClient(puerto);
            remoto = new IPEndPoint(IPAddress.Any, puerto);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
