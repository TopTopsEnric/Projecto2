using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "resource", menuName = "Tools/resources")]

public abstract class ResourcesSO : ScriptableObject
{
    public string Name;
    public string Description;
    public float Price;
    public int Limit;
    public int range;
    public int nivel;
    public string forma;
    public Sprite Sprite;
    public bool esmovible;
    public List<int> niveles_ignorar = new List<int>();


    // Datos para modelos 3D
    public Mesh Mesh;          // Malla del objeto 3D
    public Material Material;  // Material del objeto 3D


    public abstract void ActivarEfecto(List<Node> neighbors);
    
}
