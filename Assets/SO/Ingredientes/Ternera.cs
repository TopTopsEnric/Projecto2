using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ternera", menuName = "Tools/resources/Ingredients/Ternera")]
public class Ternera : IngredientesSO
{
    public override void ActivarEfecto(List<Node> neighbors, Node nodoOrigen)
    {
        Debug.Log("PasivaIngrediente ternera llamada");
        

        Node node = neighbors[0];
        
        Debug.Log("tiene ingrediente el nodo?" + node.ingrediente);
            if (node.ingrediente)
            {
            node.vida += 1;
            node.esmovible = false;
            Debug.Log("Se ha activado el efecto de Ternera " + node.esmovible + "y el efecto de vida" + node.vida);
            }



    }
}
