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
        var índiceEstatuas = 0;
        var índiceCortos = 0;
        var índiceLargos = 0;

        foreach (var bloque in fortaleza.Bloques)
        {
            switch(bloque.TipoBloque)
            {
                case TipoBloque.estatua:
                    switch (bloque.TipoEstatua)
                    {
                        case TipoEstatua.chimpancé: // 0
                            estatuas[índiceEstatuas].Posicionar(bloque.Posición, bloque.Rotación);
                            break;
                        case TipoEstatua.gorila:    // 1
                            estatuas[índiceEstatuas].Posicionar(bloque.Posición, bloque.Rotación);
                            break;
                        case TipoEstatua.orangután: // 2
                            estatuas[índiceEstatuas].Posicionar(bloque.Posición, bloque.Rotación);
                            break;
                    }
                    índiceEstatuas++;
                    break;
                case TipoBloque.corto:
                    cortos[índiceCortos].Posicionar(bloque.Posición, bloque.Rotación);
                    índiceCortos++;
                    break;
                case TipoBloque.largo:
                    largos[índiceLargos].Posicionar(bloque.Posición, bloque.Rotación);
                    índiceLargos++;
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
