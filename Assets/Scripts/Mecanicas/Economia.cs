using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economia : MonoBehaviour
{
    public float money = 100; // dinero inicial
    public int cebolla = 1;
    public AsignarPieza pieza;

    public bool PuedePagar(float cantidad)
    {
        return money >= cantidad;
    }

    public void MoreMoney(float incremento)
    {
        money += incremento*cebolla;
    }

    public void LessMoney(float gasto)
    {
        money -= gasto;
        if (money < 0) money = 0;
    }

    public void ActivarEfectoCebolla()
    {
        cebolla = 2;
    }

    public void DesactivarEfectoCebolla()
    {
        cebolla = 1;
    }
}
