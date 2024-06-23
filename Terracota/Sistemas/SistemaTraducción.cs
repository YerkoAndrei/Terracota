using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using Stride.Engine;
using Stride.Graphics;
using Stride.Core.Serialization;

namespace Terracota;
using static Constantes;

public class SistemaTraducción : StartupScript
{
    private static SistemaTraducción instancia;

    private static string jsonEspañol;
    private static string jsonInglés;

    private static Idiomas idioma;
    private static Dictionary<string, string> diccionario;

    public UrlReference textosEspañol;
    public UrlReference textosInglés;

    public SpriteFont fuentePrincipal;
    public SpriteFont fuenteRespaldo;

    public override void Start()
    {
        instancia = this;

        // Lee RawAsset como textos
        using (var stream = Content.OpenAsStream(textosEspañol))
        {
            using (var reader = new StreamReader(stream))
            {
                jsonEspañol = reader.ReadToEnd();
            }            
        }
        using (var stream = Content.OpenAsStream(textosInglés))
        {
            using (var reader = new StreamReader(stream))
            {
                jsonInglés = reader.ReadToEnd();
            }
        }

        // Usa guardada o idioma de sistema la primera vez
        var idiomaGuardado = (Idiomas)Enum.Parse(typeof(Idiomas), SistemaMemoria.ObtenerConfiguración(Configuraciones.idioma));
        if (idiomaGuardado == Idiomas.sistema)
        {
            // Revisa el idioma instalado. ISO 639-1
            var lenguajeSistema = System.Globalization.CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
            switch (lenguajeSistema)
            {
                default:
                case "es":
                    CambiarIdioma(Idiomas.español, false);
                    break;
                case "en":
                    CambiarIdioma(Idiomas.inglés, false);
                    break;
            }
        }
        else
            CambiarIdioma(idiomaGuardado, false);
    }

    public static SpriteFont VerificarFuente(string texto)
    {
        foreach(char c in texto)
        {
            if(!instancia.fuentePrincipal.IsCharPresent(c))
                return instancia.fuenteRespaldo;
        }
        return instancia.fuentePrincipal;
    }

    public static void CambiarIdioma(Idiomas nuevoIdioma, bool actualizar = true)
    {
        idioma = nuevoIdioma;
        SistemaMemoria.GuardarConfiguración(Configuraciones.idioma, idioma.ToString());

        switch (nuevoIdioma)
        {
            default:
            case Idiomas.español:
                diccionario = CrearDiccionario(jsonEspañol);
                break;
            case Idiomas.inglés:
                diccionario = CrearDiccionario(jsonInglés);
                break;
        }

        if(actualizar)
            ActualizarTextosEscena();
    }

    private static Dictionary<string, string> CrearDiccionario(string traducciones)
    {
        var diccionario = JsonSerializer.Deserialize<Dictionary<string, string>>(traducciones);
        return diccionario;
    }

    public static void ActualizarTextosEscena()
    {
        // Busca controladores de traducción en escena hija (SistemaEscena solo permite una escena hija al mismo tiempo)
        var traductores = instancia.SceneSystem.SceneInstance.RootScene.Children[0].Entities.Where(o => o.Get<ControladorTraducciones>() != null).ToArray();
        foreach (var traductor in traductores)
        {
            traductor.Get<ControladorTraducciones>().Traducir();
        }
    }

    public static string ObtenerTraducción(string código)
    {
        if (diccionario.ContainsKey(código))
            return diccionario[código];
        else
            return "¿?";
    }
}
