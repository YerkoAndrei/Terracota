﻿using System.Threading.Tasks;
using Stride.Input;
using Stride.Engine;

namespace Terracota;
using static Constantes;

public class ControladorPartidaP2P : AsyncScript
{
    public ControladorCañon cañónAnfitrión;
    public ControladorCañon cañónHuesped;

    private ControladorCañon cañónActual;
    private TipoJugador turnoJugador;
    private bool cambiandoTurno;

    public override async Task Execute()
    {
        cañónAnfitrión.Activar(true);
        cañónHuesped.Activar(false);

        turnoJugador = TipoJugador.anfitrión;
        cañónActual = cañónAnfitrión;

        while (Game.IsRunning)
        {
            if (Input.IsKeyPressed(Keys.Z) && !cambiandoTurno)
            {
                cañónActual.Disparar(TipoProyectil.bola);
                CambiarTurno();
            }

            if (Input.IsKeyPressed(Keys.X) && !cambiandoTurno)
            {
                cañónActual.Disparar(TipoProyectil.metralla);
                CambiarTurno();
            }
            
            await Script.NextFrame();
        }
    }

    private async void CambiarTurno()
    {
        cambiandoTurno = true;
        await Task.Delay(duraciónTurno);

        if (turnoJugador == TipoJugador.anfitrión)
        {
            cañónAnfitrión.Activar(false);
            cañónHuesped.Activar(true);
            cañónActual = cañónHuesped;

            turnoJugador = TipoJugador.huesped;
        }
        else
        {
            cañónAnfitrión.Activar(true);
            cañónHuesped.Activar(false);
            cañónActual = cañónAnfitrión;

            turnoJugador = TipoJugador.anfitrión;
        }
        cambiandoTurno = false;
    }
}