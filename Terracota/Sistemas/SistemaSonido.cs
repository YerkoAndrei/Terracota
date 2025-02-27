﻿using System.Globalization;
using System.Threading.Tasks;
using Stride.Engine;
using Stride.Audio;
using Stride.Core.Mathematics;

namespace Terracota;
using static Utilidades;
using static Constantes;

public class SistemaSonido : StartupScript
{
    public Sound músicaMelodía;
    public Sound músicaPercusión;

    public Sound sonidoInicio;
    public Sound sonidoVictoria;

    public Sound sonidoBotónEntra;
    public Sound sonidoBotónSale;

    public Sound sonidoCañonazo;
    public Sound sonidoCañónVertical;
    public Sound sonidoCañónHorizontal;

    public Sound sonidoBola;
    public Sound sonidoBloqueEstatuaDesactivada;

    public Sound sonidoBloqueEstatua;
    public Sound sonidoBloqueCorto;
    public Sound sonidoBloqueLargo;

    private static SistemaSonido instancia;

    private SoundInstance melodía;
    private SoundInstance perscusión;

    private SoundInstance inicio;
    private SoundInstance victoria;

    private SoundInstance botónEntra;
    private SoundInstance botónSale;

    private SoundInstance cañonazo;
    private SoundInstance cañónVertical;
    private SoundInstance cañónHorizontal;

    private SoundInstance bola;
    private SoundInstance bloqueEstatuaDesactivada;
    private SoundInstance bloqueEstatua;
    private SoundInstance bloqueCorto;
    private SoundInstance bloqueLargo;

    private bool percusiónActiva;
    private bool cambiandoMúsica;

    public override void Start()
    {
        instancia = this;
        
        melodía = músicaMelodía.CreateInstance();
        perscusión = músicaPercusión.CreateInstance();
        
        inicio = sonidoInicio.CreateInstance();
        victoria = sonidoVictoria.CreateInstance();
        
        // Interfaz
        botónEntra = sonidoBotónEntra.CreateInstance();
        botónSale = sonidoBotónSale.CreateInstance();
        
        // Cañón
        cañonazo = sonidoCañonazo.CreateInstance();
        cañónVertical = sonidoCañónVertical.CreateInstance();
        cañónHorizontal = sonidoCañónHorizontal.CreateInstance();

        // Bloques
        bola = sonidoBola.CreateInstance();
        bloqueEstatuaDesactivada = sonidoBloqueEstatuaDesactivada.CreateInstance();

        bloqueEstatua = sonidoBloqueEstatua.CreateInstance();
        bloqueCorto = sonidoBloqueCorto.CreateInstance();
        bloqueLargo = sonidoBloqueLargo.CreateInstance();

        // Música
        percusiónActiva = false;

        melodía.Volume = 0;
        perscusión.Volume = 0;
        
        melodía.IsLooping = true;
        perscusión.IsLooping = true;

        _ = CambiarVolumenMelodía(0, ObtenerVolumen(Configuraciones.volumenMúsica), true);
    }

    public static async void CambiarMúsica(bool percusión, float duración = 0)
    {
        if (percusión)
        {
            instancia.percusiónActiva = true;

            // Delay(2) para esperar async
            instancia.cambiandoMúsica = true;
            await Task.Delay(2);
            instancia.cambiandoMúsica = false;
            await CambiarVolumenTambores(0, ObtenerVolumen(Configuraciones.volumenMúsica), duración);
        }
        else if(!percusión && instancia.percusiónActiva)
        {
            instancia.percusiónActiva = false;

            // Delay(2) para esperar async
            instancia.cambiandoMúsica = true;
            await Task.Delay(2);
            instancia.cambiandoMúsica = false;
            await CambiarVolumenTambores(ObtenerVolumen(Configuraciones.volumenMúsica), 0, duración);
        }
    }

    private static async Task CambiarVolumenMelodía(float inicio, float final, bool iniciando, float duración = 0)
    {
        if (duración == 0)
            duración = 3f;

        float tiempoLerp = 0;
        float tiempo = 0;

        if (iniciando)
        {
            await Task.Delay(1000);
            instancia.melodía.Play();
            instancia.perscusión.Play();
        }

        while (tiempoLerp < duración)
        {
            tiempo = SistemaAnimación.EvaluarSuave(tiempoLerp / duración);

            instancia.melodía.Volume = MathUtil.Lerp(inicio, final, tiempo);
            tiempoLerp += (float)instancia.Game.UpdateTime.WarpElapsed.TotalSeconds;

            // Task.Delay se muestra con lag visual, pero en sonido no se nota
            await Task.Delay(1);
        }

        // Fin
        instancia.melodía.Volume = final;
    }

    private static async Task CambiarVolumenTambores(float inicio, float final, float duración = 0)
    {
        if(duración == 0)
            duración = 1.5f;

        float tiempoLerp = 0;
        float tiempo = 0;

        while (tiempoLerp < duración)
        {
            tiempo = SistemaAnimación.EvaluarSuave(tiempoLerp / duración);

            instancia.perscusión.Volume = MathUtil.Lerp(inicio, final, tiempo);
            tiempoLerp += (float)instancia.Game.UpdateTime.WarpElapsed.TotalSeconds;

            // Task.Delay se muestra con lag visual, pero en sonido no se nota
            await Task.Delay(1);

            if (instancia.cambiandoMúsica)
            {
                instancia.cambiandoMúsica = false;
                return;
            }
        }

        // Fin
        instancia.perscusión.Volume = final;
    }

    public static async Task SonarVictoria()
    {
        instancia.percusiónActiva = false;
        await CambiarVolumenTambores(ObtenerVolumen(Configuraciones.volumenMúsica), 0, 0.2f);
        await CambiarVolumenMelodía(ObtenerVolumen(Configuraciones.volumenMúsica), 0, false, 0.2f);

        instancia.victoria.Stop();
        instancia.victoria.Volume = ObtenerVolumen(Configuraciones.volumenEfectos);
        instancia.victoria.PlayExclusive();

        await Task.Delay(2000);
        await CambiarVolumenMelodía(0, ObtenerVolumen(Configuraciones.volumenMúsica), false, 2f);
        instancia.melodía.Volume = ObtenerVolumen(Configuraciones.volumenMúsica);
    }

    public static void SonarInicio()
    {
        instancia.inicio.Stop();
        instancia.inicio.Volume = ObtenerVolumen(Configuraciones.volumenMúsica);
        instancia.inicio.PlayExclusive();
    }

    public static void SonarRuleta()
    {
        instancia.botónSale.Stop();
        instancia.botónSale.Volume = ObtenerVolumen(Configuraciones.volumenEfectos) * 0.2f;
        instancia.botónSale.PlayExclusive();
    }

    public static void SonarBotónEntra()
    {
        instancia.botónEntra.Stop();
        instancia.botónEntra.Volume = ObtenerVolumen(Configuraciones.volumenEfectos);
        instancia.botónEntra.PlayExclusive();
    }

    public static void SonarBotónSale()
    {
        instancia.botónSale.Stop();
        instancia.botónSale.Volume = ObtenerVolumen(Configuraciones.volumenEfectos);
        instancia.botónSale.PlayExclusive();
    }

    public static void SonarCañonazo()
    {
        instancia.cañonazo.Stop();
        instancia.cañonazo.Volume = ObtenerVolumen(Configuraciones.volumenEfectos);
        instancia.cañonazo.PlayExclusive();
    }

    public static void SonarCañónVertical()
    {
        instancia.cañónVertical.Stop();
        instancia.cañónVertical.Volume = ObtenerVolumen(Configuraciones.volumenEfectos) * 0.02f;
        instancia.cañónVertical.PlayExclusive();
    }

    public static void SonarCañónHorizontal()
    {
        instancia.cañónHorizontal.Stop();
        instancia.cañónHorizontal.Volume = ObtenerVolumen(Configuraciones.volumenEfectos) * 0.02f;
        instancia.cañónHorizontal.PlayExclusive();
    }

    // Creación
    public static void SonarBloque(TipoBloque bloque)
    {
        switch (bloque)
        {
            case TipoBloque.estatua:
                instancia.bloqueEstatua.Stop();
                instancia.bloqueEstatua.Volume = ObtenerVolumen(Configuraciones.volumenEfectos);
                instancia.bloqueEstatua.PlayExclusive();
                break;
            case TipoBloque.corto:
                instancia.bloqueCorto.Stop();
                instancia.bloqueCorto.Volume = ObtenerVolumen(Configuraciones.volumenEfectos);
                instancia.bloqueCorto.PlayExclusive();
                break;
            case TipoBloque.largo:
                instancia.bloqueLargo.Stop();
                instancia.bloqueLargo.Volume = ObtenerVolumen(Configuraciones.volumenEfectos);
                instancia.bloqueLargo.PlayExclusive();
                break;
        }
    }

    // Juego
    public static void SonarBola(float fuerza)
    {
        var volumen = (ObtenerVolumen(Configuraciones.volumenEfectos) * fuerza) - RangoAleatorio(0, 0.2f);

        instancia.bola.Stop();
        instancia.bola.Volume = MathUtil.Clamp(volumen, 0, 1);
        instancia.bola.Pitch = RangoAleatorio(0.8f, 1.2f);
        instancia.bola.Play();
    }

    public static void SonarEstatuaDesactivada()
    {
        instancia.bloqueEstatuaDesactivada.Stop();
        instancia.bloqueEstatuaDesactivada.Volume = ObtenerVolumen(Configuraciones.volumenEfectos);
        instancia.bloqueEstatuaDesactivada.Play();
    }

    public static SoundInstance CrearInstancia(TipoBloque bloque)
    {
        switch (bloque)
        {
            case TipoBloque.estatua:
                return instancia.sonidoBloqueEstatua.CreateInstance();
            case TipoBloque.corto:
                return instancia.sonidoBloqueCorto.CreateInstance();
            case TipoBloque.largo:
                return instancia.sonidoBloqueLargo.CreateInstance();
            default:
                return null;
        }
    }

    // Música
    public static void ActualizarVolumenMúsica()
    {
        instancia.melodía.Volume = ObtenerVolumen(Configuraciones.volumenMúsica);

        if(instancia.percusiónActiva)
            instancia.perscusión.Volume = ObtenerVolumen(Configuraciones.volumenMúsica);
    }

    // Mezclador
    public static float ObtenerVolumen(Configuraciones volumen)
    {
        var volumenGeneral = float.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.volumenGeneral), CultureInfo.InvariantCulture);
        switch (volumen)
        {
            default:
                return 0;
            case Configuraciones.volumenMúsica:
                var volumenMúsica = float.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.volumenMúsica), CultureInfo.InvariantCulture);
                return volumenGeneral * volumenMúsica;
            case Configuraciones.volumenEfectos:
                var volumenEfectos = float.Parse(SistemaMemoria.ObtenerConfiguración(Configuraciones.volumenEfectos), CultureInfo.InvariantCulture);
                return volumenGeneral * volumenEfectos;
        }
    }
}
