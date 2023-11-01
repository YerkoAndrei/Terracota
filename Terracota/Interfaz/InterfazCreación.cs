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

        gridGuardar = página.FindVisualChildOfType<Grid>("Guardar");
        gridGuardar.Visibility = Visibility.Hidden;
        página.FindVisualChildOfType<Button>("PanelOscuro").Click += (sender, e) =>     { EnClicGuardar(); };
        página.FindVisualChildOfType<Button>("btnVolver").Click += (sender, e) =>       { EnClicGuardar(); };

        // Guardado
        página.FindVisualChildOfType<Button>("btnGuardar").Click += (sender, e) =>      { EnClicGuardar(); };
        página.FindVisualChildOfType<Button>("btnReiniciar").Click += (sender, e) =>    { EnClicReiniciarPosiciones(); };
        página.FindVisualChildOfType<Button>("btnSalir").Click += (sender, e) =>        { EnClicSalir(); };

        // Cámara
        página.FindVisualChildOfType<Button>("btnIzquierda").Click += (sender, e) =>    { EnClicMoverCámara(false); };
        página.FindVisualChildOfType<Button>("btnDerecha").Click += (sender, e) =>      { EnClicMoverCámara(true); };
        página.FindVisualChildOfType<Button>("btnGirar").Click += (sender, e) =>        { EnClicGirarPieza(); };

        // Estatua
        var estatuas = página.FindVisualChildOfType<UniformGrid>("Estatuas").FindVisualChildrenOfType<Button>().ToArray();
        for (int i = 0; i < estatuas.Length; i++)
        {
            //estatuas[i] = ConfigurarBotón(estatuas[i]);
            estatuas[i].Click += (sender, e) => { EnClicAgregaEstatua(i); };
        }

        // Cortos
        var cortos = página.FindVisualChildOfType<UniformGrid>("Cortos").FindVisualChildrenOfType<Button>().ToArray();
        for (int i = 0; i < cortos.Length; i++)
        {
            //cortos[i] = ConfigurarBotón(cortos[i]);
            cortos[i].Click += (sender, e) => { EnClicAgregaCorto(i); };
        }

        // Largos
        var largos = página.FindVisualChildOfType<UniformGrid>("Largos").FindVisualChildrenOfType<Button>().ToArray();
        for (int i = 0; i < largos.Length; i++)
        {
            //largos[i] = ConfigurarBotón(largos[i]);
            largos[i].Click += (sender, e) => { EnClicAgregarLargo(i); };
        }

        // Ranuras
        var ranuras = página.FindVisualChildOfType<UniformGrid>("Ranuras").FindVisualChildrenOfType<Button>().ToArray();
        for(int i=0; i< ranuras.Length; i++)
        {
            //ranuras[i] = ConfigurarBotón(ranuras[i]);
            ranuras[i].Click += (sender, e) => { EnClicGuardarRanura(i+ 1); };
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
