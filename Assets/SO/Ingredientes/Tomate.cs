using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Tomate", menuName = "Tools/resources/Ingredients/Tomate")]


public class Tomate : IngredientesSO
{

    public Sprite marcadorVisual;

    private List<Node> nodosResaltados = new List<Node>();

    private AsignarPieza piezas;

    

    public override void ActivarEfecto(List<Node> neighbors, Node nodoOrigen)
    {
        piezas = nodoOrigen.nodemap.economiaJugador.pieza;
        nodosResaltados.Clear();
        foreach (var item in neighbors)
        {
            if (item == null)
            {
                Debug.LogWarning("Un item en neighbors es null.");
                continue;
            }

            if (!item.ingrediente)
            {
                nodosResaltados.Add(item);

                if (item.spriteRenderer == null)
                {
                    Debug.LogWarning($"El nodo {item.recurso.name} no tiene spriteRenderer.");
                }
                else
                {
                    if (item.spriteRenderer.sprite == null && marcadorVisual != null)
                    {
                        item.spriteRenderer.sprite = marcadorVisual;
                    }

                    item.spriteRenderer.color = Color.red;
                }

                if (item.collider == null)
                {
                    Debug.LogWarning($"El nodo {item.recurso.name} no tiene collider.");
                }
                else if (item.collider.GetComponent<NodeClickHandler>() == null)
                {
                    piezas.inputHabilitado = false;
                    var clickHandler = item.collider.gameObject.AddComponent<NodeClickHandler>();
                    clickHandler.Init(item, nodoOrigen, this, piezas);
                }
            }
        }
        if (nodosResaltados.Count == 0)
        {
            Debug.Log("No hay nodos vecinos disponibles");
        }
    }

    public void LimpiarNodosResaltados()
    {
        foreach (var nodo in nodosResaltados)
        {
            // Solo limpiar si aún no tiene ingrediente
            if (!nodo.ingrediente)
            {
                nodo.spriteRenderer.sprite = null;
                nodo.spriteRenderer.color = Color.white;

                // Eliminar el click handler si aún existe
                var handler = nodo.collider.GetComponent<NodeClickHandler>();
                if (handler != null)
                {
                    Destroy(handler);
                }
            }
        }

        nodosResaltados.Clear();
    }
}
