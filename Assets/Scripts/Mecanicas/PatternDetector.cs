using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class FormationPattern
{
    public string Name;                       // Nombre del patr�n
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
        // Define tus patrones aqu� usando vectores de posiciones relativas
        patterns.Add(new FormationPattern(
            name: "L�nea Horizontal",
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

        // A�ade m�s patrones seg�n necesites
    }

    // M�todo principal que se llamar� desde NodeMap
    public void DetectarFormaciones(Dictionary<int, Node> nodes)
    {
        // Para cada patr�n, intentamos detectarlo en el mapa
        foreach (var pattern in patterns)
        {
            DetectarPatron(pattern, nodes);
        }
    }

    // M�todo para detectar un patr�n espec�fico
    private void DetectarPatron(FormationPattern pattern, Dictionary<int, Node> nodes)
    {
        // Recorremos cada nodo como posible origen del patr�n
        foreach (var startNodeEntry in nodes)
        {
            Node startNode = startNodeEntry.Value;
            if (!startNode.ingrediente)
                continue;

            ResourcesSO startResource = GetNodeResource(startNode);
            if (startResource == null)
                continue;

            // Solo comprobamos si este nodo puede ser parte del patr�n buscado
            if (!IsValidIngredientForPattern(startResource.Name, pattern.RequiredElements))
                continue;

            // Intentamos encontrar el patr�n comenzando desde este nodo
            TryMatchPatternFromNode(startNode, pattern, nodes);
        }
    }

    // Intenta hacer coincidir un patr�n desde un nodo espec�fico
    private void TryMatchPatternFromNode(Node startNode, FormationPattern pattern, Dictionary<int, Node> nodes)
    {
        // Para cada orientaci�n posible (0�, 90�, 180�, 270�)
        for (int rotation = 0; rotation < 4; rotation++)
        {
            // Tambi�n comprobamos reflejos (para cada rotaci�n)
            for (int reflection = 0; reflection < 2; reflection++)
            {
                List<Node> matchingNodes = new List<Node>();
                matchingNodes.Add(startNode); // El nodo inicial siempre forma parte
                bool patternFound = true;

                // Verificamos cada posici�n relativa en este patr�n (excepto la primera que es 0,0)
                for (int i = 1; i < pattern.Positions.Length; i++)
                {
                    Vector2Int relPos = pattern.Positions[i];

                    // Aplicar rotaci�n y reflexi�n si es necesario
                    Vector2Int transformedPos = TransformPosition(relPos, rotation, reflection == 1);

                    // Calcular la posici�n real en el tablero
                    Vector2Int targetPos = startNode.position + transformedPos;

                    // Buscar el nodo en esa posici�n
                    Node targetNode = FindNodeAtPosition(targetPos, nodes);

                    // Verificar si existe un nodo ah� y si tiene el ingrediente correcto
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

                // Si se encontr� el patr�n completo, aplicar el efecto
                if (patternFound && matchingNodes.Count == pattern.Positions.Length)
                {
                    Debug.Log($"�Patr�n '{pattern.Name}' encontrado! Rotaci�n: {rotation * 90}�, Reflexi�n: {reflection == 1}");
                    pattern.Effect(matchingNodes);
                }
            }
        }
    }

    // Transforma una posici�n relativa aplicando rotaci�n y/o reflexi�n
    private Vector2Int TransformPosition(Vector2Int position, int rotation, bool reflection)
    {
        Vector2Int transformed = position;

        // Aplicar reflexi�n (sobre el eje Y) si se requiere
        if (reflection)
        {
            transformed.x = -transformed.x;
        }

        // Aplicar rotaci�n (en incrementos de 90 grados)
        for (int i = 0; i < rotation; i++)
        {
            // Rotaci�n de 90 grados en sentido horario: (x,y) -> (y,-x)
            int temp = transformed.x;
            transformed.x = transformed.y;
            transformed.y = -temp;
        }

        return transformed;
    }

    // Helper para obtener el ResourcesSO asociado a un nodo
    private ResourcesSO GetNodeResource(Node node)
    {
        // Acceder al recurso del nodo mediante reflexi�n ya que es privado
        Type nodeType = typeof(Node);
        FieldInfo recursoField = nodeType.GetField("recurso", BindingFlags.Instance | BindingFlags.NonPublic);

        if (recursoField != null)
        {
            return recursoField.GetValue(node) as ResourcesSO;
        }

        return null;
    }

    // Helper para encontrar un nodo en una posici�n espec�fica
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

    // Verifica si un ingrediente es v�lido para el patr�n
    private bool IsValidIngredientForPattern(string ingredientName, string[] requiredElements)
    {
        // Si el patr�n requiere ingredientes espec�ficos, verificar que coincida
        if (requiredElements != null && requiredElements.Length > 0)
        {
            return Array.Exists(requiredElements, element => element == ingredientName);
        }

        // Si no hay requisitos espec�ficos, cualquier ingrediente es v�lido
        return true;
    }

    // Ejemplos de m�todos de efecto para cuando se detectan formaciones
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
