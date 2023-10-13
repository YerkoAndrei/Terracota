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
