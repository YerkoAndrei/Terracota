using Stride.Engine;
using Stride.Audio;
using System.Globalization;

namespace Terracota;
using static Constantes;

public class SistemaSonido : StartupScript
{
    public Sound músicaMelodía;
    public Sound músicaTambores;
    public Sound músicaVictoria;

    public Sound sonidoBotónEntra;
    public Sound sonidoBotónSale;

    public Sound sonidoCañonazo;
    public Sound sonidoCañónVertical;
    public Sound sonidoCañónHorizontal;

    public Sound sonidoBloqueDerrotado;
    public Sound sonidoBloqueEstatua;
    public Sound sonidoBloqueCorto;
    public Sound sonidoBloqueLargo;

    private static SistemaSonido instancia;

    private SoundInstance melodía;
    private SoundInstance tambores;
    private SoundInstance victoria;

    private SoundInstance botónEntra;
    private SoundInstance botónSale;

    private SoundInstance cañonazo;
    private SoundInstance cañónVertical;
    private SoundInstance cañónHorizontal;

    private SoundInstance bloqueDerrotado;
    private SoundInstance bloqueEstatua;
    private SoundInstance bloqueCorto;
    private SoundInstance bloqueLargo;

    public override void Start()
    {
        instancia = this;
        /*
        melodía = músicaMelodía.CreateInstance();
        tambores = músicaTambores.CreateInstance();
        victoria = músicaVictoria.CreateInstance();
        */
        // Interfaz
        botónEntra = sonidoBotónEntra.CreateInstance();
        botónSale = sonidoBotónSale.CreateInstance();

        // Cañón
        cañonazo = sonidoCañonazo.CreateInstance();
        cañónVertical = sonidoCañónVertical.CreateInstance();
        cañónHorizontal = sonidoCañónHorizontal.CreateInstance();

        // Bloques
        bloqueDerrotado = sonidoBloqueDerrotado.CreateInstance();
        bloqueEstatua = sonidoBloqueEstatua.CreateInstance();
        bloqueCorto = sonidoBloqueCorto.CreateInstance();
        bloqueLargo = sonidoBloqueLargo.CreateInstance();

        /*
        // Música
        ActualizarVolumenMúsica();
        CambiarMúsica(false);
        
        melodía.IsLooping = true;
        tambores.IsLooping = true;

        melodía.Play();
        tambores.Play();*/
    }

    public static void CambiarMúsica(bool tambores)
    {
        if(tambores)
            instancia.tambores.Volume = ObtenerVolumen(Configuraciones.volumenMúsica);
        else
            instancia.tambores.Volume = 0;
    }

    public static void SonarVictoria()
    {
        instancia.victoria.Stop();
        instancia.victoria.Volume = ObtenerVolumen(Configuraciones.volumenMúsica);
        instancia.victoria.PlayExclusive();
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
        instancia.cañónVertical.Volume = ObtenerVolumen(Configuraciones.volumenEfectos);
        instancia.cañónVertical.PlayExclusive();
    }

    public static void SonarCañónHorizontal()
    {
        instancia.cañónHorizontal.Stop();
        instancia.cañónHorizontal.Volume = ObtenerVolumen(Configuraciones.volumenEfectos);
        instancia.cañónHorizontal.PlayExclusive();
    }

    public static void SonarEstatuaDerrotada()
    {
        instancia.bloqueDerrotado.Stop();
        instancia.bloqueDerrotado.Volume = ObtenerVolumen(Configuraciones.volumenEfectos);
        instancia.bloqueDerrotado.PlayExclusive();
    }

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

    // Sonido espaciado
    public static Sound ObtenerSonidoBloque(TipoBloque bloque)
    {
        switch(bloque)
        {
            case TipoBloque.estatua:
                return instancia.sonidoBloqueEstatua;
            case TipoBloque.corto:
                return instancia.sonidoBloqueCorto;
            case TipoBloque.largo:
                return instancia.sonidoBloqueLargo;
            default:
            case TipoBloque.nada:
                return null;
        }
    }

    // Música
    public static void ActualizarVolumenMúsica()
    {/*
        instancia.melodía.Volume = ObtenerVolumen(Configuraciones.volumenMúsica);
        instancia.tambores.Volume = ObtenerVolumen(Configuraciones.volumenMúsica);*/
    }

    // Mezclador
    private static float ObtenerVolumen(Configuraciones volumen)
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
