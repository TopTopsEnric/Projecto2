
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Node
{
    private int nodeid;
    public Vector2Int position; // Posición en la cuadrícula
    private Dictionary<int, Node> zonas = new Dictionary<int, Node>();
    public List<Node> neighbors = new List<Node>(); // Conexiones con otros nodos
    public bool ingrediente;
    public SpriteRenderer spriteRenderer;
    public BoxCollider collider;
    private IngredientesSO Ingrediente;

    public Node(Vector2Int pos, Vector3 cellSize, bool walkable, int nodeId)
    {
        position = pos;
        ingrediente = walkable;
        this.nodeid = nodeId;

        GameObject nodeObject = new GameObject(nodeId.ToString()); // Nombre del GameObject = Número de creación
        nodeObject.transform.position = new Vector3(position.x + 0.5f, position.y + 0.5f, -1); // Centrar el nodo

        spriteRenderer = nodeObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 10;
        collider = nodeObject.AddComponent<BoxCollider>();

        // Ajustar escala del sprite al tamaño de la celda del Tilemap
        nodeObject.transform.localScale = new Vector3(cellSize.x, cellSize.y, 1);
    }

    public void SetSprite(Sprite newSprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

    public void SetIngrediente(IngredientesSO ingrediente)
    {
        this.ingrediente = ingrediente;
    }
    public void setterZonas(Dictionary<int, Node> posiciones)
    {
        zonas = posiciones;
    }


    public void radio_efecto(int rango)
    {
        neighbors.Clear();
        int vecino_lateral_derecho = nodeid + 8 * rango;
        int vecino_lateral_izquierdo = nodeid - 8 * rango;
        int vecino_diagonal_superior_derecho = nodeid + 9 * rango;
        int vecino_diagonal_superior_izquierdo = nodeid - 9 * rango;
        int vecino_diagonal_inferior_derecho = nodeid + 7 * rango;
        int vecino_diagonal_inferior_izquierdo = nodeid - 7 * rango;
        int vecino_arriba = nodeid + 1 * rango;
        int vecino_abajo = nodeid - 1 * rango;
        List<int> vecinillos = new List<int> { vecino_lateral_derecho, vecino_lateral_izquierdo, vecino_diagonal_superior_derecho, vecino_diagonal_superior_izquierdo, vecino_diagonal_inferior_derecho, vecino_diagonal_inferior_izquierdo };
        for (int i = 0; i < vecinillos.Count; i++)
        {
            if (zonas[vecinillos[i]] != null)
            {
                neighbors.Add(zonas[vecinillos[i]]);
            }
            else
            {
                continue;
            }
        }

    }

    public void PasivaIngrediente()
    {
        Ingrediente.pasiva();
    }
}
