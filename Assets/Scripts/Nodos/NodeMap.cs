
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class NodeMap : MonoBehaviour
{
    public Tilemap tilemap;
    public Dictionary<int, Node> nodes = new Dictionary<int, Node>();
    public int width;
    public int height;
    private int nodeCounter = 1;

    void Start()
    {
        GenerateMapFromTilemap();
    }

    void GenerateMapFromTilemap()
    {
        Vector3 cellSize = tilemap.cellSize; // Tamaño del tile

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                // Si el tile está pintado, se crea un nodo
                if (tilemap.HasTile(tilePosition))
                {
                    Vector3 nodePosition = tilemap.GetCellCenterWorld(tilePosition);
                    Node newNode = new Node(new Vector2Int(x, y), cellSize, false, nodeCounter);
                    nodes.Add(nodeCounter, newNode);
                    nodeCounter++;
                }
            }
        }
        for (int i = 1; i < nodes.Count; i++)
        {

            if (nodes.ContainsKey(i))
            {
                nodes[i].setterZonas(nodes);
            }
        }
    }

    public void DetectorFormaciones()
    {
        for (int i = 1; i < nodes.Count; i++)
        {
            
        }
    }

    

    // Método para cambiar el sprite de un nodo específico por su número de creación
    public void ChangeNodeSprite(int nodeId, ResourcesSO recurso)
    {
        if (nodes.ContainsKey(nodeId))
        {
            nodes[nodeId].SetIngrediente(recurso);
        }
    }
}

