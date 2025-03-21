using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New efecto", menuName = "Tools/resources/efectos")]
public class Efectos : ResourcesSO
{
    public void efecte()
    {
        Debug.Log("Inicializando utensilios...");
        Price = Random.Range(1f, 100f); // Precio aleatorio entre 1 y 100
    }
}
