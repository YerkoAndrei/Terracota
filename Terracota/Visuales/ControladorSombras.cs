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

        var nivel = (Calidades)Enum.Parse(typeof(Calidades), SistemaMemoria.ObtenerConfiguración(Configuraciones.sombras));
        switch (nivel)
        {
            case Calidades.bajo:
                luz.Shadow.Enabled = false;
                break;
            case Calidades.medio:
                luz.Shadow.Enabled = true;
                luz.Shadow.Size = LightShadowMapSize.Medium;
                break;
            case Calidades.alto:
                luz.Shadow.Enabled = true;
                luz.Shadow.Size = LightShadowMapSize.XLarge;
                break;
        }
    }
}
