using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Tomate", menuName = "Tools/resources/Ingredients/Tomate")]


public class Tomate : IngredientesSO
{

    public Sprite marcadorVisual;

    private List<Node> nodosResaltados = new List<Node>();

    private AsignarPieza piezas;

    private void BuscaPiezas()
    {
        if (piezas == null)
        {
            piezas = UnityEngine.Object.FindObjectOfType<AsignarPieza>();
            if (piezas == null)
            {
                Debug.LogError("No se encontró un componente Asignar en la escena.");
            }
        }
    }

    public override void ActivarEfecto(List<Node> neighbors, Node nodoOrigen)
    {
        BuscaPiezas();
        nodosResaltados.Clear();

        foreach (var item in neighbors)
        {
            if (!item.ingrediente)
            {
                nodosResaltados.Add(item);

                if (item.spriteRenderer != null)
                {
                    if (item.spriteRenderer.sprite == null && marcadorVisual != null)
                    {
                        item.spriteRenderer.sprite = marcadorVisual;
                    }

                    item.spriteRenderer.color = Color.red;
                }

                if (item.collider != null && item.collider.GetComponent<NodeClickHandler>() == null)
                {
                    piezas.inputHabilitado = false;
                    var clickHandler = item.collider.gameObject.AddComponent<NodeClickHandler>();
                    clickHandler.Init(item, nodoOrigen, this,piezas);
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
