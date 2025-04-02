using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asignar_Ingrediente : MonoBehaviour
{
    public AsignarPieza asignar;
    public GameObject cursor;
    

    public void SetSprite(Sprite newSprite)
    {
        if (cursor != null)
        {
            SpriteRenderer cursorRenderer = cursor.GetComponent<SpriteRenderer>();
            if (cursorRenderer != null)
            {
                cursorRenderer.sprite = newSprite;
            }
        }

        if (asignar != null)
        {
            asignar.newSprite = newSprite;
        }
    }
}
