using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Stride.Engine;
using Newtonsoft.Json;

namespace Terracota;

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
    }

    public static bool GuardarFortaleza(ElementoCreación[] bloques, int ranura, string miniatura)
    {
        var fortalezas = CargarFortalezas();

        // Crea nueva fortaleza
        var nuevaFortaleza = new Fortaleza();
        nuevaFortaleza.bloques = new List<Bloque>();
        nuevaFortaleza.ranura = ranura;
        nuevaFortaleza.miniatura = miniatura;
        for (int i = 0; i < bloques.Length; i++)
        {
            var bloque = new Bloque(bloques[i].ObtenerTipo(), bloques[i].ObtenerPosición(), bloques[i].ObtenerRotación());
            nuevaFortaleza.bloques.Add(bloque);
        }

        // Elimina ranura si existe
        var fortalezaEnRanura = fortalezas.Where(o => o.ranura == ranura).FirstOrDefault();
        if (fortalezaEnRanura != null)
            fortalezas.Remove(fortalezaEnRanura);

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

    public static Fortaleza ObtenerFortaleza(int ranura)
    {
        var fortalezas = CargarFortalezas();
        return fortalezas.Where(o => o.ranura == ranura).FirstOrDefault();
    }

    public static List<Fortaleza> CargarFortalezas()
    {
        if (!Directory.Exists(carpetaPersistente))
            Directory.CreateDirectory(carpetaPersistente);

        var fortalezas = new List<Fortaleza>();

        // Lee archivo
        if (File.Exists(rutaFortalezas))
        {
            var archivo = File.ReadAllText(rutaFortalezas);
            var desencriptado = DesEncriptar(archivo);
            fortalezas = JsonConvert.DeserializeObject<List<Fortaleza>>(desencriptado);
        }
        return fortalezas;
    }

    public static void GuardarConfiguración(List<string> nuevasConfiguraciones)
    {
        // Sobreescribe archivo
        var json = JsonConvert.SerializeObject(nuevasConfiguraciones);
        var encriptado = DesEncriptar(json);
        File.WriteAllText(rutaFortalezas, encriptado);
    }

    public static List<string> ObtenerConfiguración()
    {
        if (!Directory.Exists(carpetaPersistente))
            Directory.CreateDirectory(carpetaPersistente);

        var configuraciones = new List<string>();

        // Lee archivo
        if (File.Exists(rutaConfiguración))
        {
            var archivo = File.ReadAllText(rutaConfiguración);
            var desencriptado = DesEncriptar(archivo);
            configuraciones = JsonConvert.DeserializeObject<List<string>>(desencriptado);
        }
        return configuraciones;
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
