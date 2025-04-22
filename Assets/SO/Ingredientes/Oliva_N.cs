using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Oliva_N", menuName = "Tools/resources/Ingredients/Oliva_N")]
public class Oliva_N : IngredientesSO
{
    
    public override void ActivarEfecto(List<Node> neighbors, Node nodoOrigen)
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            Node node = neighbors[i];
            ResourcesSO recurso= node.recurso;

            if (recurso != null)
            {
                node.rango = node.rango+1;
            }
        }

    }
}
