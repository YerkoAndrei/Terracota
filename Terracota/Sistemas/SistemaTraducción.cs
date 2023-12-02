using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Stride.Engine;
using Stride.Graphics;
using Newtonsoft.Json;

namespace Terracota;
using static Constantes;

public class SistemaTraducción : StartupScript
{
    private static SistemaTraducción instancia;

    private static string textosEspañol;
    private static string textosInglés;

    private static Idiomas idioma;
    private static Dictionary<string, string> diccionario;

    public SpriteFont fuentePrincipal;
    public SpriteFont fuenteRespaldo;

    public override void Start()
    {
        instancia = this;

        // Lee textos en proyecto
        textosEspañol = new StreamReader("Assets/Textos/TextosEspañol.json", Encoding.UTF8).ReadToEnd();
        textosInglés = new StreamReader("Assets/Textos/TextosInglés.json", Encoding.UTF8).ReadToEnd();

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
                diccionario = CrearDiccionario(textosEspañol);
                break;
            case Idiomas.inglés:
                diccionario = CrearDiccionario(textosInglés);
                break;
        }

        if(actualizar)
            ActualizarTextosEscena();
    }

    private static Dictionary<string, string> CrearDiccionario(string traducciones)
    {
        var diccionario = JsonConvert.DeserializeObject<Dictionary<string, string>>(traducciones);
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
