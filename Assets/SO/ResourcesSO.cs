using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "resource", menuName = "Tools/resources")]

public abstract class ResourcesSO : ScriptableObject
{
    public string Name;
    public string Description;
    public float Price;
    public int range;
    public float vida;
    public string forma;
    public Sprite Sprite;
    public bool esmovible;
    public List<int> niveles_ignorar = new List<int>();



    public abstract void ActivarEfecto(List<Node> neighbors, Node nodoOrigen);
    
}
