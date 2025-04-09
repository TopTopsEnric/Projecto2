using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economia : MonoBehaviour
{
    public float money = 0;
    private int cebolla = 1;

    public void more_money(float incremeneto)
    {

        money += incremeneto*1;
    }

    public void less_money(float restante)
    {
        money -= restante;
    }

    public void activarefectoCebolla()
    {
        cebolla = 2;
    }
    public void desactivarefectoCebolla()
    {
        cebolla = 1;
    }


}
