namespace Terracota; 
using static Constantes;

public interface IPartida
{
    void DesactivarEstatua(TipoJugador jugador);
    TipoProyectil CambiarProyectil();
}
