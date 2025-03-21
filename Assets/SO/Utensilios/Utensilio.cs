using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ingredient", menuName = "Tools/resources/Utensilios")]

public class Utensilio : ResourcesSO
{
    public void habilidad()
    {
        Debug.Log("Inicializando utensilios...");
        Price = Random.Range(1f, 100f); // Precio aleatorio entre 1 y 100
    }
}
