using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cebolla", menuName = "Tools/resources/Ingredients/Cebolla")]
public class Cebolla : IngredientesSO
{
    public override void ActivarEfecto(List<Node> neighbors, Node nodoOrigen)
    {
        Economia economiaJugador = nodoOrigen.nodemap.economiaJugador;

        if (economiaJugador == null)
        {
            Debug.LogError("EconomiaJugador no asignada en NodeMap.");
            return;
        }

        // Ahora usas economiaJugador para modificar dinero o activar efectos
        bool limpio = true;
        foreach (var vecino in neighbors)
        {
            if (vecino.ingrediente)
            {
                limpio = false;
                break;
            }
        }

        if (limpio)
        {
            economiaJugador.ActivarEfectoCebolla();
            Debug.Log("Efecto cebolla activado");
        }
        else
        {
            economiaJugador.DesactivarEfectoCebolla();
            Debug.Log("Efecto cebolla desactivado");
        }
    }

}
