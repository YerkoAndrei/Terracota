using System;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;

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
    private Quaternion rotaciónObjetivo;
    private Quaternion direcciónObjetivo;
    private float YObjetivo;
    private bool derecha;
    private TransformComponent luzDireccional;

    // Efecto disparo
    private float ZInicial;
    private Vector3 posiciónInicial;
    private Vector3 posiciónObjetivo;
    private bool retrocediendo;

    // Llamados
    private Action enFin;

    public override void Start()
    {
        cámara = Entity.Get<TransformComponent>();
        ZInicial = cámara.Position.Z;
    }

    public override void Update()
    {
        if (rotando)
            RotarEjeCámara();

        if (moviendo)
        {
            MoverCámara();
        }
    }

    public void RotarCámara(float _YObjetivo, bool _derecha, TransformComponent _luzDireccional = null, Action _enFin = null)
    {
        // Referencias
        YObjetivo = _YObjetivo;
        derecha = _derecha;
        luzDireccional = _luzDireccional;
        enFin = _enFin;

        // Variables
        duraciónLerp = 1f;
        rotaciónInicial = eje.Rotation;
        rotaciónObjetivo = rotaciónInicial * Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo));

        // Ajusta dirección de movimiento
        if (derecha)
            direcciónObjetivo = rotaciónInicial * Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo - 0.01f));
        else
            direcciónObjetivo = rotaciónInicial * Quaternion.RotationY(MathUtil.DegreesToRadians(YObjetivo + 0.01f));

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

    private void Reiniciar()
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

        eje.Rotation = Quaternion.Lerp(rotaciónInicial, direcciónObjetivo, tiempo);

        // Mueve sol 45º aprox.
        if (luzDireccional != null)
            luzDireccional.Rotation *= Quaternion.RotationY(0.005f);

        // Fin
        if (tiempoDelta >= duraciónLerp)
        {
            eje.Rotation = rotaciónObjetivo;
            Reiniciar();
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
                IniciarNormalidad();
            else
                Reiniciar();
        }
    }

    private void IniciarNormalidad()
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
