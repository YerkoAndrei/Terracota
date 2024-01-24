using System.Linq;
using System.Collections.Generic;
using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI;
using Stride.Graphics;
using Stride.UI.Panels;
using Stride.Core.Mathematics;
using Stride.Input;

namespace Terracota;
using static Sistema;
using static Constantes;

public class InterfazJuego : SyncScript
{
    public Texture spriteBola;
    public Texture spriteMetralla;

    public Texture spriteAnfitrión;
    public Texture spriteHuesped;

    public Color colorAnfitrión;
    public Color colorHuesped;

    private Grid gridProyectil;
    private Grid gridGanador;
    private Grid btnPausa;

    private Grid gridCañón;
    private TextBlock txtNombreCañón;
    private ImageElement imgCañón;
    private ImageElement fondoCañón;

    private ImageElement imgTurnoAnfitrión;
    private ImageElement imgTurnoHuesped;

    private TextBlock txtGanador;
    private List<ImageElement> imgsGanador;
    private List<ImageElement> fondosGanador;

    private TextBlock txtProyectil;
    private TextBlock txtCantidadTurnos;
    private TextBlock txtMultiplicador;

    private Grid Opciones;
    private Grid animOpciones;
    private Grid gridPausa;
    private Grid animPausa;
    private ImageElement imgProyectil;

    private IPartida iPartida;

    private List<ImageElement> estadoAnfitrión;
    private List<ImageElement> estadoHuesped;

    private bool activo;
    private bool pausa;
    private bool animando;

    public override void Start()
    {
        var controlador = Entity.Scene.Entities.FirstOrDefault(e => e.Name == "ControladorPartida");
        foreach (var componente in controlador.Components)
        {
            if (componente is IPartida)
            {
                iPartida = (IPartida)componente;
                break;
            }
        }

        var página = Entity.Get<UIComponent>().Page.RootElement;
        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("PanelOscuro"), EnClicPausa);
        Opciones = página.FindVisualChildOfType<Grid>("Opciones");
        animOpciones = página.FindVisualChildOfType<Grid>("animOpciones");

        gridGanador = página.FindVisualChildOfType<Grid>("Ganador");
        gridProyectil = página.FindVisualChildOfType<Grid>("Proyectil");

        txtProyectil = página.FindVisualChildOfType<TextBlock>("txtProyectil");
        txtCantidadTurnos = página.FindVisualChildOfType<TextBlock>("txtCantidadTurnos");
        txtMultiplicador = página.FindVisualChildOfType<TextBlock>("txtMultiplicador");

        txtGanador = página.FindVisualChildOfType<TextBlock>("txtGanador");
        imgsGanador = new List<ImageElement>
        {
            página.FindVisualChildOfType<ImageElement>("imgGanador 0"),
            página.FindVisualChildOfType<ImageElement>("imgGanador 1"),
        };
        fondosGanador = new List<ImageElement>
        {
            página.FindVisualChildOfType<ImageElement>("FondoÍcono 0"),
            página.FindVisualChildOfType<ImageElement>("FondoÍcono 1"),
        };

        gridCañón = página.FindVisualChildOfType<Grid>("Cañón");
        txtNombreCañón = página.FindVisualChildOfType<TextBlock>("txtNombreCañón");
        imgCañón = página.FindVisualChildOfType<ImageElement>("imgCañón");
        fondoCañón = página.FindVisualChildOfType<ImageElement>("FondoCañón");

        imgTurnoAnfitrión = página.FindVisualChildOfType<ImageElement>("imgTurnoAnfitrión");
        imgTurnoHuesped = página.FindVisualChildOfType<ImageElement>("imgTurnoHuesped");

        imgProyectil = página.FindVisualChildOfType<ImageElement>("imgProyectil");
        ConfigurarBotónConImagen(página.FindVisualChildOfType<Button>("btnProyectil"), imgProyectil, EnClicProyectil);

        estadoAnfitrión = new List<ImageElement>
        {
            página.FindVisualChildOfType<ImageElement>("imgAnfitrión_0"),
            página.FindVisualChildOfType<ImageElement>("imgAnfitrión_1"),
            página.FindVisualChildOfType<ImageElement>("imgAnfitrión_2")
        };

        estadoHuesped = new List<ImageElement>
        {
            página.FindVisualChildOfType<ImageElement>("imgHuesped_0"),
            página.FindVisualChildOfType<ImageElement>("imgHuesped_1"),
            página.FindVisualChildOfType<ImageElement>("imgHuesped_2")
        };

        // Menú pausa
        gridPausa = página.FindVisualChildOfType<Grid>("Pausa");
        animPausa = página.FindVisualChildOfType<Grid>("animPausa");
        btnPausa = página.FindVisualChildOfType<Grid>("btnPausa");
        ConfigurarBotón(btnPausa, EnClicPausa);

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnReanudar"), EnClicPausa);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnReiniciar"), EnClicReiniciar);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnOpciones"), EnClicOpciones);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSalir"), EnClicSalir);

        // Predeterminado
        pausa = false;
        gridGanador.Visibility = Visibility.Hidden;
        gridPausa.Visibility = Visibility.Hidden;
    }

    public override void Update()
    {
        if (!activo)
            return;

        if (Input.IsKeyPressed(Keys.Escape))
            EnClicPausa();
    }

    public void Activar(bool activar)
    {
        activo = activar;
        if (activar)
            Entity.Get<UIComponent>().Page.RootElement.Visibility = Visibility.Visible;
        else
            Entity.Get<UIComponent>().Page.RootElement.Visibility = Visibility.Hidden;
    }

    public bool ObtenerPausa()
    {
        return pausa;
    }

    private void EnClicPausa()
    {
        if (animando)
            return;

        pausa = !pausa;
        animando = true;

        if (pausa)
        {
            gridPausa.Visibility = Visibility.Visible;
            SistemaSonido.CambiarMúsica(false, 0.5f);
            SistemaAnimación.AnimarElemento(animPausa, 0.2f, true, Direcciones.arriba, TipoCurva.rápida, () =>
            {
                animando = false;
            });
        }
        else
        {
            SistemaSonido.CambiarMúsica(true, 0.5f);
            SistemaAnimación.AnimarElemento(animPausa, 0.2f, false, Direcciones.arriba, TipoCurva.rápida, () =>
            {
                gridPausa.Visibility = Visibility.Hidden;
                animando = false;
            });
        }
    }

    private void EnClicReiniciar()
    {
        if (animando)
            return;

        SistemaSonido.CambiarMúsica(false);
        SistemaEscenas.CambiarEscena(Escenas.local);
    }

    private void EnClicOpciones()
    {
        if (animando)
            return;

        animando = true;
        Opciones.Visibility = Visibility.Visible;
        SistemaAnimación.AnimarElemento(animOpciones, 0.2f, true, Direcciones.abajo, TipoCurva.rápida, () =>
        {
            animando = false;
        });
    }

    private void EnClicSalir()
    {
        if (animando)
            return;

        if (SistemaRed.ObtenerJugando())
            SistemaRed.CerrarConexión(true);

        SistemaSonido.CambiarMúsica(false);
        SistemaEscenas.CambiarEscena(Escenas.menú);
    }

    private void EnClicProyectil()
    {
        if (animando)
            return;

        CambiarProyectil(iPartida.CambiarProyectil());
    }

    public void OcultarInterfazLocal()
    {
        txtProyectil.Text = string.Empty;
        txtCantidadTurnos.Text = string.Empty;

        gridCañón.Visibility = Visibility.Hidden;
        gridProyectil.Visibility = Visibility.Hidden;
        btnPausa.Visibility = Visibility.Hidden;
    }

    public void OcultarInterfazRemoto()
    {
        txtCantidadTurnos.Text = string.Empty;

        gridProyectil.Visibility = Visibility.Hidden;
        btnPausa.Visibility = Visibility.Hidden;
    }

    public void MostrarInterfazLocal(TipoJugador jugador, TipoProyectil proyectil, int turno, float multiplicador)
    {
        gridCañón.Visibility = Visibility.Visible;
        gridProyectil.Visibility = Visibility.Visible;
        btnPausa.Visibility = Visibility.Visible;

        CambiarNombreCañón(jugador);
        CambiarProyectil(proyectil);

        CambiarTurno(jugador);
        ActualizarEstado(turno, multiplicador);
    }

    public void MostrarInterfazRemoto(TipoJugador jugador, int turno, float multiplicador)
    {
        gridCañón.Visibility = Visibility.Visible;
        gridProyectil.Visibility = Visibility.Visible;
        btnPausa.Visibility = Visibility.Visible;

        CambiarTurno(jugador);
        ActualizarEstado(turno, multiplicador);
    }

    public void ComenzarInterfazRemoto(TipoJugador jugador, TipoProyectil proyectil)
    {
        CambiarNombreCañón(jugador);
        CambiarProyectil(proyectil);
    }

    private void ActualizarEstado(int turno, float multiplicador)
    {
        txtCantidadTurnos.Text = turno.ToString();
        txtMultiplicador.Text = "x" + multiplicador.ToString("0.0");

        if(multiplicador >= multiplicadorMáximo)
            txtMultiplicador.TextColor = Color.Red;
    }

    private void CambiarTurno(TipoJugador jugador)
    {
        imgTurnoAnfitrión.Visibility = Visibility.Hidden;
        imgTurnoHuesped.Visibility = Visibility.Hidden;

        switch (jugador)
        {
            case TipoJugador.anfitrión:
                imgTurnoAnfitrión.Visibility = Visibility.Visible;
                break;
            case TipoJugador.huesped:
                imgTurnoHuesped.Visibility = Visibility.Visible;
                break;
        }
    }

    private void CambiarNombreCañón(TipoJugador jugador)
    {
        switch (jugador)
        {
            case TipoJugador.anfitrión:
                txtNombreCañón.Text = SistemaTraducción.ObtenerTraducción("anfitrión");
                imgCañón.Source = ObtenerSprite(spriteAnfitrión);
                fondoCañón.Color = colorAnfitrión;
                break;
            case TipoJugador.huesped:
                txtNombreCañón.Text = SistemaTraducción.ObtenerTraducción("huesped");
                imgCañón.Source = ObtenerSprite(spriteHuesped);
                fondoCañón.Color = colorHuesped;
                break;
        }
    }


    private void CambiarProyectil(TipoProyectil proyectil)
    {
        switch (proyectil)
        {
            case TipoProyectil.bola:
                txtProyectil.Text = SistemaTraducción.ObtenerTraducción("bola");
                imgProyectil.Source = ObtenerSprite(spriteBola);
                break;
            case TipoProyectil.metralla:
                txtProyectil.Text = SistemaTraducción.ObtenerTraducción("metralla");
                imgProyectil.Source = ObtenerSprite(spriteMetralla);
                break;
        }
    }

    public void RestarAnfitrión(int estatua)
    {
        estadoAnfitrión[estatua].Color = Color.Red;
    }

    public void RestarHuesped(int estatua)
    {
        estadoHuesped[estatua].Color = Color.Red;
    }

    public bool ObtenerActivo()
    {
        return activo;
    }

    public async void MostrarGanador(TipoJugador jugador, int turno)
    {
        activo = false;
        txtCantidadTurnos.Text = turno.ToString();
        gridProyectil.Visibility = Visibility.Hidden;
        gridCañón.Visibility = Visibility.Hidden;

        gridGanador.Visibility = Visibility.Visible;
        btnPausa.Visibility = Visibility.Visible;

        animando = true;
        SistemaAnimación.AnimarElemento(gridGanador, 0.5f, true, Direcciones.arriba, TipoCurva.suave, () =>
        {
            animando = false;
        });

        switch (jugador)
        {
            case TipoJugador.anfitrión:
                txtGanador.Text = SistemaTraducción.ObtenerTraducción("anfitrión");
                foreach(var img in imgsGanador)
                {
                    img.Source = ObtenerSprite(spriteAnfitrión);
                }
                foreach (var fondo in fondosGanador)
                {
                    fondo.Color = colorAnfitrión;
                }
                break;
            case TipoJugador.huesped:
                txtGanador.Text = SistemaTraducción.ObtenerTraducción("huesped");
                foreach (var img in imgsGanador)
                {
                    img.Source = ObtenerSprite(spriteHuesped);
                }
                foreach (var fondo in fondosGanador)
                {
                    fondo.Color = colorHuesped;
                }
                break;
        }

        // Sonido
        await SistemaSonido.SonarVictoria();
    }
}
