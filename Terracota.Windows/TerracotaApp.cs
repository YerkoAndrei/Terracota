using Stride.Engine;

namespace Terracota
{
    class TerracotaApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
