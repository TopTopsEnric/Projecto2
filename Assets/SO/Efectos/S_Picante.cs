using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "S_Picante", menuName = "Tools/resources/efectos/S_Picante")]
public class S_Picante : Efectos
{
    public override void ActivarEfecto(List<Node> neighbors, Node nodoOrigen)
    {
        nodoOrigen.AgregarEfectoTemporal(this, 3);
    }

    public override void EjecutarTurno(Node nodo)
    {
        if (nodo.ingrediente)
        {
            var vacios = nodo.zonas.Values.Where(n => !n.ingrediente).ToList();
            if (vacios.Count > 0)
            {
                var destino = vacios[Random.Range(0, vacios.Count)];
                destino.SetIngrediente(nodo.recurso);
                nodo.destruirIngrediente();
            }
        }
    }
}
