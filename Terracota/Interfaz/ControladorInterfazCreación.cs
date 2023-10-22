using Stride.Core.Serialization;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;

namespace Terracota;

public class ControladorInterfazCreación : StartupScript
{
    public ControladorCreación controladorCreación;
    public UrlReference<Scene> escenaMenú;

    public override void Start()
    {
        var página = Entity.Get<UIComponent>().Page.RootElement;

        // Guardado
        var btnSalirGuardar = página.FindVisualChildOfType<Button>("btnSalirGuardar");
        btnSalirGuardar.Click += (sender, e) => { SalirGuardar(); };

        var btnSalir = página.FindVisualChildOfType<Button>("btnSalir");
        btnSalir.Click += (sender, e) => { Salir(); };

        // Cámara
        var btnIzquierda = página.FindVisualChildOfType<Button>("btnIzquierda");
        btnIzquierda.Click += (sender, e) => { MoverCámara(false); };

        var btnDerecha = página.FindVisualChildOfType<Button>("btnDerecha");
        btnDerecha.Click += (sender, e) => { MoverCámara(true); };


        // Corto
        var btnCorto_0 = página.FindVisualChildOfType<Button>("btnCorto_0");
        btnCorto_0.Click += (sender, e) => { AgregaCorto(0); };

        var btnCorto_1 = página.FindVisualChildOfType<Button>("btnCorto_1");
        btnCorto_1.Click += (sender, e) => { AgregaCorto(1); };

        var btnCorto_2 = página.FindVisualChildOfType<Button>("btnCorto_2");
        btnCorto_2.Click += (sender, e) => { AgregaCorto(2); };

        var btnCorto_3 = página.FindVisualChildOfType<Button>("btnCorto_3");
        btnCorto_3.Click += (sender, e) => { AgregaCorto(3); };

        var btnCorto_4 = página.FindVisualChildOfType<Button>("btnCorto_4");
        btnCorto_4.Click += (sender, e) => { AgregaCorto(4); };

        var btnCorto_5 = página.FindVisualChildOfType<Button>("btnCorto_5");
        btnCorto_5.Click += (sender, e) => { AgregaCorto(5); };

        var btnCorto_6 = página.FindVisualChildOfType<Button>("btnCorto_6");
        btnCorto_6.Click += (sender, e) => { AgregaCorto(6); };

        var btnCorto_7 = página.FindVisualChildOfType<Button>("btnCorto_7");
        btnCorto_7.Click += (sender, e) => { AgregaCorto(7); };

        var btnCorto_8 = página.FindVisualChildOfType<Button>("btnCorto_8");
        btnCorto_8.Click += (sender, e) => { AgregaCorto(8); };

        // Largo
        var btnLargo_0 = página.FindVisualChildOfType<Button>("btnLargo_0");
        btnLargo_0.Click += (sender, e) => { AgregarLargo(0); };

        var btnLargo_1 = página.FindVisualChildOfType<Button>("btnLargo_1");
        btnLargo_1.Click += (sender, e) => { AgregarLargo(1); };

        var btnLargo_2 = página.FindVisualChildOfType<Button>("btnLargo_2");
        btnLargo_2.Click += (sender, e) => { AgregarLargo(2); };

        var btnLargo_3 = página.FindVisualChildOfType<Button>("btnLargo_3");
        btnLargo_3.Click += (sender, e) => { AgregarLargo(3); };

        var btnLargo_4 = página.FindVisualChildOfType<Button>("btnLargo_4");
        btnLargo_4.Click += (sender, e) => { AgregarLargo(4); };

        var btnLargo_5 = página.FindVisualChildOfType<Button>("btnLargo_5");
        btnLargo_5.Click += (sender, e) => { AgregarLargo(5); };
    }

    private void AgregaCorto(int corto)
    {
        controladorCreación.AgregaCorto(corto);
    }

    private void AgregarLargo(int largo)
    {
        controladorCreación.AgregarLargo(largo);
    }

    private void MoverCámara(bool derecha)
    {
        controladorCreación.MoverCámara(derecha);
    }

    private void SalirGuardar()
    {
        controladorCreación.SalirGuardar();
    }

    private void Salir()
    {
        Content.Unload(SceneSystem.SceneInstance.RootScene);
        SceneSystem.SceneInstance.RootScene = Content.Load(escenaMenú);
    }
}
