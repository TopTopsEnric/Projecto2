using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Pollo", menuName = "Tools/resources/Ingredients/Pollo")]
public class Pollo : IngredientesSO
{
    public override void ActivarEfecto(List<Node> neighbors, Node nodoOrigen)
    {
        Debug.Log("Ejecutando la pasiva de pollo");

        if (neighbors.Count <= 0) return;

        // 1. Tomar una copia del estado actual de cada nodo
        Dictionary<int, ResourcesSO> currentState = new Dictionary<int, ResourcesSO>();
        foreach (Node node in neighbors)
        {
            currentState[node.nodeid] = node.recurso;
        }

        // 2. Planificar el nuevo estado (cambios por rotación)
        Dictionary<int, ResourcesSO> newState = new Dictionary<int, ResourcesSO>();

        // Convertimos la lista a un arreglo para mejor manejo
        Node[] nodeArray = neighbors.ToArray();

        // Rotamos una posición en sentido horario
        for (int i = 0; i < nodeArray.Length; i++)
        {
            int currentIndex = i;
            int nextIndex = (i + 1) % nodeArray.Length;

            // Si el nodo actual tiene un ingrediente, intentamos moverlo
            if (currentState[nodeArray[currentIndex].nodeid] != null )
            {
                // Si el siguiente nodo está vacío, movemos el ingrediente allí
                if (currentState[nodeArray[nextIndex].nodeid] == null)
                {
                    newState[nodeArray[nextIndex].nodeid] = currentState[nodeArray[currentIndex].nodeid];
                    // Marcamos el nodo actual como vacío en el estado actual
                    currentState[nodeArray[currentIndex].nodeid] = null;
                }
                else
                {
                    // Si el siguiente nodo no está vacío, mantenemos el ingrediente donde está
                    newState[nodeArray[currentIndex].nodeid] = currentState[nodeArray[currentIndex].nodeid];
                }
            }
        }

        // 3. Aplicar los cambios
        foreach (Node node in neighbors)
        {
            // Si el nodo tendrá un ingrediente en el nuevo estado
            if (newState.ContainsKey(node.nodeid) && newState[node.nodeid] != null)
            {
                // Si el ingrediente cambió o es nuevo
                if (node.recurso != newState[node.nodeid])
                {
                    // Si ya tiene un ingrediente, lo destruimos primero
                    if (node.recurso != null)
                    {
                        node.destruirIngrediente();
                    }
                    // Colocamos el nuevo ingrediente
                    node.SetIngrediente(newState[node.nodeid]);
                }
            }
            // Si el nodo estará vacío en el nuevo estado pero ahora tiene ingrediente
            else if (node.recurso != null)
            {
                node.destruirIngrediente();
            }
        }
    }
}
