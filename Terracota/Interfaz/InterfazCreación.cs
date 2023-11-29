using System.Threading.Tasks;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Events;
using Stride.UI.Panels;

namespace Terracota;
using static Constantes;
using static Sistema;

public class InterfazCreación : StartupScript
{
    public ControladorCreación controladorCreación;
    public UILibrary prefabRanura;
    public UILibrary prefabPopup;

    private UIElement página;
    private TextBlock txtMensaje;
    private EditText txtNuevoNombre;
    private Grid gridFortalezas;
    private Grid popups;
    private Grid btnNuevo;
    private Grid btnVolver;

    // Animación
    private Grid animFortalezas;
    private bool animando;

    public override void Start()
    {
        página = Entity.Get<UIComponent>().Page.RootElement;
        ConfigurarBotónOculto(página.FindVisualChildOfType<Button>("PanelOscuro"), EnClicFortalezas);

        // Popup
        popups = página.FindVisualChildOfType<Grid>("Popups");

        // Textos
        txtMensaje = página.FindVisualChildOfType<TextBlock>("txtMensaje");
        txtNuevoNombre = página.FindVisualChildOfType<EditText>("txtNuevoNombre");
        txtNuevoNombre.TextChanged += VerificarFuente;

        txtMensaje.Text = string.Empty;
        txtNuevoNombre.Text = string.Empty;

        // Panel Guardar
        gridFortalezas = página.FindVisualChildOfType<Grid>("Fortalezas");
        animFortalezas = página.FindVisualChildOfType<Grid>("PanelFortalezas");
        gridFortalezas.Visibility = Visibility.Hidden;

        btnNuevo = página.FindVisualChildOfType<Grid>("btnNuevo");
        btnVolver = página.FindVisualChildOfType<Grid>("btnVolver");
        ConfigurarBotón(btnNuevo, EnClicGuardarNueva);
        ConfigurarBotón(btnVolver, EnClicFortalezas);

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
        var fortalezas = SistemaMemoria.CargarFortalezas(true);

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
                ConfigurarRanuraVacíaCreación(nuevaRanura, i, fortalezaTemp.Nombre,
                    () => EnClicCargarFortaleza(fortalezaTemp.Nombre));
            }
            else
            {
                ConfigurarRanuraCreación(nuevaRanura, i, fortalezaTemp.Nombre,
                    () => EnClicCargarFortaleza(fortalezaTemp.Nombre),
                    () => EnClicSobreescribir(fortalezaTemp.Nombre),
                    () => EnClicEliminar(fortalezaTemp.Nombre));
            }
            padreRanuras.Height += (nuevaRanura.Height + 10);
            padreRanuras.Children.Add(nuevaRanura);
        }
    }

    private void EnClicCargarFortaleza(string nombre)
    {
        controladorCreación.EnClicCargarFortaleza(nombre);
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
        if (animando)
            return;

        animando = true;
        BloquearBotón(btnNuevo, true);
        BloquearBotón(btnVolver, true);
        if (gridFortalezas.Visibility == Visibility.Visible)
        {
            SistemaAnimación.AnimarElemento(animFortalezas, 0.2f, false, Direcciones.arriba, TipoCurva.rápida, ()=>
            {
                gridFortalezas.Visibility = Visibility.Hidden;
                controladorCreación.AbrirMenú(false);
                animando = false;
            });
        }
        else
        {
            controladorCreación.AbrirMenú(true);
            gridFortalezas.Visibility = Visibility.Visible;
            SistemaAnimación.AnimarElemento(animFortalezas, 0.2f, true, Direcciones.arriba, TipoCurva.rápida, () =>
            {
                animando = false;
                BloquearBotón(btnNuevo, false);
                BloquearBotón(btnVolver, false);
            });
        }
    }

    private void EnClicGuardarNueva()
    {
        var nombre = txtNuevoNombre.Text.ToUpper().Trim();
        if (string.IsNullOrEmpty(nombre))
        {
            MostrarMensaje("Nombre no puede estar vacío");
            return;
        }

        if (!controladorCreación.VerificarPosibleGuardar())
        {
            MostrarMensaje("Falta colocar piezas");
            return;
        }

        if (controladorCreación.EnClicGuardar(nombre, false))
        {
            txtNuevoNombre.Text = string.Empty;
            CargarFortalezas();
        }
        else
            MostrarMensaje("Error al guardar");
    }

    private void EnClicSobreescribir(string nombre)
    {
        var pregunta0 ="¿Desea sobreescribir fortaleza";
        var pregunta1 = string.Format("\" {0} \"?", nombre);

        var popup = prefabPopup.InstantiateElement<Grid>("Popup");
        ConfigurarPopup(popup, pregunta0, pregunta1, () =>
        {
            // Sí
            if (controladorCreación.EnClicGuardar(nombre, true))
            {
                CargarFortalezas();
                popups.Children.Remove(popup);
            }
            else
            {
                MostrarMensaje("Error al sobreescribir");
                popups.Children.Remove(popup);
            }
        }, () =>
        {
            // No
            popups.Children.Remove(popup);
        });
        popups.Children.Add(popup);
    }

    private void EnClicEliminar(string nombre)
    {
        var pregunta0 = "¿Desea eliminar fortaleza";
        var pregunta1 = string.Format("\" {0} \"?", nombre);

        var popup = prefabPopup.InstantiateElement<Grid>("Popup");
        ConfigurarPopup(popup, pregunta0, pregunta1, () =>
        {
            // Sí
            if(SistemaMemoria.EliminarFortaleza(nombre))
            {
                CargarFortalezas();
                popups.Children.Remove(popup);
            }
            else
            {
                MostrarMensaje("Error al eliminar");
                popups.Children.Remove(popup);
            }
        }, () =>
        {
            // No
            popups.Children.Remove(popup);
        });
        popups.Children.Add(popup);
    }

    private void VerificarFuente(object sender, RoutedEventArgs args)
    {
        var fuente = SistemaTraducción.VerificarFuente(txtNuevoNombre.Text);

        if (txtNuevoNombre.Font != fuente)
            txtNuevoNombre.Font = fuente;
    }

    private async void MostrarMensaje(string mensaje)
    {
        txtMensaje.Text = mensaje;
        await Task.Delay(1000);
        txtMensaje.Text = string.Empty;
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
