using System;
using Stride.Engine;
using Stride.Core.Mathematics;

namespace Terracota;

public class ControladorCámara : SyncScript
{
    public TransformComponent eje;
    public TransformComponent cámara;

    private bool rotando;
    private bool moviendo;

    // Lerp
    private float duraciónLerp;
    private float tiempoDelta;
    private float tiempo;

    // Rotación
    private Quaternion rotaciónInicial;
    private Quaternion rotaciónInicialLuz;
    private Quaternion rotaciónObjetivo;
    private Quaternion rotaciónObjetivoLuz;
    private bool primeraRotación;
    private bool derecha;
    private float YObjetivo;
    private TransformComponent luzDireccional;

    // Efecto disparo
    private Vector3 posiciónInicial;
    private Vector3 posiciónObjetivo;
    private float ZInicial;
    private bool retrocediendo;

    // Llamados
    private Action enFin;

    public override void Start()
    {
        ZInicial = cámara.Position.Z;
    }

    public override void Update()
    {
        if (rotando)
            RotarEjeCámara();

        if (moviendo)
            MoverCámara();
    }

    public void RotarYCámara(float _YObjetivo, bool _derecha, Action _enFin = null, TransformComponent _luzDireccional = null)
    {
        if (rotando)
        {
            eje.Rotation = rotaciónObjetivo;
            TerminarLerp();
        }

        // Referencias
        YObjetivo = _YObjetivo;
        derecha = _derecha;
        luzDireccional = _luzDireccional;
        enFin = _enFin;

        // Variables
        duraciónLerp = 1f;
        rotaciónInicial = eje.Rotation;

        // Ajusta dirección de movimiento
        if (!derecha)
            YObjetivo = (YObjetivo * -1);

        rotaciónObjetivo = rotaciónInicial * Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo));

        if (luzDireccional != null)
        {
            rotaciónInicialLuz = luzDireccional.Rotation;
            rotaciónObjetivoLuz = rotaciónInicialLuz * Quaternion.RotationY(45f);
        }
        rotando = true;
    }

    public void RotarXCámara(float XObjetivo, float tiempo)
    {
        // Variables
        duraciónLerp = tiempo;
        rotaciónInicial = eje.Rotation;

        rotaciónObjetivo = rotaciónInicial * Quaternion.RotationZ(MathUtil.DegreesToRadians(XObjetivo));
        rotando = true;
    }

    public void ActivarEfectoDisparo()
    {
        var retroceso = 0.8f;
        duraciónLerp = 0.03f;

        posiciónInicial = cámara.Position;
        posiciónObjetivo = new Vector3(posiciónInicial.X, posiciónInicial.Y, (posiciónInicial.Z + retroceso));

        retrocediendo = true;
        moviendo = true;
    }

    private void TerminarLerp()
    {
        moviendo = false;
        rotando = false;
        tiempoDelta = 0;
        tiempo = 0;

        // Llamado
        if(enFin != null)
        {
            enFin.Invoke();
            enFin = null;
        }
    }

    private void RotarEjeCámara()
    {
        tiempoDelta += (float)Game.UpdateTime.Elapsed.TotalSeconds;
        tiempo = tiempoDelta / duraciónLerp;

        eje.Rotation = Quaternion.Lerp(rotaciónInicial, rotaciónObjetivo, tiempo);

        // Mueve sol 45º
        if (luzDireccional != null)
            luzDireccional.Rotation = Quaternion.Lerp(rotaciónInicialLuz, rotaciónObjetivoLuz, tiempo);

        // Fin
        if (tiempoDelta >= duraciónLerp)
        {
            eje.Rotation = rotaciónObjetivo;
            TerminarLerp();
        }
    }

    private void MoverCámara()
    {
        tiempoDelta += (float)Game.UpdateTime.Elapsed.TotalSeconds;
        tiempo = tiempoDelta / duraciónLerp;

        cámara.Position = Vector3.Lerp(posiciónInicial, posiciónObjetivo, tiempo);

        // Fin
        if (tiempoDelta >= duraciónLerp)
        {
            cámara.Position = posiciónObjetivo;
            if (retrocediendo)
                DevolverCámara();
            else
                TerminarLerp();
        }
    }

    private void DevolverCámara()
    {
        // Retroceso
        retrocediendo = false;
        duraciónLerp = 0.8f;
        tiempoDelta = 0;
        tiempo = 0;

        posiciónInicial = cámara.Position;
        posiciónObjetivo = new Vector3(posiciónInicial.X, posiciónInicial.Y, ZInicial);
    }
}
