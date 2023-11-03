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
    private Grid gridGuardar;

    public override void Start()
    {
        página = Entity.Get<UIComponent>().Page.RootElement;
        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("PanelOscuro"), EnClicGuardar);

        // Panel Guardar
        gridGuardar = página.FindVisualChildOfType<Grid>("Guardar");
        gridGuardar.Visibility = Visibility.Hidden;

        ConfigurarBotón(página.FindVisualChildOfType<Grid>("btnNuevo"), EnClicNuevaRanura);
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

        for (int i = 0; i < fortalezas.Count; i++)
        {
            var nuevaRanura = prefabRanura.InstantiateElement<Grid>("Ranura");
            ConfigurarRanura(nuevaRanura, i, fortalezas[i].ranura, fortalezas[i].miniatura, () => EnClicGuardarRanura(fortalezas[i].ranura), CargarFortalezas);
            padreRanuras.Height += (nuevaRanura.Height + 10);
            padreRanuras.Children.Add(nuevaRanura);
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

    private void EnClicNuevaRanura()
    {
        // PENDIENTE: mensaje ok
        var ranura = SistemaMemoria.CargarFortalezas().Count;
        if (controladorCreación.EnClicGuardar(ranura + 1))
        {
            // PENDIENTE: mensaje ok
            CargarFortalezas();
        }
    }

    private void EnClicGuardarRanura(int ranura)
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
        gridGuardar.Visibility = Visibility.Hidden;
    }
}
