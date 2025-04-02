using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asignar_Ingrediente : MonoBehaviour
{
    public AsignarPieza asignar;
    public GameObject cursor;
    

    public void SetSprite(ResourcesSO recurso)
    {
        if (cursor != null)
        {
            SpriteRenderer cursorRenderer = cursor.GetComponent<SpriteRenderer>();
            if (cursorRenderer != null)
            {
                cursorRenderer.sprite = recurso.Sprite;
            }
        }

        if (asignar != null)
        {
            asignar.recurso = recurso;
        }
    }
}
