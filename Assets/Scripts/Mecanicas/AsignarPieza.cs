using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsignarPieza : MonoBehaviour
{

    public NodeMap nodeMap;
    public ResourcesSO recurso;
    public int posicion;
    public LayerMask nodosLayer; // Filtrar solo los nodos

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detectar toque en pantalla
        {
            Debug.Log("se esta enviando el raycast");
            DetectarNodo();
        }
    }

    public void DetectarNodo(bool esTomate = false, List<Node> nodosLimitados = null)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, nodosLayer))
        {
            if (hit.collider.CompareTag("Nodo"))
            {
                Debug.Log("Se está detectando la posición");
                int nodoPosicion = int.Parse(hit.collider.gameObject.name);

                if (esTomate)
                {
                    // Convertir la posición del nodo a Vector2Int para comparación
                    // Asumiendo que el nodoPosicion se puede convertir a coordenadas x, y
                    // Esta conversión dependerá de cómo mapeas los índices a posiciones
                    Vector2Int posicionSeleccionada = ConvertirIndiceAPosicion(nodoPosicion);

                    // Verificar si la posición está en la lista de nodos limitados
                    bool existeEnLista = nodosLimitados.Any(nodo => nodo.position == posicionSeleccionada);

                    if (existeEnLista)
                    {
                        posicion = nodoPosicion;
                        cambiarvisual();
                    }
                    else
                    {
                        Debug.Log("Este nodo no está disponible para selección");
                    }
                }
                else
                {
                    // Comportamiento normal
                    posicion = nodoPosicion;
                    cambiarvisual();
                }
            }
        }
    }
    private Vector2Int ConvertirIndiceAPosicion(int indice)
    {
        // Ejemplo: Si tu mapa es una cuadrícula de ancho 'width'
        int width = nodeMap.width; // Asumiendo que nodeMap tiene una propiedad width
        int x = indice % width;
        int y = indice / width;
        return new Vector2Int(x, y);
    }

    public void cambiarvisual()
    {
        nodeMap.ChangeNodeSprite(posicion, recurso);
    }
}
