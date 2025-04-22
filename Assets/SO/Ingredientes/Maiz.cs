using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Maiz", menuName = "Tools/resources/Ingredients/Maiz")]
public class Maiz : IngredientesSO
{
    public override void ActivarEfecto(List<Node> neighbors, Node nodoOrigen)
    {
        Economia economiaJugador = nodoOrigen.nodemap.economiaJugador;

        if (economiaJugador == null)
        {
            Debug.LogError("EconomiaJugador no asignada en NodeMap.");
            return;
        }

        economiaJugador.MoreMoney(10);
    }
}
