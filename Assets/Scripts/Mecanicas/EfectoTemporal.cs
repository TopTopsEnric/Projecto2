using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfectoTemporal : MonoBehaviour
{
    public Efectos efecto;
    public int turnosRestantes;

    public EfectoTemporal(Efectos efecto, int duracion)
    {
        this.efecto = efecto;
        this.turnosRestantes = duracion;
    }
}
