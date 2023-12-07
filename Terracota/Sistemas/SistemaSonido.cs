using System.Globalization;
using System.Threading.Tasks;
using Stride.Engine;
using Stride.Audio;
using Stride.Core.Mathematics;

namespace Terracota;
using static Constantes;

public class SistemaSonido : StartupScript
{
    public Sound músicaMelodía;
    public Sound músicaTambores;

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
    private SoundInstance tambores;

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

    public override void Start()
    {
        instancia = this;
        /*
        melodía = músicaMelodía.CreateInstance();
        tambores = músicaTambores.CreateInstance();
        */
        
        victoria = sonidoInicio.CreateInstance();
        inicio = sonidoInicio.CreateInstance();
        
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

        /*
        // Música
        ActualizarVolumenMúsica();
        CambiarMúsica(false);
        
        melodía.IsLooping = true;
        tambores.IsLooping = true;

        melodía.Play();
        tambores.Play();*/
    }

    public static async void CambiarMúsica(bool tambores)
    {/*
        if (tambores)
            await CambiarVolumenTambores(0, ObtenerVolumen(Configuraciones.volumenMúsica));
        else
            await CambiarVolumenTambores(ObtenerVolumen(Configuraciones.volumenMúsica), 0);*/
    }

    private static async Task CambiarVolumenTambores(float inicio, float final)
    {
        float duración = 1;
        float tiempoLerp = 0;
        float tiempo = 0;

        while (tiempoLerp < duración)
        {
            tiempo = SistemaAnimación.EvaluarSuave(tiempoLerp / duración);

            instancia.tambores.Volume = MathUtil.Lerp(inicio, final, tiempo);
            tiempoLerp += (float)instancia.Game.UpdateTime.Elapsed.TotalSeconds;

            // Task.Delay se muestra con lag visual, pero en sonido no se nota
            await Task.Delay(1);
        }

        // Fin
        instancia.tambores.Volume = final;
    }

    public static void SonarVictoria()
    {
        instancia.victoria.Stop();
        instancia.victoria.Volume = ObtenerVolumen(Configuraciones.volumenMúsica);
        instancia.victoria.PlayExclusive();
    }

    public static void SonarInicio()
    {
        instancia.inicio.Stop();
        instancia.inicio.Volume = ObtenerVolumen(Configuraciones.volumenMúsica);
        instancia.inicio.PlayExclusive();
    }

    public static void SonarRuleta()
    {
        instancia.botónEntra.Stop();
        instancia.botónEntra.Volume = ObtenerVolumen(Configuraciones.volumenMúsica);
        instancia.botónEntra.PlayExclusive();
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

    public static void SonarEstatuaDesactivada()
    {
        instancia.bloqueEstatuaDesactivada.Stop();
        instancia.bloqueEstatuaDesactivada.Volume = ObtenerVolumen(Configuraciones.volumenEfectos);
        instancia.bloqueEstatuaDesactivada.PlayExclusive();
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
        if (instancia.bola.PlayState == Stride.Media.PlayState.Playing)
            return;

        instancia.bola.Volume = ObtenerVolumen(Configuraciones.volumenEfectos) * fuerza;
        instancia.bola.PlayExclusive();
    }

    public static void SonarBloqueFísico(TipoBloque bloque, float fuerza)
    {
        switch (bloque)
        {
            case TipoBloque.estatua:
                if (instancia.bloqueEstatua.PlayState == Stride.Media.PlayState.Playing)
                    return;

                instancia.bloqueEstatua.Stop();
                instancia.bloqueEstatua.Volume = ObtenerVolumen(Configuraciones.volumenEfectos) * fuerza;
                instancia.bloqueEstatua.PlayExclusive();
                break;
            case TipoBloque.corto:
                if (instancia.bloqueCorto.PlayState == Stride.Media.PlayState.Playing)
                    return;

                instancia.bloqueCorto.Stop();
                instancia.bloqueCorto.Volume = ObtenerVolumen(Configuraciones.volumenEfectos) * fuerza;
                instancia.bloqueCorto.PlayExclusive();
                break;
            case TipoBloque.largo:
                if (instancia.bloqueLargo.PlayState == Stride.Media.PlayState.Playing)
                    return;

                instancia.bloqueLargo.Volume = ObtenerVolumen(Configuraciones.volumenEfectos) * fuerza;
                instancia.bloqueLargo.PlayExclusive();
                break;
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
