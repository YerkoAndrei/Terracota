using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI;
using Stride.UI.Events;
using Stride.Graphics;
using Stride.UI.Panels;
using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;

namespace Terracota;
using static Constantes;
using static Sistema;

public class ControladorInterfaz : StartupScript
{
    public ControladorPartidaLocal controladorPartida;
    public UrlReference<Scene> escenaJuego;

    public Texture spriteBola;
    public Texture spriteMetralla;

    public Texture spriteAnfitrión;
    public Texture spriteHuesped;

    private Grid gridProyectil;
    private Grid gridGanador;

    private TextBlock txtTurno;
    private ImageElement imgTurno;

    private TextBlock txtGanador;
    private ImageElement imgGanador;

    private TextBlock txtProyectil;
    private TextBlock txtCantidadTurnos;
    private TextBlock txtMultiplicador;

    private Button btnProyectil;
    private Button btnPausa;

    private List<ImageElement> estadoAnfitrión;
    private List<ImageElement> estadoHuesped;

    public override void Start()
    {
        var página = Entity.Get<UIComponent>().Page;

        gridGanador = página.RootElement.FindVisualChildOfType<Grid>("Ganador");
        gridProyectil = página.RootElement.FindVisualChildOfType<Grid>("Proyectil");

        txtTurno = página.RootElement.FindVisualChildOfType<TextBlock>("txtTurno");
        txtProyectil = página.RootElement.FindVisualChildOfType<TextBlock>("txtProyectil");
        txtCantidadTurnos = página.RootElement.FindVisualChildOfType<TextBlock>("txtCantidadTurnos");
        txtMultiplicador = página.RootElement.FindVisualChildOfType<TextBlock>("txtMultiplicador");

        txtGanador = página.RootElement.FindVisualChildOfType<TextBlock>("txtGanador");
        imgGanador = página.RootElement.FindVisualChildOfType<ImageElement>("imgGanador");
        imgTurno = página.RootElement.FindVisualChildOfType<ImageElement>("imgTurno");

        btnProyectil = página.RootElement.FindVisualChildOfType<Button>("btnProyectil");
        btnProyectil.Click += EnClicProyectil;

        btnPausa = página.RootElement.FindVisualChildOfType<Button>("btnPausa");
        btnPausa.Click += EnClicPausa;

        estadoAnfitrión = new List<ImageElement>
        {
            página.RootElement.FindVisualChildOfType<ImageElement>("imgAnfitrión_0"),
            página.RootElement.FindVisualChildOfType<ImageElement>("imgAnfitrión_1"),
            página.RootElement.FindVisualChildOfType<ImageElement>("imgAnfitrión_2")
        };

        estadoHuesped = new List<ImageElement>
        {
            página.RootElement.FindVisualChildOfType<ImageElement>("imgHuesped_0"),
            página.RootElement.FindVisualChildOfType<ImageElement>("imgHuesped_1"),
            página.RootElement.FindVisualChildOfType<ImageElement>("imgHuesped_2")
        };

        // Predeterminado
        gridGanador.Visibility = Visibility.Hidden;
        CambiarInterfaz(TipoJugador.anfitrión, TipoProyectil.bola);
    }

    private void EnClicPausa(object sender, RoutedEventArgs e)
    {
        // Recargar escena
        Content.Unload(SceneSystem.SceneInstance.RootScene);
        SceneSystem.SceneInstance.RootScene = Content.Load(escenaJuego);
    }

    private void EnClicProyectil(object sender, RoutedEventArgs e)
    {
        CambiarProyectil(controladorPartida.CambiarProyectil());
    }

    public void PausarInterfaz()
    {
        txtProyectil.Text = string.Empty;
        txtCantidadTurnos.Text = string.Empty;

        btnPausa.Visibility = Visibility.Hidden;
        gridProyectil.Visibility = Visibility.Hidden;

        btnProyectil.CanBeHitByUser = false;
        btnPausa.CanBeHitByUser = false;
    }

    public void ActivarTurno(bool activar)
    {
        if (activar)
            imgTurno.Visibility = Visibility.Visible;
        else
        {
            imgTurno.Visibility = Visibility.Hidden;
            txtTurno.Text = string.Empty;
        }
    }

    public void CambiarInterfaz(TipoJugador jugador, TipoProyectil proyectil)
    {
        ActivarTurno(false);
        CambiarTurno(jugador);

        imgTurno.Visibility = Visibility.Visible;
        gridProyectil.Visibility = Visibility.Visible;
        btnPausa.Visibility = Visibility.Visible;

        btnProyectil.CanBeHitByUser = true;
        btnPausa.CanBeHitByUser = true;

        CambiarProyectil(proyectil);
    }

    public void ActualizarTurno(int turno, float multiplicador)
    {
        txtCantidadTurnos.Text = turno.ToString();
        txtMultiplicador.Text = "x" + multiplicador.ToString("0.0");

        if(multiplicador >= multiplicadorMáximo)
            txtMultiplicador.TextColor = Color.Red;
    }

    private void CambiarTurno(TipoJugador jugador)
    {
        switch (jugador)
        {
            case TipoJugador.anfitrión:
                txtTurno.Text = "Anfitrión";
                imgTurno.Source = ObtenerSprite(spriteAnfitrión);
                break;
            case TipoJugador.huesped:
                txtTurno.Text = "Huesped";
                imgTurno.Source = ObtenerSprite(spriteHuesped);
                break;
        }
    }

    private void CambiarProyectil(TipoProyectil proyectil)
    {
        switch (proyectil)
        {
            case TipoProyectil.bola:
                txtProyectil.Text = "Bola";
                CambiarImagenBotón(btnProyectil, spriteBola);
                break;
            case TipoProyectil.metralla:
                txtProyectil.Text = "Metralla";
                CambiarImagenBotón(btnProyectil, spriteMetralla);
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

    public void MostrarGanador(TipoJugador jugador, int turno)
    {
        CambiarTurno(jugador);
        ActivarTurno(true);

        txtCantidadTurnos.Text = turno.ToString();

        gridGanador.Visibility = Visibility.Visible;

        gridProyectil.Visibility = Visibility.Hidden;
        btnPausa.Visibility = Visibility.Visible;

        btnProyectil.CanBeHitByUser = false;
        btnPausa.CanBeHitByUser = true;

        switch (jugador)
        {
            case TipoJugador.anfitrión:
                imgGanador.Source = ObtenerSprite(spriteAnfitrión);
                txtGanador.Text = "Ganador: " + "Anfitrión";
                break;
            case TipoJugador.huesped:
                imgGanador.Source = ObtenerSprite(spriteHuesped);
                txtGanador.Text = "Ganador: " + "Huesped";
                break;
        }
    }
}
