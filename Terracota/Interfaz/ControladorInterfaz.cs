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

    private TextBlock txtTurno;
    private TextBlock txtProyectil;

    private Grid gridGanador;
    private TextBlock txtGanador;
    private ImageElement imgTurno;

    private Button btnProyectil;
    private Button btnPausa;

    private List<ImageElement> estadoAnfitrión;
    private List<ImageElement> estadoHuesped;

    public override void Start()
    {
        var página = Entity.Get<UIComponent>().Page;

        txtTurno = página.RootElement.FindVisualChildOfType<TextBlock>("txtTurno");
        txtProyectil = página.RootElement.FindVisualChildOfType<TextBlock>("txtProyectil");

        gridGanador = página.RootElement.FindVisualChildOfType<Grid>("Ganador");
        txtGanador = página.RootElement.FindVisualChildOfType<TextBlock>("txtGanador");
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
        txtTurno.Text = string.Empty;
        txtProyectil.Text = string.Empty;

        imgTurno.Visibility = Visibility.Hidden;
        btnProyectil.Visibility = Visibility.Hidden;
        btnPausa.Visibility = Visibility.Hidden;

        btnProyectil.CanBeHitByUser = false;
        btnPausa.CanBeHitByUser = false;
    }

    public void CambiarInterfaz(TipoJugador jugador, TipoProyectil proyectil)
    {
        CambiarTurno(jugador);

        imgTurno.Visibility = Visibility.Visible;
        btnProyectil.Visibility = Visibility.Visible;
        btnPausa.Visibility = Visibility.Visible;

        btnProyectil.CanBeHitByUser = true;
        btnPausa.CanBeHitByUser = true;

        CambiarProyectil(proyectil);
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

    public void MostrarGanador(TipoJugador jugador)
    {
        CambiarTurno(jugador);

        gridGanador.Visibility = Visibility.Visible;
        imgTurno.Visibility = Visibility.Visible;
        btnProyectil.Visibility = Visibility.Hidden;
        btnPausa.Visibility = Visibility.Visible;

        btnProyectil.CanBeHitByUser = false;
        btnPausa.CanBeHitByUser = true;

        switch (jugador)
        {
            case TipoJugador.anfitrión:
                txtGanador.Text = "Ganador: " + "Anfitrión";
                break;
            case TipoJugador.huesped:
                txtGanador.Text = "Ganador: " + "Huesped";
                break;
        }
    }
}
