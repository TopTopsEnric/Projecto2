using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Utensilios", menuName = "Tools/resources/Utensilios")]

public abstract class Utensilio : ResourcesSO
{
    public static NodeMap nodeMap;
    public virtual int nodosRequeridos => 1;

}
