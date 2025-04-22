using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Queso", menuName = "Tools/resources/Ingredients/Queso")]
public class Queso : IngredientesSO
{
    public override void ActivarEfecto(List<Node> neighbors, Node nodoOrigen)
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            Node node = neighbors[i];
            ResourcesSO recurso = node.recurso;

            if (recurso != null)
            {
                node.esmovible = false;
            }
        }

    }
}
