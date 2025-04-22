using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "S_Especial", menuName = "Tools/resources/efectos/S_Especial")]
public class S_Especial : Efectos
{
    public override void ActivarEfecto(List<Node> neighbors, Node nodoOrigen)
    {
        nodoOrigen.AgregarEfectoTemporal(this, 1);
    }

    public override void EjecutarTurno(Node nodo)
    {
        int fila = nodo.position.y;

        // Obtener todos los nodos en la fila (Y constante)
        List<Node> filaNodos = nodo.zonas.Values
            .Where(n => n.position.y == fila)
            .ToList();

        foreach (var actual in filaNodos)
        {
            // Buscar el nodo que está justo arriba (Y + 1, misma X)
            Vector2Int posicionArriba = new Vector2Int(actual.position.x, actual.position.y + 1);

            Node destino = nodo.zonas.Values.FirstOrDefault(n => n.position == posicionArriba);

            if (actual.ingrediente && destino != null && !destino.ingrediente)
            {
                destino.SetIngrediente(actual.recurso);
                actual.destruirIngrediente();
            }
        }
    }
}
