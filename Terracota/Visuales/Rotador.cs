using Stride.Engine;
using Stride.Core.Mathematics;

namespace Terracota;

public class Rotador : SyncScript
{
    public float ánguloY;

    public override void Update()
    {
        Entity.Transform.Rotation *= Quaternion.RotationY(ánguloY * 10 * (float)Game.UpdateTime.WarpElapsed.TotalSeconds);
    }
}
