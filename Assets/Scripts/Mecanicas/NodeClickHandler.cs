using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeClickHandler : MonoBehaviour
{
    private Node nodoDestino;
    private Node nodoOrigen;
    private IngredientesSO ingrediente;
    private AsignarPieza piezas;

    public void Init(Node destino, Node origen, IngredientesSO ingrediente, AsignarPieza piezas)
    {
        this.nodoDestino = destino;
        this.nodoOrigen = origen;
        this.ingrediente = ingrediente;
        this.piezas = piezas;
    }

    private void OnMouseDown()
    {
        if (!nodoDestino.ingrediente && ingrediente != null)
        {
            nodoOrigen.destruirIngrediente();
            nodoDestino.SetIngrediente(ingrediente);

            // Limpiar visualmente los otros nodos
            if (ingrediente is Tomate tomate)
            {
                tomate.LimpiarNodosResaltados();
                piezas.inputHabilitado = true;
            }

            Destroy(this);
        }
    }
}
