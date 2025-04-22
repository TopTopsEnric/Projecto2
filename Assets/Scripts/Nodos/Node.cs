
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Node
{
    public int nodeid;
    public NodeMap nodemap;
    public int rango;
    public float vida;
    public string forma;
    public Vector2Int position; 
    public Dictionary<int, Node> zonas = new Dictionary<int, Node>();
    public List<Node> neighbors = new List<Node>(); 
    public bool ingrediente;
    public SpriteRenderer spriteRenderer;
    public BoxCollider collider;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    public  ResourcesSO recurso;
    public bool esmovible;
    public List<int> saltables = new List<int>();
    public List<EfectoTemporal> efectosTemporales = new List<EfectoTemporal>();
    public SpriteRenderer utensilioSpriteRenderer;

    public Node(Vector3 worldPosition,Vector2Int pos, Vector3 cellSize, int nodeId, NodeMap nodemap)
    {
        position = pos;
        this.nodeid = nodeId;
        this.nodemap = nodemap;
        GameObject nodeObject = new GameObject(nodeId.ToString());
        nodeObject.transform.position = worldPosition; 

        nodeObject.tag = "Nodo";
        nodeObject.layer = nodemap.layer;

        // Crear spriteRenderer para ingrediente primero
        spriteRenderer = nodeObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Default";  // Misma capa para ambos
        spriteRenderer.sortingOrder = 2;               // Orden 2 para ingrediente

        // Crear GameObject hijo para utensilio
        GameObject utensilioObject = new GameObject("UtensilioSprite");
        utensilioObject.layer = nodemap.layer;
        utensilioObject.transform.parent = nodeObject.transform;
        utensilioObject.transform.localPosition = Vector3.zero;

        // Crear spriteRenderer para utensilio
        utensilioSpriteRenderer = utensilioObject.AddComponent<SpriteRenderer>();
        utensilioSpriteRenderer.sortingLayerName = "Default";  // Misma capa
        utensilioSpriteRenderer.sortingOrder = 3;               // Orden 3 para utensilio (encima)
        utensilioSpriteRenderer.enabled = false;

        collider = nodeObject.AddComponent<BoxCollider>();

        nodeObject.transform.localScale = new Vector3(cellSize.x, cellSize.y, 1);
    }

    public void AgregarEfectoTemporal(Efectos efecto, int duracion)
    {
        efectosTemporales.Add(new EfectoTemporal(efecto, duracion));
    }

    public void SetUtensilioVisual(ResourcesSO utensilio)
    {
        if (utensilio != null && utensilio.Sprite != null)
        {
            utensilioSpriteRenderer.sprite = utensilio.Sprite;
            utensilioSpriteRenderer.enabled = true;
        }
        else
        {
            utensilioSpriteRenderer.sprite = null;
            utensilioSpriteRenderer.enabled = false;
        }
    }

    public void AplicarEfectosTemporales()
    {
        for (int i = efectosTemporales.Count - 1; i >= 0; i--)
        {
            var efecto = efectosTemporales[i];

            // Ejecutar el efecto si tiene lógica de ejecución por turno
            efecto.efecto.EjecutarTurno(this);

            efecto.turnosRestantes--;

            if (efecto.turnosRestantes <= 0)
            {
                efectosTemporales.RemoveAt(i);
            }
        }
    }

    public void destruirIngrediente()
    {
        recurso = null;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
        }
        else
        {
            Debug.LogError("spriteRenderer is null in node " + nodeid);
        }
        //meshFilter.mesh = null;
        // meshRenderer.material = null;
        esmovible = false;
        ingrediente = false;
        rango = 0;
        saltables = new List<int>();

    }

    public void SetIngrediente(ResourcesSO so)
    {
        if (recurso != null)
        {
            destruirIngrediente();  // Destruir el ingrediente actual si existe
        }

        if (so == null)
        {
            Debug.LogError("ResourcesSO is null when setting ingredient for node " + nodeid);
            return;
        }

        recurso = so;  // Referencia al recurso original

        // Seteo de los campos visuales
        if (spriteRenderer != null)
        {
            if (so.Sprite == null)
            {
                Debug.LogError("Sprite is null in ResourcesSO for node " + nodeid);
            }
            else
            {
                try
                {
                    spriteRenderer.sprite = so.Sprite;
                   // Debug.Log("Successfully set sprite for node " + nodeid);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error setting sprite: " + e.Message);
                }
            }
        }
        else
        {
            Debug.LogError("spriteRenderer is null in node " + nodeid + " when setting ingredient");
        }

        // Seteo de los demás campos del ScriptableObject
        esmovible = so.esmovible;
        ingrediente = true;
        rango = so.range;
        vida = so.vida;
        saltables = new List<int>(so.niveles_ignorar);
    }

    public void setterZonas(Dictionary<int, Node> posiciones)
    {
        zonas = posiciones;
    }


    public void radio_efecto(int rango, string forma, List<int> ignorarNiveles = null)
    {
        neighbors.Clear();

        foreach (var zona in zonas.Values)
        {
            if (zona == this) continue; // No añadirse a sí mismo

            int dx = Mathf.Abs(zona.position.x - this.position.x);
            int dy = Mathf.Abs(zona.position.y - this.position.y);

            int distancia = Mathf.Max(dx, dy); // Distancia tipo "grid" (como en tilemaps)

            if (distancia > rango) continue; // Si está fuera de rango, no lo añado

            if (ignorarNiveles != null && ignorarNiveles.Contains(distancia)) continue;

            switch (forma)
            {
                case "cruz":
                    if ((dx == 0 && dy != 0) || (dx != 0 && dy == 0))
                        neighbors.Add(zona);
                    break;

                case "x":
                    if (dx == dy && dx != 0)
                        neighbors.Add(zona);
                    break;

                case "cuadrado":
                    if (distancia != 0)
                    {
                        neighbors.Add(zona);
                    }
                    break;

                case "utensilio":
                    if ((dx == 0 && dy != 0)) // Solo arriba y abajo
                        neighbors.Add(zona);
                    break;

                default:
                    Debug.LogWarning("Forma no reconocida");
                    break;
            }
        }

        // Solo los de cuadrado quiero que estén ordenados
        if (forma == "cuadrado")
        {
            neighbors = neighbors.OrderBy(n =>
            {
                Vector2 dir = new Vector2(n.position.x - this.position.x, n.position.y - this.position.y);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                angle = (angle + 360) % 360;
                return angle;
            }).ToList();
        }
        else if (forma == "cruz")
        {
            neighbors = neighbors.OrderBy(n =>
            {
                if (n.position.y > this.position.y) return 0; // Abajo (prioridad máxima)
                if (n.position.x < this.position.x) return 1; // Izquierda
                if (n.position.x > this.position.x) return 2; // Derecha
                return 3; // Arriba
            }).ToList();
        }
    }

    public void PasivaIngrediente()
    {

       
        if (recurso!=null)
        {
            Debug.Log(position);
            radio_efecto(rango, recurso.forma, saltables);
            foreach (var item in neighbors)
            {
                //Debug.Log(item.position);
            }


            recurso.ActivarEfecto(neighbors, this); 
        }
        else
        {
            Debug.Log("NODE VACIO");
        }
         
    }
}
