using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace Terracota;
using static Constantes;

public class ControladorFortaleza : StartupScript
{
    public List<ElementoBloque> estatuas = new List<ElementoBloque> { };
    public List<ElementoBloque> cortos = new List<ElementoBloque> { };
    public List<ElementoBloque> largos = new List<ElementoBloque> { };

    private List<ControladorEstatua> controladoresEstatuas;
    private bool inicializado;
    private Vector3 posiciónInicial;

    public override void Start() 
    {
        posiciónInicial = Entity.Transform.Position;
        Entity.Transform.Position = new Vector3(posiciónInicial.X, -100, posiciónInicial.Z);

        controladoresEstatuas = new List<ControladorEstatua>();
        foreach (var estatua in estatuas)
        {
            controladoresEstatuas.Add(estatua.Entity.Get<ControladorEstatua>());
        }
    }

    public void Inicializar(Fortaleza fortaleza, bool anfitrión)
    {
        if(!inicializado)
        {
            inicializado = true;
            Entity.Transform.Position = posiciónInicial;
        }

        // Bloques
        var índieEstatuas = 0;
        var índieCortos = 0;
        var índieLargos = 0;

        foreach (var bloque in fortaleza.bloques)
        {
            switch(bloque.tipoBloque)
            {
                case TipoBloque.estatua:
                    estatuas[índieEstatuas].Posicionar(bloque.posición, bloque.rotación);
                    índieEstatuas++;
                    break;
                case TipoBloque.corto:
                    cortos[índieCortos].Posicionar(bloque.posición, bloque.rotación);
                    índieCortos++;
                    break;
                case TipoBloque.largo:
                    largos[índieLargos].Posicionar(bloque.posición, bloque.rotación);
                    índieLargos++;
                    break;
            }
        }

        // Rotación
        if (anfitrión)
            Entity.Transform.Rotation = Quaternion.RotationY(MathUtil.DegreesToRadians(0));
        else
            Entity.Transform.Rotation = Quaternion.RotationY(MathUtil.DegreesToRadians(180));
    }

    public void Activar()
    {
        foreach (var estatua in estatuas)
        {
            estatua.Activar();
        }
        foreach (var corto in cortos)
        {
            corto.Activar();
        }
        foreach (var largo in largos)
        {
            largo.Activar();
        }

        foreach (var estatua in controladoresEstatuas)
        {
            estatua.Iniciar();
        }
    }
}
