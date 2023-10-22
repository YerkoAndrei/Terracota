using System.Collections.Generic;
using Stride.Engine;
using Newtonsoft.Json;
using System.IO;

namespace Terracota;
using static Constantes;

public class SistemaMemoria : StartupScript
{
    public override void Start()
    {

    }

    public static void GuardarFortaleza(ElementoBloque[] bloques)
    {
        var listaElementos = new List<Bloque>();
        for(int i=0; i < bloques.Length; i++)
        {
            var bloque = new Bloque(bloques[i].ObtenerTipo(), bloques[i].ObtenerPosición(), bloques[i].ObtenerRotación());
            listaElementos.Add(bloque);
        }

        var json = JsonConvert.SerializeObject(listaElementos);
        var encriptado = DesEncriptar(json);

        //File.WriteAllText("C:\\Users\\YerkoAndrei\\Desktop\\algo.txt", json);
    }
}
