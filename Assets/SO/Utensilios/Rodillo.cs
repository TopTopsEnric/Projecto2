using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "Rodillo", menuName = "Tools/resources/Utensilios/Rodillo")]
public class Rodillo : Utensilio
{

    public override void ActivarEfecto(List<Node> nodosSeleccionados, Node nodoOrigen)
    {
        Debug.Log("Activando efecto de rodillo en nodo " + nodoOrigen.nodeid);
        if (nodosSeleccionados.Count < 1) return;

        int columnaX = nodoOrigen.position.x;
        bool haciaDerecha = true; // Cambia según lógica o selección del jugador

        // Obtener todos los nodos en la columna X, ordenados por Y ascendente
        var columna = nodeMap.nodes.Values.Where(n => n.position.x == columnaX).OrderBy(n => n.position.y).ToList();

        if (columna.Count == 0) return;

        // Para cada nodo en la columna, mover su ingrediente al nodo vecino horizontal (x+1 o x-1)
        // Primero guardamos los ingredientes que se moverán para evitar sobreescrituras
        Dictionary<Node, ResourcesSO> ingredientesDestino = new Dictionary<Node, ResourcesSO>();

        foreach (var nodo in columna)
        {
            int destinoX = haciaDerecha ? nodo.position.x + 1 : nodo.position.x - 1;
            Vector2Int posDestino = new Vector2Int(destinoX, nodo.position.y);

            Node nodoDestino = nodeMap.nodes.Values.FirstOrDefault(n => n.position == posDestino);
            if (nodoDestino != null)
            {
                ingredientesDestino[nodoDestino] = nodo.recurso; // Guardamos el recurso que irá al nodo destino
            }
        }

        // Ahora asignamos los ingredientes a los nodos destino
        foreach (var kvp in ingredientesDestino)
        {
            kvp.Key.SetIngrediente(kvp.Value);
        }

        // Limpiar ingredientes en nodos origen que se movieron (si el nodo destino existe)
        foreach (var nodo in columna)
        {
            int destinoX = haciaDerecha ? nodo.position.x + 1 : nodo.position.x - 1;
            Vector2Int posDestino = new Vector2Int(destinoX, nodo.position.y);

            if (nodeMap.nodes.Values.Any(n => n.position == posDestino))
            {
                nodo.destruirIngrediente();
            }
        }

        Debug.Log("Ingredientes movidos horizontalmente con rodillo en columna " + columnaX + " hacia " + (haciaDerecha ? "derecha" : "izquierda"));
    }
}
