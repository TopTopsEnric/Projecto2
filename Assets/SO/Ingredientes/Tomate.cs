using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
[CreateAssetMenu(fileName = "Tomate", menuName = "Tools/resources/Ingredients/Tomate")]


public class Tomate : IngredientesSO
{
    public AsignarPieza asignar;

    public override void ActivarEfecto(List<Node> neighbors)
    {
        if (neighbors == null || neighbors.Count == 0)
        {
            Debug.Log("No hay nodos vecinos disponibles");
            return;
        }

        if (asignar != null)
        {
            // Llamar a DetectarNodo con los parámetros específicos
            asignar.DetectarNodo(true, neighbors);
        }
        else
        {
            Debug.LogError("La referencia a AsignarPieza no está configurada en el componente Tomate");
        }
    }
}
