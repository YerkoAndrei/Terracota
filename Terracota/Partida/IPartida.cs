namespace Terracota; 
using static Constantes;

public interface IPartida
{
    bool ObtenerActivo();
    void DesactivarEstatua(TipoJugador jugador);
    TipoProyectil CambiarProyectil();
}
