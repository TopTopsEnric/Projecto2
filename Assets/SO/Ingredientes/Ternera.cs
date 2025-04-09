using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ternera", menuName = "Tools/resources/Ingredients/Ternera")]
public class Ternera : IngredientesSO
{
    public override void ActivarEfecto(List<Node> neighbors)
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            Node node = neighbors[i];
            if (i == 0) { 
                node.vida += 1;
            }
            ResourcesSO recurso = node.recurso;

            if (recurso != null)
            {
                node.rango = node.rango + 1;
            }
        }
    }
}
