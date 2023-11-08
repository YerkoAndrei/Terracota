using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;

namespace Terracota;
using static Constantes;
using static Sistema;

public class InterfazCreación : StartupScript
{
    public ControladorCreación controladorCreación;
    public UILibrary prefabRanura;

    private UIElement página;
    private Grid gridFortalezas;
    private Grid popup;

    public override void Start()
    {
        página = Entity.Get<UIComponent>().Page.RootElement;
        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("PanelOscuro"), EnClicFortalezas);

        // Popup
        popup = página.FindVisualChildOfType<Grid>("Popup");
        popup.Visibility = Visibility.Hidden;

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

        var fortalezas = SistemaMemoria.CargarFortalezas(true);
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

            // Fortaleza vacía
            if (i == 0)
            {
                ConfigurarRanuraVacíaCreación(nuevaRanura, i, fortalezaTemp.ranura, fortalezaTemp.miniatura,
                    () => EnClicCargarFortaleza(fortalezaTemp.ranura));
            }
            else
            {
                ConfigurarRanuraCreación(nuevaRanura, i, fortalezaTemp.ranura, fortalezaTemp.miniatura,
                    () => EnClicGuardar(fortalezaTemp.ranura),
                    () => EnClicCargarFortaleza(fortalezaTemp.ranura),
                    () => EnClicEliminar(fortalezaTemp.ranura));
            }
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
        if (controladorCreación.EnClicGuardar(SistemaMemoria.ObtenerRanuraMásAlta() + 1))
        {
            CargarFortalezas();
            MostrarMensaje("OK");
        }
        else
            MostrarMensaje("Todas las piezas deben estar listas.");
    }

    private void EnClicGuardar(int ranura)
    {
        var pregunta = string.Format("¿Desea sobreescribir fortaleza {0}?", ranura.ToString());
        ConfigurarPop(popup, pregunta, () =>
        {
            // Sí
            if (controladorCreación.EnClicGuardar(ranura))
            {
                CargarFortalezas();
                popup.Visibility = Visibility.Hidden;
                MostrarMensaje("OK");
            }
            else
            {
                MostrarMensaje("Error");
                popup.Visibility = Visibility.Hidden;
            }
        }, () =>
        {
            // No
            popup.Visibility = Visibility.Hidden;
        });
        popup.Visibility = Visibility.Visible;
    }

    private void EnClicEliminar(int ranura)
    {
        var pregunta = string.Format("¿Desea eliminar fortaleza {0}?", ranura.ToString());
        ConfigurarPop(popup, pregunta, () =>
        {
            // Sí
            if(SistemaMemoria.EliminarFortaleza(ranura))
            {
                CargarFortalezas();
                popup.Visibility = Visibility.Hidden;
                MostrarMensaje("OK");
            }
            else
            {
                MostrarMensaje("Error");
                popup.Visibility = Visibility.Hidden;
            }
        }, () =>
        {
            // No
            popup.Visibility = Visibility.Hidden;
        });
        popup.Visibility = Visibility.Visible;
    }

    private void MostrarMensaje(string mensaje)
    {
        // PENDIENTE: mensaje ok
    }

    private void EnClicSalir()
    {
        SistemaEscenas.CambiarEscena(Escenas.menú);
    }

    public void CerrarPanelGuardar()
    {
        gridFortalezas.Visibility = Visibility.Hidden;
    }
}
