using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Stride.Engine;
using Newtonsoft.Json;

namespace Terracota;
using static Sistema;
using static Constantes;

public class SistemaMemoria : StartupScript
{
    private static string carpetaPersistente = "C:\\Users\\{0}\\AppData\\LocalLow\\{1}\\{2}";
    private static string desarrollador = "YerkoAndrei";
    private static string producto = "Terracota";

    private static string archivoFortalezas = "Fortalezas";
    private static string archivoConfiguración = "Configuración";

    private static string rutaFortalezas;
    private static string rutaConfiguración;

    public override void Start()
    {
        carpetaPersistente = string.Format(carpetaPersistente, Environment.UserName, desarrollador, producto);
        rutaFortalezas = Path.Combine(carpetaPersistente, archivoFortalezas);
        rutaConfiguración = Path.Combine(carpetaPersistente, archivoConfiguración);

        EstablecerConfiguraciónPredeterminada();
    }

    // Fortalezas
    public static List<Fortaleza> CargarFortalezas(bool crearVacía)
    {
        if (!Directory.Exists(carpetaPersistente))
            Directory.CreateDirectory(carpetaPersistente);

        // Agrega fortaleza vacía
        var fortalezas = new List<Fortaleza>();
        if (crearVacía)
            fortalezas.Add(GenerarFortalezaVacía());

        // Lee archivo
        if (File.Exists(rutaFortalezas))
        {
            var archivo = File.ReadAllText(rutaFortalezas);
            var desencriptado = DesEncriptar(archivo);
            fortalezas.AddRange(JsonConvert.DeserializeObject<List<Fortaleza>>(desencriptado));

            // Orden
            fortalezas = fortalezas.OrderBy(o => o.Fecha).ToList();
        }
        return fortalezas;
    }

    public static bool GuardarFortaleza(bool sobreescribir, ElementoCreación[] bloques, string nombre, string miniatura)
    {
        var fortalezas = CargarFortalezas(false);

        // Sobreescribe o niega escritura
        var fortalezaEnRanura = fortalezas.Where(o => o.Nombre == nombre).FirstOrDefault();
        if (fortalezaEnRanura != null)
        {
            if (sobreescribir)
                fortalezas.Remove(fortalezaEnRanura);
            else
                return false;
        }

        // Crea nueva fortaleza
        var nuevaFortaleza = new Fortaleza();
        nuevaFortaleza.Nombre = nombre;
        nuevaFortaleza.Fecha = FormatearFechaEstándar(DateTime.Now);
        nuevaFortaleza.Bloques = new List<Bloque>();
        for (int i = 0; i < bloques.Length; i++)
        {
            var bloque = new Bloque(bloques[i].ObtenerTipo(), bloques[i].ObtenerPosiciónRelativa(), bloques[i].ObtenerRotación());
            nuevaFortaleza.Bloques.Add(bloque);
        }

        // Agrega nueva fortaleza
        fortalezas.Add(nuevaFortaleza);

        // Guarda archivo
        try
        {
            var json = JsonConvert.SerializeObject(fortalezas);
            var encriptado = DesEncriptar(json);
            File.WriteAllText(rutaFortalezas, encriptado);
            return true;
        }
        catch { return false; }
    }

    public static Fortaleza ObtenerFortaleza(string nombre)
    {
        var fortalezas = CargarFortalezas(true);
        return fortalezas.Where(o => o.Nombre == nombre).FirstOrDefault();
    }

    public static bool EliminarFortaleza(string nombre)
    {
        var fortalezas = CargarFortalezas(false);
        var fortaleza = fortalezas.Where(o => o.Nombre == nombre).FirstOrDefault();

        if (fortaleza == null)
            return false;

        fortalezas.Remove(fortaleza);

        // Guarda archivo
        try
        {
            var json = JsonConvert.SerializeObject(fortalezas);
            var encriptado = DesEncriptar(json);
            File.WriteAllText(rutaFortalezas, encriptado);
            return true;
        }
        catch { return false; }
    }

    // Configuración
    private static void EstablecerConfiguraciónPredeterminada()
    {
        if (File.Exists(rutaConfiguración))
            return;

        // Valores predeterminados
        var diccionario = new Dictionary<string, string>
        {
            { Configuraciones.idioma.ToString(),            Idiomas.sistema.ToString() },
            { Configuraciones.gráficos.ToString(),          NivelesConfiguración.alto.ToString() },
            { Configuraciones.sombras.ToString(),           NivelesConfiguración.alto.ToString() },
            { Configuraciones.volumenGeneral.ToString(),    "1" },
            { Configuraciones.volumenMúsica.ToString(),     "0.5" },
            { Configuraciones.volumenEfectos.ToString(),    "0.5" },
            { Configuraciones.velocidadRed.ToString(),      "60" },
            { Configuraciones.puertoRed.ToString(),         "666" },
            { Configuraciones.pantallaCompleta.ToString(),  true.ToString() },
            { Configuraciones.resolución.ToString(),        "1920x1080" }
        };

        // Guarda archivo
        var json = JsonConvert.SerializeObject(diccionario);
        var encriptado = DesEncriptar(json);
        File.WriteAllText(rutaConfiguración, encriptado);
    }

    public static void GuardarConfiguración(Configuraciones configuración, string valor)
    {
        var configuraciones = ObtenerConfiguraciones();

        // Nuevo o remplazo
        configuraciones[configuración.ToString()] = valor;

        // Sobreescribe archivo
        var json = JsonConvert.SerializeObject(configuraciones);
        var encriptado = DesEncriptar(json);
        File.WriteAllText(rutaConfiguración, encriptado);
    }

    private static Dictionary<string, string> ObtenerConfiguraciones()
    {
        if (!Directory.Exists(carpetaPersistente))
            Directory.CreateDirectory(carpetaPersistente);

        var configuraciones = new Dictionary<string, string>();

        // Lee archivo
        if (File.Exists(rutaConfiguración))
        {
            var archivo = File.ReadAllText(rutaConfiguración);
            var desencriptado = DesEncriptar(archivo);
            configuraciones = JsonConvert.DeserializeObject<Dictionary<string, string>>(desencriptado);
        }
        return configuraciones;
    }

    public static string ObtenerConfiguración(Configuraciones llave)
    {
        var configuraciones = ObtenerConfiguraciones();
        return configuraciones[llave.ToString()];
    }

    // XOR
    private static string DesEncriptar(string texto)
    {
        var entrada = new StringBuilder(texto);
        var salida = new StringBuilder(texto.Length);
        char c;

        for (int i = 0; i < texto.Length; i++)
        {
            c = entrada[i];
            c = (char)(c ^ 08021996);
            salida.Append(c);
        }
        return salida.ToString();
    }
}
