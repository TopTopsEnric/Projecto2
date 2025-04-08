using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limpiador_Efectos : MonoBehaviour
{
    // nodes = Diccionario de tus nodos
    // recursosOriginales = Lista de los resources SO originales
    public static void CorregirRecursos(Dictionary<int, Node> nodes)
    {
        foreach (var nodo in nodes.Values)
        {
            // Si el nodo tiene recurso asignado
            if (nodo.recurso != null)
            {
                nodo.SetIngrediente(nodo.recurso);
            }
            else
            {
                continue;
            }
        }
    }
}
