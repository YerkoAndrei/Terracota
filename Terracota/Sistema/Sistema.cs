namespace Terracota;

public static class Sistema
{
    public static float RangoAleatorio(float min, float max)
    {
        System.Random random = new();
        double val = (random.NextDouble() * (max - min) + min);
        return (float)val;
    }
}
