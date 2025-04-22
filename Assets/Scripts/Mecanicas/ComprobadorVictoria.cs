using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComprobadorVictoria : MonoBehaviour
{
    public NodeMap nodeMapJugador1;
    public NodeMap nodeMapJugador2;

    // Receta: IngredienteSO -> cantidad mínima requerida
    public List<IngredienteCantidad> recetaMinimos = new List<IngredienteCantidad>();

    [System.Serializable]
    public class IngredienteCantidad
    {
        public IngredientesSO ingrediente;
        public int cantidadMinima;
    }

    public string ComprobarVictoria()
    {
        var conteoJugador1 = ContarIngredientes(nodeMapJugador1);
        var conteoJugador2 = ContarIngredientes(nodeMapJugador2);

        bool cumpleJugador1 = CumpleReceta(conteoJugador1);
        bool cumpleJugador2 = CumpleReceta(conteoJugador2);

        if (cumpleJugador1 && !cumpleJugador2)
        {
            return "Jugador 1 gana: cumple receta y jugador 2 no.";
        }
        else if (!cumpleJugador1 && cumpleJugador2)
        {
            return "Jugador 2 gana: cumple receta y jugador 1 no.";
        }
        else if (!cumpleJugador1 && !cumpleJugador2)
        {
            int valor1 = CalcularValorTotal(conteoJugador1);
            int valor2 = CalcularValorTotal(conteoJugador2);

            if (valor1 < valor2)
                return "Jugador 1 gana: menor valor total de ingredientes.";
            else if (valor2 < valor1)
                return "Jugador 2 gana: menor valor total de ingredientes.";
            else
                return "Empate: mismos valores totales.";
        }
        else
        {
            int valorSobrante1 = CalcularValorSobrante(conteoJugador1);
            int valorSobrante2 = CalcularValorSobrante(conteoJugador2);

            if (valorSobrante1 < valorSobrante2)
                return "Jugador 1 gana: menor valor sobrante.";
            else if (valorSobrante2 < valorSobrante1)
                return "Jugador 2 gana: menor valor sobrante.";
            else
                return "Empate: mismos valores sobrantes.";
        }
    }

    private Dictionary<IngredientesSO, int> ContarIngredientes(NodeMap nodeMap)
    {
        Dictionary<IngredientesSO, int> conteo = new Dictionary<IngredientesSO, int>();

        foreach (var node in nodeMap.nodes.Values)
        {
            if (node.ingrediente && node.recurso != null)
            {
                IngredientesSO ingrediente = node.recurso as IngredientesSO;
                if (ingrediente != null)
                {
                    if (!conteo.ContainsKey(ingrediente))
                        conteo[ingrediente] = 0;
                    conteo[ingrediente]++;
                }
            }
        }

        return conteo;
    }

    private bool CumpleReceta(Dictionary<IngredientesSO, int> conteo)
    {
        foreach (var item in recetaMinimos)
        {
            if (!conteo.ContainsKey(item.ingrediente) || conteo[item.ingrediente] < item.cantidadMinima)
                return false;
        }
        return true;
    }

    private int CalcularValorTotal(Dictionary<IngredientesSO, int> conteo)
    {
        int total = 0;
        foreach (var par in conteo)
        {
            int valor = Mathf.RoundToInt(par.Key.Price);
            total += valor * par.Value;
        }
        return total;
    }

    private int CalcularValorSobrante(Dictionary<IngredientesSO, int> conteo)
    {
        int total = 0;
        foreach (var par in conteo)
        {
            int minimo = 0;
            var item = recetaMinimos.Find(x => x.ingrediente == par.Key);
            if (item != null)
                minimo = item.cantidadMinima;

            int sobrante = par.Value - minimo;
            if (sobrante > 0)
            {
                int valor = Mathf.RoundToInt(par.Key.Price);
                total += valor * sobrante;
            }
        }
        return total;
    }
}
