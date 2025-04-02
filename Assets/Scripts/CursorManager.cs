using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private void Update()
    {
        // Convertir la posición del mouse en coordenadas del mundo
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Mover el cursor a la posición del ratón
        transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
    }
}
