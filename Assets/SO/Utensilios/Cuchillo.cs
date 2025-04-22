using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "Cuchillo", menuName = "Tools/resources/Utensilios/Cuchillo")]
public class Cuchillo : Utensilio
{

    public override void ActivarEfecto(List<Node> nodosSeleccionados, Node nodoOrigen)
    {
        // Solo activar si el nodo origen está vacío (sin ingrediente)
        if (nodoOrigen.ingrediente)
        {
            Debug.Log("El nodo origen debe estar vacío para usar el cuchillo.");
            return;
        }

        // Dirección hacia arriba
        Vector2Int direccion = new Vector2Int(0, 1);

        Vector2Int posActual = nodoOrigen.position;
        Node nodoActual = nodoOrigen;

        // Limpiar sprite utensilio en nodo origen para iniciar movimiento
        nodoOrigen.SetUtensilioVisual(null);

        while (true)
        {
            posActual += direccion;

            // Buscar nodo en la posición actual
            Node nodoSiguiente = nodeMap.nodes.Values.FirstOrDefault(n => n.position == posActual);
            if (nodoSiguiente == null)
            {
                // No hay más nodos arriba, terminar movimiento
                nodoActual.SetUtensilioVisual(null);
                break;
            }

            // Mover sprite del cuchillo al nodo siguiente
            nodoActual.SetUtensilioVisual(null); // Limpiar sprite del nodo anterior
            nodoSiguiente.SetUtensilioVisual(this); // Mostrar sprite del cuchillo en nodo siguiente

            nodoActual = nodoSiguiente;

            if (nodoSiguiente.ingrediente)
            {
                // Encontró ingrediente, destruirlo y detener
                nodoSiguiente.destruirIngrediente();
                Debug.Log("Ingrediente destruido con cuchillo en nodo " + nodoSiguiente.nodeid);

                // Limpiar sprite del cuchillo tras acción
                nodoSiguiente.SetUtensilioVisual(null);
                break;
            }
        }
    }
}
