using Stride.Core.Serialization;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;

namespace Terracota;
using static Constantes;
using static Terracota.Constantes;

public class ControladorInterfazCreación : StartupScript
{
    public ControladorCreación controladorCreación;
    public UrlReference<Scene> escenaMenú;

    public override void Start()
    {
        var página = Entity.Get<UIComponent>().Page.RootElement;

        // Guardado
        var btnGuardar = página.FindVisualChildOfType<Button>("btnGuardar");
        btnGuardar.Click += (sender, e) => { EnClicGuardar(); };

        var btnReiniciar = página.FindVisualChildOfType<Button>("btnReiniciar");
        btnReiniciar.Click += (sender, e) => { EnClicReiniciarPosiciones(); };

        var btnSalir = página.FindVisualChildOfType<Button>("btnSalir");
        btnSalir.Click += (sender, e) => { EnClicSalir(); };

        // Cámara
        var btnIzquierda = página.FindVisualChildOfType<Button>("btnIzquierda");
        btnIzquierda.Click += (sender, e) => { EnClicMoverCámara(false); };

        var btnDerecha = página.FindVisualChildOfType<Button>("btnDerecha");
        btnDerecha.Click += (sender, e) => { EnClicMoverCámara(true); };

        var btnGirar = página.FindVisualChildOfType<Button>("btnGirar");
        btnGirar.Click += (sender, e) => { EnClicGirarPieza(); };

        // Estatua
        var btnEstatua_0 = página.FindVisualChildOfType<Button>("btnEstatua_0");
        btnEstatua_0.Click += (sender, e) => { EnClicAgregaEstatua(0); };

        var btnEstatua_1 = página.FindVisualChildOfType<Button>("btnEstatua_1");
        btnEstatua_1.Click += (sender, e) => { EnClicAgregaEstatua(1); };

        var btnEstatua_2 = página.FindVisualChildOfType<Button>("btnEstatua_2");
        btnEstatua_2.Click += (sender, e) => { EnClicAgregaEstatua(2); };

        // Corto
        var btnCorto_0 = página.FindVisualChildOfType<Button>("btnCorto_0");
        btnCorto_0.Click += (sender, e) => { EnClicAgregaCorto(0); };

        var btnCorto_1 = página.FindVisualChildOfType<Button>("btnCorto_1");
        btnCorto_1.Click += (sender, e) => { EnClicAgregaCorto(1); };

        var btnCorto_2 = página.FindVisualChildOfType<Button>("btnCorto_2");
        btnCorto_2.Click += (sender, e) => { EnClicAgregaCorto(2); };

        var btnCorto_3 = página.FindVisualChildOfType<Button>("btnCorto_3");
        btnCorto_3.Click += (sender, e) => { EnClicAgregaCorto(3); };

        var btnCorto_4 = página.FindVisualChildOfType<Button>("btnCorto_4");
        btnCorto_4.Click += (sender, e) => { EnClicAgregaCorto(4); };

        var btnCorto_5 = página.FindVisualChildOfType<Button>("btnCorto_5");
        btnCorto_5.Click += (sender, e) => { EnClicAgregaCorto(5); };

        var btnCorto_6 = página.FindVisualChildOfType<Button>("btnCorto_6");
        btnCorto_6.Click += (sender, e) => { EnClicAgregaCorto(6); };

        var btnCorto_7 = página.FindVisualChildOfType<Button>("btnCorto_7");
        btnCorto_7.Click += (sender, e) => { EnClicAgregaCorto(7); };

        var btnCorto_8 = página.FindVisualChildOfType<Button>("btnCorto_8");
        btnCorto_8.Click += (sender, e) => { EnClicAgregaCorto(8); };

        // Largo
        var btnLargo_0 = página.FindVisualChildOfType<Button>("btnLargo_0");
        btnLargo_0.Click += (sender, e) => { EnClicAgregarLargo(0); };

        var btnLargo_1 = página.FindVisualChildOfType<Button>("btnLargo_1");
        btnLargo_1.Click += (sender, e) => { EnClicAgregarLargo(1); };

        var btnLargo_2 = página.FindVisualChildOfType<Button>("btnLargo_2");
        btnLargo_2.Click += (sender, e) => { EnClicAgregarLargo(2); };

        var btnLargo_3 = página.FindVisualChildOfType<Button>("btnLargo_3");
        btnLargo_3.Click += (sender, e) => { EnClicAgregarLargo(3); };

        var btnLargo_4 = página.FindVisualChildOfType<Button>("btnLargo_4");
        btnLargo_4.Click += (sender, e) => { EnClicAgregarLargo(4); };

        var btnLargo_5 = página.FindVisualChildOfType<Button>("btnLargo_5");
        btnLargo_5.Click += (sender, e) => { EnClicAgregarLargo(5); };
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

    private void EnClicGuardar()
    {
        controladorCreación.EnClicGuardar();
    }

    private void EnClicReiniciarPosiciones()
    {
        controladorCreación.EnClicReiniciarPosiciones();
    }

    private void EnClicSalir()
    {
        Content.Unload(SceneSystem.SceneInstance.RootScene);
        SceneSystem.SceneInstance.RootScene = Content.Load(escenaMenú);
    }
}
