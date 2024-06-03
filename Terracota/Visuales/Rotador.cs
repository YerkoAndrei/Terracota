using Stride.Engine;
using Stride.Core.Mathematics;

namespace Terracota;

public class Rotador : SyncScript
{
    public float velocidadY;

    public override void Update()
    {
        Entity.Transform.Rotation *= Quaternion.RotationY(velocidadY * (float)Game.UpdateTime.WarpElapsed.TotalSeconds);
    }
}
