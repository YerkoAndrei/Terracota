using System;
using System.Linq;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering.Sprites;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;

namespace Terracota;
using static Constantes;

public static class Sistema
{
    private static string ISO8006 = "yyyy-MM-ddThh:mm:ss";

    public static float RangoAleatorio(float min, float max)
    {
        var aleatorio = new Random();
        double valor = (aleatorio.NextDouble() * (max - min) + min);
        return (float)valor;
    }

    public static int RangoAleatorio(int min, int max)
    {
        var aleatorio = new Random();
        return aleatorio.Next(min, max);
    }

    public static Vector3 EulerAleatorio()
    {
        var aleatorio = new Random();

        // RotationEulerXYZ está en radianes
        var x = MathUtil.DegreesToRadians(aleatorio.Next(0, 360));
        var y = MathUtil.DegreesToRadians(aleatorio.Next(0, 360));
        var z = MathUtil.DegreesToRadians(aleatorio.Next(0, 360));
        return new Vector3(x, y, z);
    }

    public static ISpriteProvider ObtenerSprite(Texture textura)
    {
        var sprite = new SpriteFromTexture();
        sprite.Texture = textura;

        return sprite;
    }

    public static void ConfigurarBotón(Grid grid, Action action)
    {
        // Busca contenido dentro del "grid botón"
        var imagen = grid.FindVisualChildOfType<ImageElement>("img");
        var botón = grid.FindVisualChildOfType<Button>("btn");
        var ícono = grid.FindVisualChildOfType<ImageElement>("imgÍcono");

        // Texto
        var texto = ObtenerTexto(grid);
        var colorTexto = new Color(25, 20 ,20);
        if (texto != null)
            colorTexto = texto.TextColor;

        // En clic
        botón.Click += (s, a) => { action.Invoke(); };

        // Sonidos
        botón.TouchDown += (s, a) => { SistemaSonido.SonarBotónEntra(); };
        botón.TouchUp += (s, a) => { SistemaSonido.SonarBotónSale(); };

        // Cambios color
        var colorBase = imagen.Color;

        // Guarda colores iniciales. Se usa en bloqueo. Podría mejorar.
        grid.Children.Add(new Grid()
        {
            Name = "colorInicialBotón",
            BackgroundColor = colorBase,
            Opacity = 0
        });
        grid.Children.Add(new Grid()
        {
            Name = "colorInicialTexto",
            BackgroundColor = colorTexto,
            Opacity = 0
        });

        botón.MouseOverStateChanged += (s, a) =>
        {
            if (VerificarBloqueo(botón, imagen, texto, ícono, colorBase, colorTexto))
                return;
            switch (a.NewValue)
            {
                case MouseOverState.MouseOverElement:
                    imagen.Color = colorBase * colorEnCursor;
                    break;
                case MouseOverState.MouseOverNone:
                    imagen.Color = colorBase * colorNormal;
                    break;
            }
        };
        botón.TouchDown += (s, a) =>
        {
            if (VerificarBloqueo(botón, imagen, texto, ícono, colorBase, colorTexto))
                return;
            imagen.Color = colorBase * colorEnClic;
        };
        botón.TouchUp += (s, a) =>
        {
            if (VerificarBloqueo(botón, imagen, texto, ícono, colorBase, colorTexto))
                return;
            imagen.Color = colorBase * colorEnCursor;
        };
    }

    public static void ConfigurarBotónConImagen(Button botón, ImageElement imagen, Action action)
    {
        // En clic
        botón.Click += (sender, e) => { action.Invoke(); };

        // Sonidos
        botón.TouchDown += (s, a) => { SistemaSonido.SonarBotónEntra(); };
        botón.TouchUp += (s, a) => { SistemaSonido.SonarBotónSale(); };

        // Cambios color
        var colorBase = imagen.Color;
        botón.MouseOverStateChanged += (sender, args) =>
        {
            switch (args.NewValue)
            {
                case MouseOverState.MouseOverElement:
                    imagen.Color = colorBase * colorEnCursor;
                    break;
                case MouseOverState.MouseOverNone:
                    imagen.Color = colorBase * colorNormal;
                    break;
            }
        };
        botón.TouchDown += (sender, args) =>
        {
            imagen.Color = colorBase * colorEnClic;
        };
        botón.TouchUp += (sender, args) =>
        {
            imagen.Color = colorBase * colorEnCursor;
        };
    }

    private static bool VerificarBloqueo(Button botón, ImageElement imagen, TextBlock texto, ImageElement ícono, Color colorBase, Color colorTexto)
    {
        if (botón.CanBeHitByUser)
            return false;

        // Bloqueo
        imagen.Color = colorBase * colorBloqueado;
        if (texto != null)
            texto.TextColor = colorTexto * colorBloqueado;

        if (ícono != null)
            ícono.Color = colorTexto * colorBloqueado;

        return true;
    }

    // Debe llamarse después de configurar botón
    public static void BloquearBotón(Grid grid, bool bloquear)
    {
        // Busca contenido dentro del "grid botón"
        var imagen = grid.FindVisualChildOfType<ImageElement>("img");
        var botón = grid.FindVisualChildOfType<Button>("btn");
        var ícono = grid.FindVisualChildOfType<ImageElement>("imgÍcono");
        var texto = ObtenerTexto(grid);

        botón.CanBeHitByUser = !bloquear;

        // Colores
        if (bloquear)
        {
            imagen.Color = grid.FindVisualChildOfType<Grid>("colorInicialBotón").BackgroundColor * colorBloqueado;
            if (texto != null)
                texto.TextColor = grid.FindVisualChildOfType<Grid>("colorInicialTexto").BackgroundColor * colorBloqueado;
            if (ícono != null)
                ícono.Color = grid.FindVisualChildOfType<Grid>("colorInicialTexto").BackgroundColor * colorBloqueado;
        }
        else
        {
            imagen.Color = grid.FindVisualChildOfType<Grid>("colorInicialBotón").BackgroundColor * colorNormal;
            if (texto != null)
                texto.TextColor = grid.FindVisualChildOfType<Grid>("colorInicialTexto").BackgroundColor * colorNormal;
            if (ícono != null)
                ícono.Color = grid.FindVisualChildOfType<Grid>("colorInicialTexto").BackgroundColor * colorNormal;
        }
    }

    public static void ConfigurarBotónOculto(Button botón, Action action)
    {
        botón.Click += (sender, e) => { action.Invoke(); };
    }

    public static void ConfigurarRanuraCreación(Grid grid, int fila, string nombre, Action enCargar, Action enSobreescribir, Action enElminar)
    {
        // Busca contenido dentro del "grid botón"
        ConfigurarBotón(grid, enCargar);
        var texto = ObtenerTexto(grid);

        // Botones
        var btnEliminar = grid.FindVisualChildOfType<Grid>("btnEliminar");
        ConfigurarBotón(btnEliminar, enElminar);

        var btnSobreescribir = grid.FindVisualChildOfType<Grid>("btnSobreescribir");
        ConfigurarBotón(btnSobreescribir, enSobreescribir);

        // Visual
        texto.Text = nombre;
        texto.Font = SistemaTraducción.VerificarFuente(nombre);
        grid.SetGridRow(fila);
    }

    public static void ConfigurarRanuraVacíaCreación(Grid grid, int fila, string nombre, Action enCargar)
    {
        // Busca contenido dentro del "grid botón"
        ConfigurarBotón(grid, enCargar);
        var texto = ObtenerTexto(grid);

        // Botones
        grid.FindVisualChildOfType<Grid>("btnEliminar").Visibility = Visibility.Hidden;
        grid.FindVisualChildOfType<Grid>("btnSobreescribir").Visibility = Visibility.Hidden;

        // Visual
        texto.Text = nombre;
        grid.SetGridRow(fila);
    }

    public static void ConfigurarHostLAN(Grid grid, string ip, string nombre, Action enConectarComoAnfitrión, Action enConectarComoHuesped)
    {
        // Busca contenido dentro del "grid botón"
        var textoNombreIP = grid.FindVisualChildOfType<TextBlock>("txtNombreIP");
        var conectar = grid.FindVisualChildOfType<TextBlock>("txtConectar");
        var anfitrión = grid.FindVisualChildOfType<TextBlock>("txtAnfitrión");
        var huesped = grid.FindVisualChildOfType<TextBlock>("txtHuesped");

        conectar.Text = SistemaTraducción.ObtenerTraducción("conectarComo");
        anfitrión.Text = SistemaTraducción.ObtenerTraducción("anfitrión");
        huesped.Text = SistemaTraducción.ObtenerTraducción("huesped");

        // Botones
        var btnComoAnfitrión = grid.FindVisualChildOfType<Grid>("btnComoAnfitrión");
        ConfigurarBotón(btnComoAnfitrión, enConectarComoAnfitrión);

        var btnComoHuesped = grid.FindVisualChildOfType<Grid>("btnComoHuesped");
        ConfigurarBotón(btnComoHuesped, enConectarComoHuesped);

        // Visual
        textoNombreIP.Text = ip;

        if (!string.IsNullOrEmpty(nombre))
            textoNombreIP.Text += "  -  " + nombre;

        textoNombreIP.Font = SistemaTraducción.VerificarFuente(textoNombreIP.Text);
        conectar.Font = SistemaTraducción.VerificarFuente(conectar.Text);
        anfitrión.Font = SistemaTraducción.VerificarFuente(conectar.Text);
        huesped.Font = SistemaTraducción.VerificarFuente(conectar.Text);
    }

    public static TextBlock ObtenerTexto(Grid grid)
    {
        // Encuentra primer texto del grid
        var textos = grid.FindVisualChildrenOfType<TextBlock>().ToArray();
        if(textos.Length > 0)
            return textos[0];
        else
            return null;
    }

    public static void ConfigurarRanuraElección(Grid grid, int fila, string nombre, Action enClic)
    {
        // Busca contenido dentro del "grid botón"
        ConfigurarBotón(grid, enClic);
        var texto = ObtenerTexto(grid);

        // Visual
        texto.Text = nombre;
        texto.Font = SistemaTraducción.VerificarFuente(nombre);
        grid.SetGridRow(fila);
    }

    public static void ConfigurarPopup(Grid grid, string pregunta0, string pregunta1, Action enClicSí, Action enClicNo)
    {
        grid.FindVisualChildOfType<TextBlock>("txtPregunta 0").Text = pregunta0;
        grid.FindVisualChildOfType<TextBlock>("txtPregunta 1").Text = pregunta1;
        grid.FindVisualChildOfType<TextBlock>("txtPregunta 1").Font = SistemaTraducción.VerificarFuente(pregunta1);

        ConfigurarBotón(grid.FindVisualChildOfType<Grid>("btnSí"), enClicSí);
        ConfigurarBotón(grid.FindVisualChildOfType<Grid>("btnNo"), enClicNo);
    }

    public static string FormatearFechaEstándar(DateTime fecha)
    {
        var fechaHora = fecha.ToString(ISO8006);
        return fechaHora;
    }

    public static float ObtenerMayorValor(Vector3 velocidad)
    {
        var mayor = 0f;

        if (MathF.Abs(velocidad.X) > mayor)
            mayor = MathF.Abs(velocidad.X);

        if (MathF.Abs(velocidad.Y) > mayor)
            mayor = MathF.Abs(velocidad.Y);

        if (MathF.Abs(velocidad.Z) > mayor)
            mayor = MathF.Abs(velocidad.Z);

        return mayor;
    }
}
