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
    public UILibrary prefabRanura;
    public UrlReference<Scene> escenaMenú;

    private UIElement página;
    private Grid gridFortalezas;

    public override void Start()
    {
        página = Entity.Get<UIComponent>().Page.RootElement;
        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("PanelOscuro"), EnClicFortalezas);

        // Panel Guardar
        gridFortalezas = página.FindVisualChildOfType<Grid>("Fortalezas");
        gridFortalezas.Visibility = Visibility.Hidden;

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnNuevo"), EnClicGuardarNueva);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnVolver"), EnClicFortalezas);

        // Guardado
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnFortalezas"), EnClicFortalezas);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnReiniciar"), EnClicReiniciarPosiciones);
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnSalir"), EnClicSalir);

        // Cámara
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnIzquierda"), () => EnClicMoverCámara(false));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnDerecha"), () => EnClicMoverCámara(true));
        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnGirar"), EnClicGirarPieza);

        // Instanciando fortalezas guardadas
        CargarFortalezas();
    }

    private void CargarFortalezas()
    {
        // Instanciando fortalezas guardadas
        var padreRanuras = página.FindVisualChildOfType<UniformGrid>("Ranuras");
        var vacío = página.FindVisualChildOfType<ImageElement>("imgVacío");
        vacío.Visibility = Visibility.Hidden;

        var fortalezas = SistemaMemoria.CargarFortalezas();
        if (fortalezas.Count == 0)
            vacío.Visibility = Visibility.Visible;

        // Limpieza
        padreRanuras.Children.Clear();

        // Scroll
        padreRanuras.Rows = fortalezas.Count;
        padreRanuras.Height = 0;

        // Se usa una variable temporal para las acciones
        for (int i = 0; i < fortalezas.Count; i++)
        {
            var nuevaRanura = prefabRanura.InstantiateElement<Grid>("Ranura");
            var fortalezaTemp = fortalezas[i];

            ConfigurarRanura(nuevaRanura, i, fortalezaTemp.ranura, fortalezaTemp.miniatura,
                () => EnClicGuardar(fortalezaTemp.ranura),
                () => EnClicCargarFortaleza(fortalezaTemp.ranura), 
                CargarFortalezas);
            padreRanuras.Height += (nuevaRanura.Height + 10);
            padreRanuras.Children.Add(nuevaRanura);
        }
    }

    private void EnClicCargarFortaleza(int ranura)
    {
        controladorCreación.EnClicCargarFortaleza(ranura);
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

    private void EnClicFortalezas()
    {
        if (gridFortalezas.Visibility == Visibility.Visible)
        {
            gridFortalezas.Visibility = Visibility.Hidden;
            controladorCreación.AbrirMenú(false);
        }
        else
        {
            gridFortalezas.Visibility = Visibility.Visible;
            controladorCreación.AbrirMenú(true);
        }
    }

    private void EnClicGuardarNueva()
    {
        // PENDIENTE: mensaje ok
        var ranura = SistemaMemoria.CargarFortalezas().Count;
        if (controladorCreación.EnClicGuardar(ranura + 1))
        {
            // PENDIENTE: mensaje ok
            CargarFortalezas();
        }
    }

    private void EnClicGuardar(int ranura)
    {
        // PENDIENTE: pop up sobreescribir
        if (controladorCreación.EnClicGuardar(ranura))
        {
            // PENDIENTE: mensaje ok
            CargarFortalezas();
        }
    }

    private void EnClicSalir()
    {
        Content.Unload(SceneSystem.SceneInstance.RootScene);
        SceneSystem.SceneInstance.RootScene = Content.Load(escenaMenú);
    }

    public void CerrarPanelGuardar()
    {
        gridFortalezas.Visibility = Visibility.Hidden;
    }
}
