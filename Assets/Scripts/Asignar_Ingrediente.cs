using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Asignar_Ingrediente : MonoBehaviour
{
    public AsignarPieza asignar;
    public GameObject cursor; // Ahora el cursor debe tener un componente Image

    public void SetSprite(ResourcesSO recurso)
    {
        Debug.Log($"SetSprite llamado en {gameObject.name} con recurso {recurso.Name}");

        if (cursor != null)
        {
            Image cursorImage = cursor.GetComponent<Image>();
            if (cursorImage != null)
            {
                cursorImage.sprite = recurso.Sprite;
                
            }
            else
            {
                Debug.LogWarning("No se encontró componente Image en cursor");
            }
        }
        else
        {
            Debug.LogWarning("Cursor es null");
        }

        if (asignar != null)
        {
            asignar.SetRecurso(recurso);
            Debug.Log($"SetRecurso llamado en asignar {asignar.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("Asignar es null");
        }
    }
}
