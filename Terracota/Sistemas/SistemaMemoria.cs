﻿using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using Stride.Engine;

namespace Terracota;
using static Utilidades;
using static Constantes;

public class SistemaMemoria : StartupScript
{
    private static string carpetaPersistente = string.Empty;
    private static string desarrollador = "YerkoAndrei";
    private static string producto = "Terracota";

    private static string archivoFortalezas = "Fortalezas";
    private static string archivoConfiguración = "Configuración";

    private static string rutaFortalezas;
    private static string rutaConfiguración;

    public override void Start()
    {
        EstablecerRutas();
    }

    private static void EstablecerRutas()
    {
        carpetaPersistente = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), desarrollador, producto);
        rutaFortalezas = Path.Combine(carpetaPersistente, archivoFortalezas);
        rutaConfiguración = Path.Combine(carpetaPersistente, archivoConfiguración);
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
            fortalezas.AddRange(JsonSerializer.Deserialize<List<Fortaleza>>(desencriptado));

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
            var json = JsonSerializer.Serialize(fortalezas);
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
            var json = JsonSerializer.Serialize(fortalezas);
            var encriptado = DesEncriptar(json);
            File.WriteAllText(rutaFortalezas, encriptado);
            return true;
        }
        catch { return false; }
    }

    // Configuración
    public static void EstablecerConfiguraciónPredeterminada(int ancho, int alto)
    {
        EstablecerRutas();

        if (!Directory.Exists(carpetaPersistente))
            Directory.CreateDirectory(carpetaPersistente);

        if (File.Exists(rutaConfiguración))
            return;

        // Resolución original
        var resolución = "1280x720";
        if(ancho > 0 && alto > 0)
            resolución = ancho.ToString() + "x" + alto.ToString();

        // Valores predeterminados
        var diccionario = new Dictionary<string, string>
        {
            { Configuraciones.idioma.ToString(),            Idiomas.sistema.ToString() },
            { Configuraciones.gráficos.ToString(),          Calidades.alto.ToString() },
            { Configuraciones.sombras.ToString(),           Calidades.alto.ToString() },
            { Configuraciones.vSync.ToString(),             false.ToString() },
            { Configuraciones.volumenGeneral.ToString(),    "1" },
            { Configuraciones.volumenMúsica.ToString(),     "0.5" },
            { Configuraciones.volumenEfectos.ToString(),    "0.5" },
            { Configuraciones.velocidadRed.ToString(),      "60" },
            { Configuraciones.puertoRed.ToString(),         "28" },
            { Configuraciones.pantallaCompleta.ToString(),  true.ToString() },
            { Configuraciones.resolución.ToString(),        resolución }
        };

        // Guarda archivo
        var json = JsonSerializer.Serialize(diccionario);
        var encriptado = DesEncriptar(json);
        File.WriteAllText(rutaConfiguración, encriptado);
    }

    public static void GuardarConfiguración(Configuraciones configuración, string valor)
    {
        var configuraciones = ObtenerConfiguraciones();

        // Nuevo o remplazo
        configuraciones[configuración.ToString()] = valor;

        // Sobreescribe archivo
        var json = JsonSerializer.Serialize(configuraciones);
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
            configuraciones = JsonSerializer.Deserialize<Dictionary<string, string>>(desencriptado);
        }
        return configuraciones;
    }

    public static string ObtenerConfiguración(Configuraciones llave)
    {
        var configuraciones = ObtenerConfiguraciones();
        return configuraciones[llave.ToString()];
    }

    public static bool ObtenerExistenciaArchivo()
    {
        if (!Directory.Exists(carpetaPersistente))
            return false;

        return File.Exists(rutaConfiguración);
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
