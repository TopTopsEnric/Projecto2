using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cebolla", menuName = "Tools/resources/Ingredients/Cebolla")]
public class Cebolla : IngredientesSO
{
     public Economia economia;
    public override void ActivarEfecto(List<Node> neighbors)
    {
        if (neighbors.Count == 0) {
            economia.activarefectoCebolla();
        }
        else {
            economia.desactivarefectoCebolla();
        }

    }
}
