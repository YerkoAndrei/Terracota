using System;
using System.Linq;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering.Sprites;
using Stride.UI.Controls;

namespace Terracota;

public static class Sistema
{
    public static float RangoAleatorio(float min, float max)
    {
        System.Random random = new();
        double val = (random.NextDouble() * (max - min) + min);
        return (float)val;
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

    public static void CambiarImagenBotón(Button botón, Texture textura)
    {
        var sprite = ObtenerSprite(textura);

        // PENDIENTE: Multiplicar por colores
        botón.NotPressedImage = sprite;
        botón.PressedImage = sprite;
        botón.MouseOverImage = sprite;
    }

    public static ISpriteProvider ObtenerSprite(Texture textura)
    {
        var sprite = new SpriteFromTexture();
        sprite.Texture = textura;

        return sprite;
    }
}
