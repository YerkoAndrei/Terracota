using System;
using Stride.Engine;
using Stride.Rendering.Lights;

namespace Terracota;
using static Constantes;

public class ControladorSombras : StartupScript
{
    public LightComponent luzPrincipal;

    private LightDirectional luz;

    public override void Start()
    {
        if (luz == null)
            ReferenciarLuz();

        ActualizarSombras();
    }

    private void ReferenciarLuz()
    {
        luz = luzPrincipal.Type as LightDirectional;
    }

    public void ActualizarSombras()
    {
        if (luz == null)
            ReferenciarLuz();

        var nivel = (NivelesConfiguración)Enum.Parse(typeof(NivelesConfiguración), SistemaMemoria.ObtenerConfiguración(Configuraciones.sombras));
        switch (nivel)
        {
            case NivelesConfiguración.bajo:
                luz.Shadow.Enabled = false;
                break;
            case NivelesConfiguración.medio:
                luz.Shadow.Enabled = true;
                luz.Shadow.Size = LightShadowMapSize.Medium;
                break;
            case NivelesConfiguración.alto:
                luz.Shadow.Enabled = true;
                luz.Shadow.Size = LightShadowMapSize.XLarge;
                break;
        }
    }
}
