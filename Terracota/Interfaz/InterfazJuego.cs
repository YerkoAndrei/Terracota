﻿using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI;
using Stride.Graphics;
using Stride.UI.Panels;
using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;

namespace Terracota;
using static Sistema;
using static Constantes;

public class InterfazJuego : StartupScript
{
    public ControladorPartidaLocal controladorPartida;
    public UrlReference<Scene> escenaJuego;
    public UrlReference<Scene> escenaMenú;

    public Texture spriteBola;
    public Texture spriteMetralla;

    public Texture spriteAnfitrión;
    public Texture spriteHuesped;

    private Grid gridProyectil;
    private Grid gridGanador;
    private Grid btnPausa;

    private TextBlock txtTurno;
    private ImageElement imgTurno;

    private TextBlock txtGanador;
    private ImageElement imgGanador;

    private TextBlock txtProyectil;
    private TextBlock txtCantidadTurnos;
    private TextBlock txtMultiplicador;

    private Grid gridPausa;
    private ImageElement imgProyectil;

    private List<ImageElement> estadoAnfitrión;
    private List<ImageElement> estadoHuesped;

    private bool pausa;

    public override void Start()
    {
        var página = Entity.Get<UIComponent>().Page.RootElement;
        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("PanelOscuro"), EnClicPausa);

        gridGanador = página.FindVisualChildOfType<Grid>("Ganador");
        gridProyectil = página.FindVisualChildOfType<Grid>("Proyectil");

        txtTurno = página.FindVisualChildOfType<TextBlock>("txtTurno");
        txtProyectil = página.FindVisualChildOfType<TextBlock>("txtProyectil");
        txtCantidadTurnos = página.FindVisualChildOfType<TextBlock>("txtCantidadTurnos");
        txtMultiplicador = página.FindVisualChildOfType<TextBlock>("txtMultiplicador");

        txtGanador = página.FindVisualChildOfType<TextBlock>("txtGanador");
        imgGanador = página.FindVisualChildOfType<ImageElement>("imgGanador");
        imgTurno = página.FindVisualChildOfType<ImageElement>("imgTurno");

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
        CambiarInterfaz(TipoJugador.anfitrión, TipoProyectil.bola);
    }

    public void Activar(bool activar)
    {
        if(activar)
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
        pausa = !pausa;
        if (pausa)
            gridPausa.Visibility = Visibility.Visible;
        else
            gridPausa.Visibility = Visibility.Hidden;
    }

    private void EnClicReiniciar()
    {
        Content.Unload(SceneSystem.SceneInstance.RootScene);
        SceneSystem.SceneInstance.RootScene = Content.Load(escenaJuego);
    }

    private void EnClicOpciones()
    {

    }

    private void EnClicSalir()
    {
        Content.Unload(SceneSystem.SceneInstance.RootScene);
        SceneSystem.SceneInstance.RootScene = Content.Load(escenaMenú);
    }

    private void EnClicProyectil()
    {
        CambiarProyectil(controladorPartida.CambiarProyectil());
    }

    public void PausarInterfaz()
    {
        txtProyectil.Text = string.Empty;
        txtCantidadTurnos.Text = string.Empty;

        btnPausa.Visibility = Visibility.Hidden;
        gridProyectil.Visibility = Visibility.Hidden;
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
                imgProyectil.Source = ObtenerSprite(spriteBola);
                break;
            case TipoProyectil.metralla:
                txtProyectil.Text = "Metralla";
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

    public void MostrarGanador(TipoJugador jugador, int turno)
    {
        CambiarTurno(jugador);
        ActivarTurno(true);

        txtCantidadTurnos.Text = turno.ToString();
        gridGanador.Visibility = Visibility.Visible;
        gridProyectil.Visibility = Visibility.Hidden;
        btnPausa.Visibility = Visibility.Visible;

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