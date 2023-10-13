using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI;
using Stride.UI.Events;
using Stride.Graphics;

namespace Terracota;
using static Constantes;
using static Sistema;

public class ControladorInterfaz : StartupScript
{
    public ControladorPartidaLocal controladorPartida;

    public Texture spriteBola;
    public Texture spriteMetralla;

    public Texture spriteAnfitrión;
    public Texture spriteHuesped;

    private TextBlock txtTurno;
    private TextBlock txtProyectil;

    private ImageElement imgTurno;
    private Button btnProyectil;
    private Button btnPausa;

    public override void Start()
    {
        var página = Entity.Get<UIComponent>().Page;

        txtTurno = página.RootElement.FindVisualChildOfType<TextBlock>("txtTurno");
        txtProyectil = página.RootElement.FindVisualChildOfType<TextBlock>("txtProyectil");

        imgTurno = página.RootElement.FindVisualChildOfType<ImageElement>("imgTurno");
        btnProyectil = página.RootElement.FindVisualChildOfType<Button>("btnProyectil");
        btnProyectil.Click += EnClicProyectil;

        btnPausa = página.RootElement.FindVisualChildOfType<Button>("btnPausa");
        btnPausa.Click += EnClicPausa;

        // Predeterminado
        CambiarInterfaz(TipoJugador.anfitrión, TipoProyectil.bola);
    }

    private void EnClicPausa(object sender, RoutedEventArgs e)
    {
        // Reiniciar partida

    }

    private void EnClicProyectil(object sender, RoutedEventArgs e)
    {
        CambiarProyectil(controladorPartida.CambiarProyectil());
    }

    public void PausarInterfaz()
    {
        txtTurno.Text = string.Empty;
        txtProyectil.Text = string.Empty;

        imgTurno.Source = null;
        btnProyectil.NotPressedImage = null;

        btnProyectil.CanBeHitByUser = false;
    }

    public void CambiarInterfaz(TipoJugador jugador, TipoProyectil proyectil)
    {
        switch(jugador)
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

        btnProyectil.CanBeHitByUser = true;
        CambiarProyectil(proyectil);
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

    public void RestarAnfitrión()
    {

    }

    public void RestarHuesped()
    {

    }
}
