using Stride.Core.Serialization;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;

namespace Terracota;
using static Constantes;

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
        página.FindVisualChildOfType<Button>("btnEstatua_0").Click += (sender, e) => { EnClicAgregaEstatua(0); };
        página.FindVisualChildOfType<Button>("btnEstatua_1").Click += (sender, e) => { EnClicAgregaEstatua(1); };
        página.FindVisualChildOfType<Button>("btnEstatua_2").Click += (sender, e) => { EnClicAgregaEstatua(2); };

        // Corto
        página.FindVisualChildOfType<Button>("btnCorto_0").Click += (sender, e) => { EnClicAgregaCorto(0); };
        página.FindVisualChildOfType<Button>("btnCorto_1").Click += (sender, e) => { EnClicAgregaCorto(1); };
        página.FindVisualChildOfType<Button>("btnCorto_2").Click += (sender, e) => { EnClicAgregaCorto(2); };
        página.FindVisualChildOfType<Button>("btnCorto_3").Click += (sender, e) => { EnClicAgregaCorto(3); };
        página.FindVisualChildOfType<Button>("btnCorto_4").Click += (sender, e) => { EnClicAgregaCorto(4); };
        página.FindVisualChildOfType<Button>("btnCorto_5").Click += (sender, e) => { EnClicAgregaCorto(5); };
        página.FindVisualChildOfType<Button>("btnCorto_6").Click += (sender, e) => { EnClicAgregaCorto(6); };
        página.FindVisualChildOfType<Button>("btnCorto_7").Click += (sender, e) => { EnClicAgregaCorto(7); };
        página.FindVisualChildOfType<Button>("btnCorto_8").Click += (sender, e) => { EnClicAgregaCorto(8); };

        // Largo
        página.FindVisualChildOfType<Button>("btnLargo_0").Click += (sender, e) => { EnClicAgregarLargo(0); };
        página.FindVisualChildOfType<Button>("btnLargo_1").Click += (sender, e) => { EnClicAgregarLargo(1); };
        página.FindVisualChildOfType<Button>("btnLargo_2").Click += (sender, e) => { EnClicAgregarLargo(2); };
        página.FindVisualChildOfType<Button>("btnLargo_3").Click += (sender, e) => { EnClicAgregarLargo(3); };
        página.FindVisualChildOfType<Button>("btnLargo_4").Click += (sender, e) => { EnClicAgregarLargo(4); };
        página.FindVisualChildOfType<Button>("btnLargo_5").Click += (sender, e) => { EnClicAgregarLargo(5); };

        // Ranuras
        página.FindVisualChildOfType<Button>("btnRanura_0").Click += (sender, e) => { EnClicGuardarRanura(0); };
        página.FindVisualChildOfType<Button>("btnRanura_1").Click += (sender, e) => { EnClicGuardarRanura(1); };
        página.FindVisualChildOfType<Button>("btnRanura_2").Click += (sender, e) => { EnClicGuardarRanura(2); };
        página.FindVisualChildOfType<Button>("btnRanura_3").Click += (sender, e) => { EnClicGuardarRanura(3); };
        página.FindVisualChildOfType<Button>("btnRanura_4").Click += (sender, e) => { EnClicGuardarRanura(4); };
        página.FindVisualChildOfType<Button>("btnRanura_5").Click += (sender, e) => { EnClicGuardarRanura(5); };
        página.FindVisualChildOfType<Button>("btnRanura_6").Click += (sender, e) => { EnClicGuardarRanura(6); };
        página.FindVisualChildOfType<Button>("btnRanura_7").Click += (sender, e) => { EnClicGuardarRanura(7); };
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
