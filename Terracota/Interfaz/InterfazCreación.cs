using Stride.Core.Serialization;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;
using System.Linq;

namespace Terracota;
using static Constantes;
using static Sistema;

public class InterfazCreación : StartupScript
{
    public ControladorCreación controladorCreación;
    public UrlReference<Scene> escenaMenú;

    private Grid gridGuardar;

    public override void Start()
    {
        var página = Entity.Get<UIComponent>().Page.RootElement;
        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("PanelOscuro"), EnClicGuardar);

        // Panel Guardar
        gridGuardar = página.FindVisualChildOfType<Grid>("Guardar");
        gridGuardar.Visibility = Visibility.Hidden;

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnVolver"), EnClicGuardar);

        // Guardado
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnGuardar"), EnClicGuardar);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnReiniciar"), EnClicReiniciarPosiciones);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSalir"), EnClicSalir);

        // Cámara
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnIzquierda"), () => EnClicMoverCámara(false));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnDerecha"), () => EnClicMoverCámara(true));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnGirar"), EnClicGirarPieza);

        // Estatua
        var estatuas = página.FindVisualChildOfType<UniformGrid>("Estatuas").FindVisualChildrenOfType<Grid>().ToArray();
        for (int i = 0; i < estatuas.Length; i++)
        {
            ConfigurarBotón(estatuas[i], () => EnClicAgregaEstatua(i));
        }

        // Cortos
        var cortos = página.FindVisualChildOfType<UniformGrid>("Cortos").FindVisualChildrenOfType<Grid>().ToArray();
        for (int i = 0; i < cortos.Length; i++)
        {
            ConfigurarBotón(cortos[i], () => EnClicAgregaCorto(i));
        }

        // Largos
        var largos = página.FindVisualChildOfType<UniformGrid>("Largos").FindVisualChildrenOfType<Grid>().ToArray();
        for (int i = 0; i < largos.Length; i++)
        {
            ConfigurarBotón(largos[i], () => EnClicAgregarLargo(i));
        }

        // Ranuras
        var ranuras = página.FindVisualChildOfType<UniformGrid>("Ranuras").FindVisualChildrenOfType<Grid>().ToArray();
        for(int i=0; i< ranuras.Length; i++)
        {
            ConfigurarBotón(estatuas[i], () => EnClicGuardarRanura(i+1));
        }
    }

    private void EnClicAgregaEstatua(int estatua)
    {
        controladorCreación.AgregarBloque(TipoBloque.estatua, estatua);
    }

    private void EnClicAgregaCorto(int corto)
    {
        controladorCreación.AgregarBloque(TipoBloque.corto, corto);
    }

    private void EnClicAgregarLargo(int largo)
    {
        controladorCreación.AgregarBloque(TipoBloque.largo, largo);
    }

    private void EnClicMoverCámara(bool derecha)
    {
        controladorCreación.EnClicMoverCámara(derecha);
    }

    private void EnClicGirarPieza()
    {
        // PENDIENTE: móvil
        controladorCreación.EnClicGirarPieza();
    }

    private void EnClicReiniciarPosiciones()
    {
        controladorCreación.EnClicReiniciarPosiciones();
    }

    private void EnClicGuardar()
    {
        if (gridGuardar.Visibility == Visibility.Visible)
            gridGuardar.Visibility = Visibility.Hidden;
        else
            gridGuardar.Visibility = Visibility.Visible;
    }

    private void EnClicGuardarRanura(int ranura)
    {
        controladorCreación.EnClicGuardar(ranura);
    }

    private void EnClicSalir()
    {
        Content.Unload(SceneSystem.SceneInstance.RootScene);
        SceneSystem.SceneInstance.RootScene = Content.Load(escenaMenú);
    }

    public void CerrarPanelGuardar()
    {
        gridGuardar.Visibility = Visibility.Hidden;
    }
}
