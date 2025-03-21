using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsignarPieza : MonoBehaviour
{
    // Start is called before the first frame update
    public NodeMap nodeMap;
    public Sprite newSprite;

    public void cambiarvisual()
    {
        nodeMap.ChangeNodeSprite(8, newSprite); // Cambia el nodo en la posición (2,3)
    }
}
