
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class NodeMap : MonoBehaviour
{
    public Tilemap tilemap;
    public int jugadorId;
    public int layer;
    public Economia economiaJugador;
    public Dictionary<int, Node> nodes = new Dictionary<int, Node>();
    public int width;
    public int height;
    private int nodeCounter = 1;
    

    // Referencia al detector de patrones
    [SerializeField] private PatternDetector patternDetector;
    void Awake()
    {
        Utensilio.nodeMap = this; 
    }
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
                    Node newNode = new Node(nodePosition,new Vector2Int(x, y), cellSize, nodeCounter,this);
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

    // Método simple que delega la detección de formaciones al PatternDetector
    public void DetectorFormaciones()
    {
        patternDetector.DetectarFormaciones(nodes);
    }

    public void ejecutarPasiva()
    {
        foreach (var node in nodes)
        {
            //Debug.Log(node);
            node.Value.PasivaIngrediente();
        }
    }

    public void EjecutarEfectosTemporales()
    {
        foreach (var node in nodes.Values)
        {
            node.AplicarEfectosTemporales();
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DetectorFormaciones();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ejecutarPasiva();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            EjecutarEfectosTemporales();
            
        }
    }
}

