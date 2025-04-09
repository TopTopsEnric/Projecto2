using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Oliva_V", menuName = "Tools/resources/Ingredients/Oliva_V")]
public class Oliva_V : IngredientesSO
{
    public override void ActivarEfecto(List<Node> neighbors)
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            Node node = neighbors[i];
            ResourcesSO recurso = node.recurso;

            if (recurso != null)
            {
                node.rango = node.rango - 1;
            }
        }

    }
}

