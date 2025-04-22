using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Espatula", menuName = "Tools/resources/Utensilios/Espatula")]
public class Espatula : Utensilio
{
    public override int nodosRequeridos => 2;
    public override void ActivarEfecto(List<Node> nodosSeleccionados, Node nodoOrigen)
    {
        if (nodosSeleccionados.Count < 2) return;

        Node nodo1 = nodosSeleccionados[0];
        Node nodo2 = nodosSeleccionados[1];

        var tempRecurso = nodo1.recurso;
        nodo1.SetIngrediente(nodo2.recurso);
        nodo2.SetIngrediente(tempRecurso);

        Debug.Log("Ingredientes intercambiados con espátula entre nodos " + nodo1.nodeid + " y " + nodo2.nodeid);
    }
}
