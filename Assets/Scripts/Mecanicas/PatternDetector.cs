using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class FormationPattern
{
    public string Name;                       // Nombre del patrón
    public string[] RequiredElements;         // Ingredientes necesarios
    public Vector2Int[] Positions;            // Posiciones relativas
    public Action<List<Node>> Effect;         // Efecto a aplicar cuando se detecta

    // Constructor para patrones basados en posiciones relativas e ingredientes
    public FormationPattern(string name, string[] requiredElements, Vector2Int[] positions, Action<List<Node>> effect)
    {
        Name = name;
        RequiredElements = requiredElements;
        Positions = positions;
        Effect = effect;
    }
}

public class PatternDetector : MonoBehaviour
{
    [SerializeField] private List<FormationPattern> patterns = new List<FormationPattern>();

    private void Awake()
    {
        // Inicializar los patrones predefinidos
        InitializePatterns();
    }

    private void InitializePatterns()
    {
        // Define tus patrones aquí usando vectores de posiciones relativas
        patterns.Add(new FormationPattern(
            name: "Línea Horizontal",
            requiredElements: new string[] { "tomate", "tomate", "tomate" },
            positions: new Vector2Int[] {
                new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0)
            },
            effect: ActivarEfectoLinea
        ));

        patterns.Add(new FormationPattern(
            name: "Cuadrado",
            requiredElements: new string[] { "lechuga", "lechuga", "lechuga", "lechuga" },
            positions: new Vector2Int[] {
                new Vector2Int(0, 0), new Vector2Int(1, 0),
                new Vector2Int(0, 1), new Vector2Int(1, 1)
            },
            effect: ActivarEfectoCuadrado
        ));

        patterns.Add(new FormationPattern(
            name: "L Shape",
            requiredElements: new string[] { "cebolla", "cebolla", "cebolla", "cebolla" },
            positions: new Vector2Int[] {
                new Vector2Int(0, 0), new Vector2Int(0, 1),
                new Vector2Int(0, 2), new Vector2Int(1, 0)
            },
            effect: ActivarEfectoL
        ));

        // Añade más patrones según necesites
    }

    // Método principal que se llamará desde NodeMap
    public void DetectarFormaciones(Dictionary<int, Node> nodes)
    {
        // Para cada patrón, intentamos detectarlo en el mapa
        foreach (var pattern in patterns)
        {
            DetectarPatron(pattern, nodes);
        }
    }

    // Método para detectar un patrón específico
    private void DetectarPatron(FormationPattern pattern, Dictionary<int, Node> nodes)
    {
        // Recorremos cada nodo como posible origen del patrón
        foreach (var startNodeEntry in nodes)
        {
            Node startNode = startNodeEntry.Value;
            if (!startNode.ingrediente)
                continue;

            ResourcesSO startResource = GetNodeResource(startNode);
            if (startResource == null)
                continue;

            // Solo comprobamos si este nodo puede ser parte del patrón buscado
            if (!IsValidIngredientForPattern(startResource.Name, pattern.RequiredElements))
                continue;

            // Intentamos encontrar el patrón comenzando desde este nodo
            TryMatchPatternFromNode(startNode, pattern, nodes);
        }
    }

    // Intenta hacer coincidir un patrón desde un nodo específico
    private void TryMatchPatternFromNode(Node startNode, FormationPattern pattern, Dictionary<int, Node> nodes)
    {
        // Para cada orientación posible (0°, 90°, 180°, 270°)
        for (int rotation = 0; rotation < 4; rotation++)
        {
            // También comprobamos reflejos (para cada rotación)
            for (int reflection = 0; reflection < 2; reflection++)
            {
                List<Node> matchingNodes = new List<Node>();
                matchingNodes.Add(startNode); // El nodo inicial siempre forma parte
                bool patternFound = true;

                // Verificamos cada posición relativa en este patrón (excepto la primera que es 0,0)
                for (int i = 1; i < pattern.Positions.Length; i++)
                {
                    Vector2Int relPos = pattern.Positions[i];

                    // Aplicar rotación y reflexión si es necesario
                    Vector2Int transformedPos = TransformPosition(relPos, rotation, reflection == 1);

                    // Calcular la posición real en el tablero
                    Vector2Int targetPos = startNode.position + transformedPos;

                    // Buscar el nodo en esa posición
                    Node targetNode = FindNodeAtPosition(targetPos, nodes);

                    // Verificar si existe un nodo ahí y si tiene el ingrediente correcto
                    if (targetNode == null || !targetNode.ingrediente)
                    {
                        patternFound = false;
                        break;
                    }

                    ResourcesSO targetResource = GetNodeResource(targetNode);
                    if (targetResource == null || !IsValidIngredientForPattern(targetResource.Name, pattern.RequiredElements))
                    {
                        patternFound = false;
                        break;
                    }

                    matchingNodes.Add(targetNode);
                }

                // Si se encontró el patrón completo, aplicar el efecto
                if (patternFound && matchingNodes.Count == pattern.Positions.Length)
                {
                    Debug.Log($"¡Patrón '{pattern.Name}' encontrado! Rotación: {rotation * 90}°, Reflexión: {reflection == 1}");
                    pattern.Effect(matchingNodes);
                }
            }
        }
    }

    // Transforma una posición relativa aplicando rotación y/o reflexión
    private Vector2Int TransformPosition(Vector2Int position, int rotation, bool reflection)
    {
        Vector2Int transformed = position;

        // Aplicar reflexión (sobre el eje Y) si se requiere
        if (reflection)
        {
            transformed.x = -transformed.x;
        }

        // Aplicar rotación (en incrementos de 90 grados)
        for (int i = 0; i < rotation; i++)
        {
            // Rotación de 90 grados en sentido horario: (x,y) -> (y,-x)
            int temp = transformed.x;
            transformed.x = transformed.y;
            transformed.y = -temp;
        }

        return transformed;
    }

    // Helper para obtener el ResourcesSO asociado a un nodo
    private ResourcesSO GetNodeResource(Node node)
    {
        // Acceder al recurso del nodo mediante reflexión ya que es privado
        Type nodeType = typeof(Node);
        FieldInfo recursoField = nodeType.GetField("recurso", BindingFlags.Instance | BindingFlags.NonPublic);

        if (recursoField != null)
        {
            return recursoField.GetValue(node) as ResourcesSO;
        }

        return null;
    }

    // Helper para encontrar un nodo en una posición específica
    private Node FindNodeAtPosition(Vector2Int position, Dictionary<int, Node> nodes)
    {
        foreach (var node in nodes.Values)
        {
            if (node.position == position)
            {
                return node;
            }
        }
        return null;
    }

    // Verifica si un ingrediente es válido para el patrón
    private bool IsValidIngredientForPattern(string ingredientName, string[] requiredElements)
    {
        // Si el patrón requiere ingredientes específicos, verificar que coincida
        if (requiredElements != null && requiredElements.Length > 0)
        {
            return Array.Exists(requiredElements, element => element == ingredientName);
        }

        // Si no hay requisitos específicos, cualquier ingrediente es válido
        return true;
    }

    // Ejemplos de métodos de efecto para cuando se detectan formaciones
    private void ActivarEfectoLinea(List<Node> nodes)
    {
        Debug.Log($"Muro!!!!!!!!!!");
        
    }

    private void ActivarEfectoCuadrado(List<Node> nodes)
    {
        Debug.Log($"Formacion Cuadrada");
        
    }

    private void ActivarEfectoL(List<Node> nodes)
    {
        Debug.Log($"L de perdicion");
        
    }
}
