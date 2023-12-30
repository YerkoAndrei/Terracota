using Stride.Core.Mathematics;
using Stride.Engine;

namespace Terracota;

// 0    Estatua_0
// 1    Estatua_1
// 2    Estatua_2
// 3    Corto_0
// 4    Corto_1
// 5    Corto_2
// 6    Corto_3
// 7    Corto_4
// 8    Corto_5
// 9    Corto_6
// 10   Corto_7
// 11   Corto_8
// 12   Largo_0
// 13   Largo_1
// 14   Largo_2
// 15   Largo_3
// 16   Largo_4
// 17   Largo_5

public class ElementoBloqueFísico : StartupScript
{
    public int idBloque;

    public override void Start() { }

    public void Posicionar(Vector3 posición, Quaternion rotación)
    {
        Entity.Transform.Position = posición;
        Entity.Transform.Rotation = rotación;
    }
}
