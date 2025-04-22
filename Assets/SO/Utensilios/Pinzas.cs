using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pinzas", menuName = "Tools/resources/Utensilios/Pinzas")]
public class Pinzas : Utensilio
{
    public override int nodosRequeridos => 2;
    public override void ActivarEfecto(List<Node> nodosSeleccionados, Node nodoOrigen)
    {
        if (nodosSeleccionados.Count < 2) return;

        Node origen = nodosSeleccionados[0];
        Node destino = nodosSeleccionados[1];

        if (origen.ingrediente)
        {
            destino.SetIngrediente(origen.recurso);
            origen.destruirIngrediente();
            Debug.Log("Ingrediente movido con pinzas de nodo " + origen.nodeid + " a nodo " + destino.nodeid);
        }
    }
}
