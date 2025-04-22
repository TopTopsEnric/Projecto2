using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Canvas canvas; // Asignar el Canvas del jugador en inspector
    public RectTransform cursorRectTransform; // El UI Image o elemento que representa el cursor

    void Update()
    {
        Vector2 pos;
        // Convertir la posición del mouse a coordenadas locales del Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out pos);

        // Mover el cursor UI a esa posición local
        cursorRectTransform.localPosition = pos;
    }
}
