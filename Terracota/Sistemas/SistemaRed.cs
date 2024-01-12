using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Timers;
using Stride.Engine;
using Newtonsoft.Json;
namespace Terracota;
using static Constantes;

public class SistemaRed : StartupScript
{
    private static TipoJugador tipoJugador;

    private static string IPLocalActual;
    private static string IPPúblicaActual;
    private static string IPConectada;

    // UDP P2P
    private static UdpClient udp;
    private static IPEndPoint remoto;

    private static bool conectado;
    private static bool jugando;
    private static int velocidadRed;
    private static int puerto;

    // Partida remota
    private static SistemaRed instancia;
    private static ControladorPartidaRemota controlador;

    private static Timer temporizador;

    public override void Start()
    {
        instancia = this;

        ObtenerIPs();
        ActualizarConfiguración();

        EscucharUDP();
    }

    private static void CrearTemporizador()
    {
        temporizador = new Timer(1000 / velocidadRed);
        temporizador.Elapsed += ActualizarRed;
        temporizador.AutoReset = true;
        temporizador.Enabled = true;
        temporizador.Start();
    }

    private static async void ActualizarRed(object sender, ElapsedEventArgs e)
    {
        if (conectado && jugando)
        {
            if(tipoJugador == TipoJugador.anfitrión)
                await ActualizarFísica();
            else if (tipoJugador == TipoJugador.huesped)
                await ActualizarCañón();
        }
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
        var cliente = new HttpClient();
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

    public static async Task<string> ConectarDispositivo(string ip, TipoConexión tipoConexión, TipoJugador conectarLocalComo, bool contactar)
    {
        try
        {
            // Conexión
            if (udp != null)
                udp.Close();

            udp = new UdpClient(puerto);
            remoto = new IPEndPoint(IPAddress.Parse(ip), puerto);
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            udp.Connect(ip, puerto);

            var correcto = false;
            if (contactar)
            {
                // Datos conexión
                var miIP = string.Empty;
                switch (tipoConexión)
                {
                    case TipoConexión.local:
                        miIP = IPLocalActual;
                        break;
                    case TipoConexión.global:
                        miIP = IPPúblicaActual;
                        break;
                }

                // Contrario en remoto
                var conectarRemotoComo = conectarLocalComo;
                switch (conectarLocalComo)
                {
                    case TipoJugador.anfitrión:
                        conectarRemotoComo = TipoJugador.huesped;
                        break;
                    case TipoJugador.huesped:
                        conectarRemotoComo = TipoJugador.anfitrión;
                        break;
                }

                var conexión = new Conexión
                {
                    IP = miIP,
                    TipoConexión = tipoConexión,
                    ConectarComo = conectarRemotoComo
                };

                correcto = await EnviarData(DataRed.conectar, conexión);
            }

            if (correcto || !contactar)
            {
                MarcarDispositivo(conectarLocalComo);
                IPConectada = ip;
                return string.Empty;
            }
            else
                return "error";
        }
        catch (Exception e)
        {
            // PENDIENTE: Controlar y traducir errores
            return e.Message;
        }
    }

    public static async Task<bool> EnviarData(DataRed entrada, dynamic data = null)
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
        var buffer = Encoding.Unicode.GetBytes(dataFinal);

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

    private static async void EscucharUDP()
    {
        while (true)
        {
            await RecibirData();
        }
    }

    private static async Task RecibirData()
    {
        string buffer;
        try
        {
            var resultado = await udp.ReceiveAsync();
            buffer = Encoding.Unicode.GetString(resultado.Buffer);
        }
        catch
        {
            return;
        }

        if(string.IsNullOrEmpty(buffer))
            return;

        var data = JsonConvert.DeserializeObject<Dictionary<int, string>>(buffer);
        var entrada = (DataRed)data.Keys.Single();

        switch(entrada)
        {
            case DataRed.conectar:
                var conexión = JsonConvert.DeserializeObject<Conexión>(data.Values.Single());
                MostrarInvitación(conexión);
                break;
            case DataRed.iniciarPartida:
                IniciarPartida(false);
                break;
            case DataRed.cerrar:
                CerrarConexión(false);
                break;
            case DataRed.anfitriónListo:
                controlador.RevisarJugadoresListos(TipoJugador.anfitrión);
                break;
            case DataRed.huespedListo:
                controlador.RevisarJugadoresListos(TipoJugador.huesped);
                break;
            case DataRed.comenzarRuleta:
                var toques = int.Parse(data.Values.Single());
                ComenzarRuleta(toques);
                break;
            case DataRed.finalizarRuleta:
                var ganaAnfitrión = bool.Parse(data.Values.Single());
                FinalizarRuleta(ganaAnfitrión);
                break;
            case DataRed.turnoAnfitrión:
                controlador.ActualizarTurno(TipoJugador.anfitrión);
                break;
            case DataRed.turnoHuesped:
                controlador.ActualizarTurno(TipoJugador.huesped);
                break;
            case DataRed.cargarFortaleza:
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
            case DataRed.pausa:
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
            case DataRed.disparo:
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
            case DataRed.texto:
                var texto = JsonConvert.DeserializeObject<Texto>(data.Values.Single());
                controlador.ActualizarTexto(texto);
                break;
            case DataRed.físicas:
                var físicas = JsonConvert.DeserializeObject<Físicas>(data.Values.Single());
                controlador.ActualizarFísicas(físicas);
                break;
            case DataRed.cañón:
                var cañón = JsonConvert.DeserializeObject<RotaciónCañón>(data.Values.Single());
                switch (tipoJugador)
                {
                    case TipoJugador.anfitrión:
                        controlador.ActualizarCañón(cañón, TipoJugador.huesped);
                        break;
                    case TipoJugador.huesped:
                        controlador.ActualizarCañón(cañón, TipoJugador.anfitrión);
                        break;
                }
                break;
        }
    }

    private static async Task ActualizarFísica()
    {
        var data = controlador.ObtenerFísicas();
        await EnviarData(DataRed.físicas, data);
    }

    private static async Task ActualizarCañón()
    {
        var data = controlador.ObtenerRotaciónCañón();
        await EnviarData(DataRed.cañón, data);
    }

    private static void MostrarInvitación(Conexión conexión)
    {
        // Obtención interfaz
        var interfaz = instancia.SceneSystem.SceneInstance.RootScene.Children[0].Entities.Where(o => o.Get<InterfazMenú>() != null).FirstOrDefault().Get<InterfazMenú>();
        interfaz.MostrarInvitación(conexión);
    }

    public static async void IniciarPartida(bool contactar)
    {
        if (contactar)
            await EnviarData(DataRed.iniciarPartida);

        SistemaEscenas.CambiarEscena(Escenas.remoto);
    }

    public static async void CerrarConexión(bool contactar)
    {
        if (contactar)
            await EnviarData(DataRed.cerrar);

        ActualizarConfiguración();
        MarcarDispositivo(TipoJugador.nada);
        IPConectada = string.Empty;

        SistemaEscenas.CambiarEscena(Escenas.menú);
    }

    private static void ComenzarRuleta(int toques)
    {
        // Obtención interfaz
        var elección = instancia.SceneSystem.SceneInstance.RootScene.Children[0].Entities.Where(o => o.Get<InterfazElecciónRemota>() != null).FirstOrDefault().Get<InterfazElecciónRemota>();
        elección.ComenzarRuleta(toques);
    }

    private static void FinalizarRuleta(bool ganaAnfitrión)
    {
        // Obtención interfaz
        var elección = instancia.SceneSystem.SceneInstance.RootScene.Children[0].Entities.Where(o => o.Get<InterfazElecciónRemota>() != null).FirstOrDefault().Get<InterfazElecciónRemota>();
        elección.ComenzarPartida(ganaAnfitrión);
    }
        
    public static void MarcarDispositivo(TipoJugador _tipoJugador)
    {
        tipoJugador = _tipoJugador;
    }

    public static void Conectar()
    {
        // Obtención controlador
        controlador = instancia.SceneSystem.SceneInstance.RootScene.Children[0].Entities.Where(o => o.Get<ControladorPartidaRemota>() != null).FirstOrDefault().Get<ControladorPartidaRemota>();
        conectado = true;
    }

    public static void ActivarActualizaciónRed(bool activar)
    {
        jugando = activar;
        CrearTemporizador();
    }

    public static string ObtenerIP(TipoConexión tipoConexión)
    {
        switch (tipoConexión)
        {
            case TipoConexión.local:
                return IPLocalActual;
            case TipoConexión.global:
                return IPPúblicaActual;
            default:
                return string.Empty;
        }
    }

    public static TipoJugador ObtenerTipoJugador()
    {
        return tipoJugador;
    }

    public static bool ObtenerJugando()
    {
        return (conectado && jugando);
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
