using System;
using Stride.Engine;
using Stride.Animations;
using Stride.Core.Mathematics;
using Stride.UI.Panels;
using Stride.UI;
using Stride.Rendering.Lights;
using Stride.UI.Controls;

namespace Terracota;
using static Constantes;

public class SistemaAnimación : SyncScript
{
    public ComputeCurveSampler<float> curvaRápida;

    private static SistemaAnimación instancia;

    private bool animando;

    // Animación
    private Grid elemento;
    private float duración;
    private float tiempoDelta;
    private float tiempo;
    private TipoCurva tipoCurva;
    private Thickness margenInicio;
    private Thickness margenObjetivo;
    private Action enFin;

    public override void Start()
    {
        instancia = this;
    }

    public override void Update()
    {
        if (animando)
            Animar();
    }

    public static float EvaluarSuave(float tiempo)
    {
        return MathUtil.SmoothStep(tiempo);
    }

    public static float EvaluarRápido(float tiempo)
    {
        instancia.curvaRápida.UpdateChanges();
        return instancia.curvaRápida.Evaluate(tiempo);
    }

    private static void AnimarElemento(Grid _elemento, float _duración, bool entrando, Direcciones dirección, TipoCurva _tipoCurva, Action _enFin)
    {
        var original = _elemento.Margin;
        var fuera = _elemento.Margin;

        switch (dirección)
        {
            case Direcciones.arriba:
                fuera = new Thickness(original.Left, original.Top + 1000, original.Right, original.Bottom);
                break;
            case Direcciones.abajo:
                fuera = new Thickness(original.Left, original.Top, original.Right, original.Bottom + 1000);
                break;
            case Direcciones.izquierda:
                fuera = new Thickness(original.Left + 1000, original.Top, original.Right, original.Bottom);
                break;
            case Direcciones.derecha:
                fuera = new Thickness(original.Left, original.Top, original.Right + 1000, original.Bottom);
                break;
        }

        if(entrando)
        {
            instancia.margenInicio = fuera;
            instancia.margenObjetivo = original;
        }
        else
        {
            instancia.margenInicio = original;
            instancia.margenObjetivo = fuera;
        }

        // Predeterminados
        instancia.elemento.Margin = instancia.margenInicio;

        instancia.elemento = _elemento;
        instancia.duración = _duración;
        instancia.tipoCurva = _tipoCurva;
        instancia.enFin = _enFin;

        instancia.tiempoDelta = 0;
        instancia.tiempo = 0;
        instancia.animando = true;
    }

    private void Animar()
    {
        tiempoDelta += (float)Game.UpdateTime.Elapsed.TotalSeconds;
        switch (tipoCurva)
        {
            case TipoCurva.nada:
                tiempo = tiempoDelta / duración;
                break;
            case TipoCurva.suave:
                tiempo = EvaluarSuave(tiempoDelta / duración);
                break;
            case TipoCurva.rápida:
                tiempo = EvaluarRápido(tiempoDelta / duración);
                break;
        }

        float left = MathUtil.Lerp(margenInicio.Left, margenObjetivo.Left, tiempo);
        float top = MathUtil.Lerp(margenInicio.Top, margenObjetivo.Top, tiempo);
        float right = MathUtil.Lerp(margenInicio.Right, margenObjetivo.Right, tiempo);
        float bottom = MathUtil.Lerp(margenInicio.Bottom, margenObjetivo.Bottom, tiempo);
        elemento.Margin = new Thickness(left, top, right, bottom);

        // Fin
        if (tiempoDelta >= duración)
        {
            elemento.Margin = margenObjetivo;
            TerminarLerp();
        }
    }

    private void TerminarLerp()
    {
        animando = false;
        tiempoDelta = 0;
        tiempo = 0;

        // Llamado
        if (enFin != null)
        {
            enFin.Invoke();
            enFin = null;
        }
    }
}
