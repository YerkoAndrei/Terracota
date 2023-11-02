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

    // Solo encuenta en primeros hijos
    public static Entity EncontrarEntidad(SceneInstance escena, string nombre)
    {
        var entidades = escena.RootScene.Entities;
        var entidad = entidades.FirstOrDefault(e => e.Name == nombre);

        if (entidad != null)
            return entidad;
        else
        {
            for(int i=0; i< entidades.Count; i++)
            {
                entidades[i].FindChild(nombre);

                if(entidades[i] != null)
                    return entidades[i];
            }
            return null;
        }
    }

    public static ISpriteProvider ObtenerSprite(Texture textura)
    {
        var sprite = new SpriteFromTexture();
        sprite.Texture = textura;

        return sprite;
    }

    public static void ConfigurarBotón(Grid grid, Action action)
    {
        try
        {
            // Busca contenido dentro del "grid botón"
            var imagen = grid.FindVisualChildOfType<ImageElement>("img");
            var botón = grid.FindVisualChildOfType<Button>("btn");

            // En clic
            botón.Click += (s, a) => { action.Invoke(); };
        
            // Cambios color
            botón.MouseOverStateChanged += (s, a) =>
            {
                switch (a.NewValue)
                {
                    case MouseOverState.MouseOverElement:
                        imagen.Color = colorEnCursor;
                        break;
                    case MouseOverState.MouseOverNone:
                        imagen.Color = colorNormal;
                        break;
                }
            };
            botón.TouchDown += (s, a) =>
            {
                imagen.Color = colorEnClic;
            };
            botón.TouchUp += (s, a) =>
            {
                imagen.Color = colorEnCursor;
            };
        }
        catch
        {
            Console.WriteLine("Error botón: " + grid.Name);
        }
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
}
