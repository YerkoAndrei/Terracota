using System;
using System.Linq;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering.Sprites;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;

namespace Terracota;
using static Constantes;

public static class Sistema
{
    public static float RangoAleatorio(float min, float max)
    {
        var aleatorio = new Random();
        double val = (aleatorio.NextDouble() * (max - min) + min);
        return (float)val;
    }

    public static Vector3 EulerAleatorio()
    {
        var aleatorio = new Random();

        // RotationEulerXYZ está en radianes
        var x = MathUtil.DegreesToRadians(aleatorio.Next(0, 360));
        var y = MathUtil.DegreesToRadians(aleatorio.Next(0, 360));
        var z = MathUtil.DegreesToRadians(aleatorio.Next(0, 360));
        return new Vector3(x, y, z);
    }

    public static ISpriteProvider ObtenerSprite(Texture textura)
    {
        var sprite = new SpriteFromTexture();
        sprite.Texture = textura;

        return sprite;
    }

    public static void ConfigurarBotón(Grid grid, Action action)
    {
        // Busca contenido dentro del "grid botón"
        var imagen = grid.FindVisualChildOfType<ImageElement>("img");
        var botón = grid.FindVisualChildOfType<Button>("btn");

        // En clic
        botón.Click += (s, a) => { action.Invoke(); };

        // Cambios color
        var colorBase = imagen.Color;
        botón.MouseOverStateChanged += (s, a) =>
        {
            switch (a.NewValue)
            {
                case MouseOverState.MouseOverElement:
                    imagen.Color = colorBase * colorEnCursor;
                    break;
                case MouseOverState.MouseOverNone:
                    imagen.Color = colorBase * colorNormal;
                    break;
            }
        };
        botón.TouchDown += (s, a) =>
        {
            imagen.Color = colorBase * colorEnClic;
        };
        botón.TouchUp += (s, a) =>
        {
            imagen.Color = colorBase * colorEnCursor;
        };
    }

    public static void ConfigurarBotónConImagen(Button botón, ImageElement imagen, Action action)
    {
        // En clic
        botón.Click += (sender, e) => { action.Invoke(); };

        // Cambios color
        botón.MouseOverStateChanged += (sender, args) =>
        {
            switch (args.NewValue)
            {
                case MouseOverState.MouseOverElement:
                    imagen.Color = colorEnCursor;
                    break;
                case MouseOverState.MouseOverNone:
                    imagen.Color = colorNormal;
                    break;
            }
        };
        botón.TouchDown += (sender, args) =>
        {
            imagen.Color = colorEnClic;
        };
        botón.TouchUp += (sender, args) =>
        {
            imagen.Color = colorEnCursor;
        };
    }

    public static void ConfigurarBotónOculto(Button botón, Action action)
    {
        botón.Click += (sender, e) => { action.Invoke(); };
    }

    public static void ConfigurarRanuraCreación(Grid grid, int fila, int ranura, string miniaturaB64, Action enCargar, Action enSobreescribir, Action enElminar)
    {
        // Busca contenido dentro del "grid botón"
        ConfigurarBotón(grid, enCargar);
        var miniatura = grid.FindVisualChildOfType<ImageElement>("Miniatura");
        var texto = grid.FindVisualChildOfType<TextBlock>("txt");

        // Botones
        var btnEliminar = grid.FindVisualChildOfType<Grid>("btnEliminar");
        ConfigurarBotón(btnEliminar, enElminar);

        var btnSobreescribir = grid.FindVisualChildOfType<Grid>("btnSobreescribir");
        ConfigurarBotón(btnSobreescribir, enSobreescribir);

        // Visual
        texto.Text = ranura.ToString();
        grid.SetGridRow(fila);

        // Miniatura
        //miniatura.Source = ConvertirB64(miniaturaB64);
    }

    public static void ConfigurarRanuraVacíaCreación(Grid grid, int fila, int ranura, string miniaturaB64, Action enCargar)
    {
        // Busca contenido dentro del "grid botón"
        ConfigurarBotón(grid, enCargar);
        var miniatura = grid.FindVisualChildOfType<ImageElement>("Miniatura");
        var texto = grid.FindVisualChildOfType<TextBlock>("txt");

        // Botones
        grid.FindVisualChildOfType<Grid>("btnEliminar").Visibility = Visibility.Hidden;
        grid.FindVisualChildOfType<Grid>("btnSobreescribir").Visibility = Visibility.Hidden;

        // Visual
        texto.Text = ranura.ToString();
        grid.SetGridRow(fila);

        // Miniatura
        //miniatura.Source = ConvertirB64(miniaturaB64);
    }

    public static void ConfigurarRanuraElección(Grid grid, int fila, int ranura, string miniaturaB64, Action enClic)
    {
        // Busca contenido dentro del "grid botón"
        ConfigurarBotón(grid, enClic);
        var miniatura = grid.FindVisualChildOfType<ImageElement>("Miniatura");
        var texto = grid.FindVisualChildOfType<TextBlock>("txt");

        // Visual
        texto.Text = ranura.ToString();
        grid.SetGridRow(fila);

        // Miniatura
        //miniatura.Source = ConvertirB64(miniaturaB64);
    }

    private static ISpriteProvider ConvertirB64(string B64)
    {
        var sprite = new SpriteFromTexture();
        // PENDIENTE: miniatura

        return sprite;
    }

    public static void ConfigurarPop(Grid grid, string pregunta, Action enClicSí, Action enClicNo)
    {
        grid.FindVisualChildOfType<TextBlock>("txtPregunta").Text = pregunta;
        ConfigurarBotón(grid.FindVisualChildOfType<Grid>("btnSí"), enClicSí);
        ConfigurarBotón(grid.FindVisualChildOfType<Grid>("btnNo"), enClicNo);
    }
}
