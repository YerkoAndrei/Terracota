using System;
using Stride.Engine;
using Stride.Core.Mathematics;

namespace Terracota;

public class ControladorCámara : SyncScript
{
    public TransformComponent eje;
    public TransformComponent cámara;

    private bool rotando;
    private bool rotandoLuz;
    private bool moviendo;

    // Lerp
    private float duraciónRotación;
    private float duraciónLerp;
    private float tiempoDelta;
    private float tiempo;

    private float duraciónLerpLuz;
    private float tiempoDeltaLuz;
    private float tiempoLuz;

    // Rotación
    private Quaternion rotaciónInicial;
    private Quaternion rotaciónInicialLuz;
    private Quaternion rotaciónObjetivo;
    private Quaternion rotaciónObjetivoLuz;
    private bool derecha;
    private float YObjetivo;

    // Efecto disparo
    private Vector3 posiciónInicial;
    private Vector3 posiciónObjetivo;
    private TransformComponent luzDireccional;
    private float ZInicial;
    private bool retrocediendo;

    // Llamados
    private Action enFin;

    public override void Start()
    {
        ZInicial = cámara.Position.Z;
        duraciónRotación = 1;
    }

    public override void Update()
    {
        if (rotando)
            RotarEjeCámara();

        if (rotandoLuz)
            RotarEjeLuz();

        if (moviendo)
            MoverCámara();
    }

    public void RotarYCámara(float _YObjetivo, bool _derecha, Action _enFin = null)
    {
        if (rotando)
        {
            eje.Rotation = rotaciónObjetivo;
            TerminarLerp();
        }

        // Referencias
        YObjetivo = _YObjetivo;
        derecha = _derecha;
        enFin = _enFin;

        // Variables
        duraciónLerp = duraciónRotación;
        rotaciónInicial = eje.Rotation;

        // Ajusta dirección de movimiento
        if (!derecha)
            YObjetivo = (YObjetivo * -1);

        rotaciónObjetivo = rotaciónInicial * Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo));
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

    public void RotarLuz(TransformComponent _luzDireccional)
    {
        // Referencias
        duraciónLerpLuz = duraciónRotación;
        luzDireccional = _luzDireccional;
        rotaciónInicialLuz = luzDireccional.Rotation;

        var rotación = 45f;
        if(SistemaRed.ObtenerJugando())
        {
            if (SistemaRed.ObtenerTipoJugador() == Constantes.TipoJugador.anfitrión)
                rotación = 11.25f;
            else
                rotación = -11.25f;
        }

        rotaciónObjetivoLuz = rotaciónInicialLuz * Quaternion.RotationY(MathUtil.DegreesToRadians(rotación));
        rotandoLuz = true;
    }

    public void ActivarEfectoDisparo()
    {
        var retroceso = 1;
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
        tiempo = SistemaAnimación.EvaluarSuave(tiempoDelta / duraciónLerp);

        eje.Rotation = Quaternion.Lerp(rotaciónInicial, rotaciónObjetivo, tiempo);

        // Fin
        if (tiempoDelta >= duraciónLerp)
        {
            eje.Rotation = rotaciónObjetivo;
            TerminarLerp();
        }
    }

    private void RotarEjeLuz()
    {
        tiempoDeltaLuz += (float)Game.UpdateTime.Elapsed.TotalSeconds;
        tiempoLuz = SistemaAnimación.EvaluarSuave(tiempoDeltaLuz / duraciónLerpLuz);

        luzDireccional.Rotation = Quaternion.Lerp(rotaciónInicialLuz, rotaciónObjetivoLuz, tiempoLuz);

        // Fin
        if (tiempoDeltaLuz >= duraciónLerpLuz)
        {
            rotandoLuz = false;
            tiempoDeltaLuz = 0;
            tiempoLuz = 0;
            luzDireccional.Rotation = rotaciónObjetivoLuz;
        }
    }

    private void MoverCámara()
    {
        tiempoDelta += (float)Game.UpdateTime.Elapsed.TotalSeconds;

        if(retrocediendo)
            tiempo = tiempoDelta / duraciónLerp;
        else
            tiempo = SistemaAnimación.EvaluarRápido(tiempoDelta / duraciónLerp);

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
