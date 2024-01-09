using System;
using System.Collections.Generic;
using Stride.Engine;
using Stride.Animations;
using Stride.Core.Mathematics;
using Stride.UI.Panels;
using Stride.UI;

namespace Terracota;
using static Constantes;

public class SistemaAnimación : SyncScript
{
    public ComputeCurveSampler<float> curvaRápida;

    private static SistemaAnimación instancia;

    private Dictionary<string, Thickness> elementosConocidos;
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
        elementosConocidos = new Dictionary<string, Thickness>();
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
        if (tiempo > 1)
            tiempo = 1;

        instancia.curvaRápida.UpdateChanges();
        return instancia.curvaRápida.Evaluate(tiempo);
    }

    public static void AnimarElemento(Grid _elemento, float _duración, bool entrando, Direcciones dirección, TipoCurva _tipoCurva, Action _enFin)
    {
        Thickness original;
        var fuera = new Thickness();

        // Busca o agrega elemento en diccionario, queda guardado por la sesión
        var nombre = _elemento.Name + "_" + _elemento.Parent.Name;
        if (instancia.elementosConocidos.ContainsKey(nombre))
            original = instancia.elementosConocidos.GetValueOrDefault(nombre);
        else
        {
            instancia.elementosConocidos.Add(nombre, _elemento.Margin);
            original = _elemento.Margin;
        }

        // Crea dirección exterior
        switch (dirección)
        {
            case Direcciones.arriba:
                fuera = new Thickness(original.Left, original.Top, original.Right, original.Bottom + 1500);
                break;
            case Direcciones.abajo:
                fuera = new Thickness(original.Left, original.Top + 1500, original.Right, original.Bottom);
                break;
            case Direcciones.izquierda:
                fuera = new Thickness(original.Left, original.Top, original.Right + 3000, original.Bottom);
                break;
            case Direcciones.derecha:
                fuera = new Thickness(original.Left + 3000, original.Top, original.Right, original.Bottom);
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
        instancia.elemento = _elemento;
        instancia.duración = _duración;
        instancia.tipoCurva = _tipoCurva;
        instancia.enFin = _enFin;

        instancia.elemento.Margin = instancia.margenInicio;

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
