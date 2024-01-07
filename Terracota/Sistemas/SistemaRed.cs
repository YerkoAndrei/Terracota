using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using Stride.Engine;
using Newtonsoft.Json;

namespace Terracota;
using static Constantes;
using static Terracota.Constantes;

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
    private static bool jugando;
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

        // PENDIENTE: cambiar a Hz?
        while (Game.IsRunning)
        {
            if (conectado && jugando && tipoJugador == TipoJugador.anfitrión)
            {
                cuadroActual++;
                if (cuadroActual >= velocidadRed)
                {
                    cuadroActual = 0;
                    await ActualizarFísica();
                }
            }
            await Script.NextFrame();
        }

        while (true)
        {
            await RecibirData();
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

            // Solo anfitrión (automático)
            //if(conectarComo == TipoJugador.anfitrión)
            //    udp.Client.Bind(remoto);

            udp.Connect(ip, puerto);

            if (iniciar)
            {
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

                SistemaEscenas.CambiarEscena(Escenas.remoto);
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

    private static async Task RecibirData()
    {
        string buffer = string.Empty;
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
        var entrada = (EntradasRed)data.Keys.Single();

        switch(entrada)
        {
            case EntradasRed.conexión:
                var conexión = JsonConvert.DeserializeObject<Conexión>(data.Values.Single());
                MostrarInvitación(conexión);
                break;
            case EntradasRed.cerrar:
                CerrarConexión();
                break;
            case EntradasRed.anfitriónListo:
                controlador.RevisarJugadoresListos(TipoJugador.anfitrión);
                break;
            case EntradasRed.huespedListo:
                controlador.RevisarJugadoresListos(TipoJugador.huesped);
                break;
            case EntradasRed.comenzarRuleta:
                var toques = int.Parse(data.Values.Single());
                ComenzarRuleta(toques);
                break;
            case EntradasRed.finalizarRuleta:
                var ganaAnfitrión = bool.Parse(data.Values.Single());
                FinalizarRuleta(ganaAnfitrión);
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

    private async Task ActualizarFísica()
    {
        var data = controlador.ObtenerFísicas();
        await EnviarData(EntradasRed.físicas, data);
    }

    private static void MostrarInvitación(Conexión conexión)
    {
        // Obtención interfaz
        var interfaz = instancia.SceneSystem.SceneInstance.RootScene.Children[0].Entities.Where(o => o.Get<InterfazMenú>() != null).FirstOrDefault().Get<InterfazMenú>();
        interfaz.MostrarInvitación(conexión);
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

    private static void CerrarConexión()
    {
        ActualizarConfiguración();
        MarcarDispositivo(TipoJugador.nada);
        IPConectada = string.Empty;

        SistemaEscenas.CambiarEscena(Escenas.menú);
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

    public static void ActivarActualizaciónFísicas(bool activar)
    {
        jugando = activar;
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
